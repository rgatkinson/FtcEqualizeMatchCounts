using System;
using System.CodeDom.Compiler;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using FEMC.DAL;
using Microsoft.Data.Sqlite;

namespace FEMC
    {
    class Database : IDisposable
        {
        //---------------------------------------------------------------------------------------------------
        // State
        //---------------------------------------------------------------------------------------------------

        public SqliteConnection Connection = null;
        public SqliteTransaction Transaction = null;
        public Tables Tables;
        public readonly int? AveragingMatchCountGoal;

        public readonly IDictionary<long, Team>                          TeamsByNumber = new Dictionary<long, Team>();
        public readonly IDictionary<FMSTeamId, Team>                     TeamsById = new Dictionary<FMSTeamId, Team>();
        public readonly List<Team>                                       Teams = new List<Team>();
        public readonly IDictionary<long, ScheduledMatch>                ScheduledMatchesByNumber = new Dictionary<long, ScheduledMatch>();
        public readonly IDictionary<FMSScheduleDetailId, ScheduledMatch> ScheduledMatchesById = new Dictionary<FMSScheduleDetailId, ScheduledMatch>();
        public readonly IDictionary<long, List<PlayedMatch>>             PlayedMatchesByNumber = new Dictionary<long, List<PlayedMatch>>();
        public readonly IDictionary<long, LeagueHistoryMatch>            LeagueHistoryMatchesByNumber = new Dictionary<long, LeagueHistoryMatch>();
        public readonly IDictionary<string, Event>                       EventsByCode = new Dictionary<string, Event>();

        private List<EqualizationMatch> equalizationMatches = new List<EqualizationMatch>();

        public int MaxAveragingMatchCount
            {
            get {
                int result = 0;
                foreach (Team team in Teams)
                    {
                    result = Math.Max(result, team.AveragingMatchCount);
                    }
                return result;
                }
            }

        public string EqualizationMatchCreatorName => "FTC Equalize Match Counts";

        public string ThisEventCode => Tables.Config.Map["code"].Value.NonNullValue;
        public Event ThisEvent => EventsByCode[ThisEventCode];
        public FMSEventId FMSEventId => new FMSEventId(Tables.Config.Map["FMSEventId"].Value.NonNullValue);

        public List<Event> OtherEvents
            {
            get {
                List<Event> result = new List<Event>();
                foreach (var anEvent in EventsByCode.Values)
                    {
                    if (anEvent != ThisEvent)
                        {
                        result.Add(anEvent);
                        }
                    }
                result.Sort((a,b) => (int)(a.StartNonNull.ToUniversalTime().Ticks - b.StartNonNull.ToUniversalTime().Ticks));
                return result;
                }
            }


        string fileName = null;
        bool disposed = false;

        //---------------------------------------------------------------------------------------------------
        // Construction
        //---------------------------------------------------------------------------------------------------

        public Database(ProgramOptions programOptions)
            {
            fileName = Path.GetFullPath(programOptions.Filename);
            Tables = new Tables(this);
            AveragingMatchCountGoal = programOptions.AverageToExistingMax ? (int?)null : programOptions.AveragingMatchCountGoal;
            }

        ~Database()
            {
            Dispose(false);
            }

        public void Dispose() // called by consumers
            {
            Dispose(true);
            GC.SuppressFinalize(this);
            }

        protected virtual void Dispose(bool explicitlyDisposed)
            {
            if (!disposed)
                { 
                if (explicitlyDisposed)
                    {
                    // Free managed objects
                    Close();
                    }
                // Free native objects
                disposed = true;
                }
            }

        //---------------------------------------------------------------------------------------------------
        // Opening and closing
        //---------------------------------------------------------------------------------------------------

        public void Open()
            {
            Close();

            string uri = new System.Uri(fileName).AbsoluteUri;
            string cs = "Filename=" + fileName;

            Connection = new SqliteConnection(cs);
            Connection.Open();
            }

        public bool IsTransactionInProgress => Transaction != null;

        public void BeginTransaction()
            {
            Trace.Assert(!IsTransactionInProgress);
            Transaction = Connection.BeginTransaction(IsolationLevel.Serializable);
            }

        public void CommitTransaction()
            {
            Trace.Assert(IsTransactionInProgress);
            Transaction.Commit();
            Transaction = null;
            }

        public void AbortTransaction()
            {
            if (IsTransactionInProgress)
                {
                Transaction.Rollback();
                Transaction = null;
                }
            }

        public void Close()
            {
            if (Connection != null)
                {
                AbortTransaction();
                Connection.Close();
                Connection = null;
                }
            }

        //---------------------------------------------------------------------------------------------------
        // Querying
        //---------------------------------------------------------------------------------------------------

        public void Load()
            {
            Tables.Clear();
            Tables.Load();

            ClearDataAccessLayer();
            LoadDataAccessLayer();
            }

        void ClearDataAccessLayer()
            {
            TeamsById.Clear();
            TeamsByNumber.Clear();
            Teams.Clear();
            ScheduledMatchesByNumber.Clear();
            ScheduledMatchesById.Clear();
            PlayedMatchesByNumber.Clear();
            LeagueHistoryMatchesByNumber.Clear();
            EventsByCode.Clear();

            equalizationMatches.Clear();
            }

        public void LoadDataAccessLayer()
            {
            foreach (var row in Tables.LeagueMeets.Rows)
                {
                Event anEvent = new Event(this, row);
                EventsByCode[anEvent.EventCode] = anEvent;
                }

            foreach (var row in Tables.Team.Rows)
                {
                Team team = new Team(this, row);
                TeamsByNumber[team.TeamNumber] = team;
                TeamsById[team.TeamId] = team;
                Teams.Add(team);
                }
            Teams.Sort((a, b) => a.TeamNumber - b.TeamNumber);

            foreach (var row in Tables.ScheduledMatch.Rows)
                {
                ScheduledMatch scheduledMatch = new ScheduledMatch(this, row);
                }

            foreach (var row in Tables.PlayedMatch.Rows)
                {
                PlayedMatch playedMatch = new PlayedMatch(this, row);
                if (!PlayedMatchesByNumber.TryGetValue(playedMatch.MatchNumber, out List<PlayedMatch> playedMatches))
                    {
                    playedMatches = new List<PlayedMatch>();
                    PlayedMatchesByNumber[playedMatch.MatchNumber] = playedMatches;
                    }
                playedMatches.Add(playedMatch);
                }

            foreach (var row in Tables.LeagueHistory.Rows)
                {
                LeagueHistoryMatch.Process(this, row);
                }
            }

        public void ReportEvents(IndentedTextWriter writer)
            {
            writer.WriteLine("This event:");
            writer.Indent++;
            ThisEvent.Report(writer);
            writer.WriteLine($"{Teams.Count} teams participating");
            writer.Indent--;

            List<Event> otherEvents = OtherEvents;
            if (otherEvents.Count > 0)
                {
                writer.WriteLine();
                writer.WriteLine("Previous events:");
                bool first = true;
                writer.Indent++;
                foreach (var otherEvent in otherEvents)
                    {
                    if (!first)
                        {
                        writer.WriteLine();
                        }
                    otherEvent.Report(writer);
                    first = false;
                    }
                writer.Indent--;
                }
            }

        public int ReportTeams(IndentedTextWriter writer, bool verbose)
            {
            int matchCountGoal = AveragingMatchCountGoal ?? MaxAveragingMatchCount;

            writer.WriteLine($"Teams: averaging match count goal: {matchCountGoal}");
            writer.Indent++;

            ISet<Team> completedTeams = new HashSet<Team>();
            IDictionary<Team, int> matchesNeededByTeam = new ConcurrentDictionary<Team, int>();

            int teamsReported = 0;
            int totalTeamEqualizationMatchesNeeded = 0;
            foreach (Team team in Teams)
                {
                int teamEqualizationMatchesNeeded = matchCountGoal - team.AveragingMatchCount;
                totalTeamEqualizationMatchesNeeded += teamEqualizationMatchesNeeded;

                // Report
                if (verbose || teamEqualizationMatchesNeeded > 0)
                    { 
                    if (verbose)
                        {
                        writer.WriteLine();
                        }
                    team.Report(writer, verbose, matchCountGoal);
                    teamsReported += 1;
                    }

                // Accumulate info to make equalization plan
                if (teamEqualizationMatchesNeeded == 0)
                    {
                    completedTeams.Add(team);
                    }
                else
                    {
                    matchesNeededByTeam[team] = teamEqualizationMatchesNeeded;
                    }
                }

            List<Team> rotating = new List<Team>(completedTeams);
            equalizationMatches = new List<EqualizationMatch>();
            while (matchesNeededByTeam.Count > 0)
                {
                // Get a complement of teams that all need equalization matches
                var teams = new List<Team>(matchesNeededByTeam.Keys.Take(4));
                
                // Round out to 4 with teams that will be surrogates. Rotate through who we decide to use (just for fun)
                var surrogates = new List<Team>(rotating.Take(4 - teams.Count));
                rotating.RemoveRange(0, surrogates.Count);
                rotating.AddRange(surrogates);

                teams.AddRange(surrogates);
                var isSurrogates = new List<bool>(teams.Select(team => surrogates.Contains(team)));

                EqualizationMatch equalizationMatch = new EqualizationMatch(this, teams, isSurrogates);
                equalizationMatches.Add(equalizationMatch);

                foreach (Team team in teams)
                    {
                    if (!surrogates.Contains(team))
                        {
                        matchesNeededByTeam[team] = matchesNeededByTeam[team]-1;
                        if (matchesNeededByTeam[team] == 0)
                            {
                            matchesNeededByTeam.Remove(team);
                            completedTeams.Add(team);
                            rotating.Add(team);
                            }
                        }
                    }
                }
            if (teamsReported == 0)
                {
                writer.WriteLine();
                writer.WriteLine("All teams up to date");
                }
            else
                {
                writer.WriteLine("----------------");
                writer.WriteLine($"Total: needed {totalTeamEqualizationMatchesNeeded} matches can be accomplished in {equalizationMatches.Count} equalization matches");
                }

            writer.Indent--;
            return equalizationMatches.Count;
            }

        public int SaveEqualizationMatches(IndentedTextWriter writer, bool verbose)
            {
            foreach (var equalizationMatch in equalizationMatches)
                {
                equalizationMatch.SaveToDatabase();
                }

            int result = equalizationMatches.Count;
            equalizationMatches.Clear();
            return result;
            }
        }
    }
