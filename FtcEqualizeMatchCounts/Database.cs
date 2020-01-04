using FEMC.DAL;
using Microsoft.Data.Sqlite;
using System;
using System.CodeDom.Compiler;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace FEMC
    {
    class Database : IDisposable
        {
        //---------------------------------------------------------------------------------------------------
        // State
        //---------------------------------------------------------------------------------------------------

        public SqliteConnection     Connection = null;
        public SqliteTransaction    Transaction = null;
        public Tables               Tables;

        public readonly IDictionary<long, Team>                          TeamsByNumber = new Dictionary<long, Team>();
        public readonly IDictionary<FMSTeamId, Team>                     TeamsById = new Dictionary<FMSTeamId, Team>();
        public readonly List<Team>                                       Teams = new List<Team>();
        public readonly IDictionary<long, ScheduledMatch>                ScheduledMatchesByNumber = new Dictionary<long, ScheduledMatch>();
        public readonly IDictionary<FMSScheduleDetailId, ScheduledMatch> ScheduledMatchesById = new Dictionary<FMSScheduleDetailId, ScheduledMatch>();
        public readonly IDictionary<long, List<PlayedMatch>>             PlayedMatchesByNumber = new Dictionary<long, List<PlayedMatch>>();
        public readonly IDictionary<Tuple<string,long>, LeagueHistoryMatch> LeagueHistoryMatchesByEventAndMatchNumber = new Dictionary<Tuple<string,long>, LeagueHistoryMatch>(); // key: event code, match number
        public readonly IDictionary<string, Event>                       EventsByCode = new Dictionary<string, Event>();
        public readonly List<EqualizationMatch>                          EqualizationMatches = new List<EqualizationMatch>();

        public int MaxAveragingMatchCount
            {
            get {
                int result = 0;
                foreach (Team team in Teams)
                    {
                    result = Math.Max(result, team.AveragingMatchCount);
                    }
                return Math.Min(result, programOptions.AveragingMatchCountCap);
                }
            }

        public ProgramOptions ProgramOptions => programOptions;
        public string         EqualizationMatchCreatorName => "FTC Equalize Match Counts";
        private DateTime      endOfTournament;
        private TimeSpan      endOfTournamentDuration;
        public string         ThisEventCode => Tables.Config.Map["code"].Value.NonNullValue;
        public Event          ThisEvent => EventsByCode[ThisEventCode];
        public FMSEventId     FMSEventId => TableColumn.Create<FMSEventId>(Tables.Config.Map["FMSEventId"].Value.NonNullValue);
        public DateTime       Start => TableColumn.Create<DateTimeAsInteger>(long.Parse(Tables.Config.Map["start"].Value.NonNullValue)).DateTimeNonNull;
        public DateTime       End => TableColumn.Create<DateTimeAsInteger>(long.Parse(Tables.Config.Map["end"].Value.NonNullValue)).DateTimeNonNull;

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


        private ProgramOptions programOptions;
        private string fileName;
        private bool disposed = false;

        //---------------------------------------------------------------------------------------------------
        // Construction
        //---------------------------------------------------------------------------------------------------

        public Database(ProgramOptions programOptions)
            {
            this.programOptions = programOptions;
            fileName = Path.GetFullPath(programOptions.Filename);
            Tables = new Tables(this);
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
            // References, for fun and profit:
            //  https://www.sqlite.org/atomiccommit.html
            //  https://www.sqlite.org/lang_transaction.html
            // 
            Trace.Assert(IsTransactionInProgress);
            //
            // We *could* just call Transaction.Commit(), but, bizarrely, that provide no notification as to the transaction success.
            // So, we roll our own
            //
            bool aborted = false;
            bool commitAttempted = false;
            // https://www.sqlite.org/c3ref/commit_hook.html
            SQLitePCL.raw.sqlite3_commit_hook(Connection.Handle, (object data) =>
            {
                commitAttempted = true;
                return 0;
            }, null);
            SQLitePCL.raw.sqlite3_rollback_hook(Connection.Handle, delegate(object data)
                {
                aborted = true;
                }, null);
            Connection.ExecuteNonQuery("COMMIT;");
            int rc = SQLitePCL.raw.sqlite3_errcode(Connection.Handle); // retrieve error code, if any: https://www.sqlite.org/c3ref/errcode.html
            Trace.Assert(commitAttempted);

            // Clean up at end. Surprisingly difficult, as we need a new transaction to replace the one
            // that we completed w/o the Transaction object knowing about it
            SQLitePCL.raw.sqlite3_commit_hook(Connection.Handle, null, null); // paranoia
            SQLitePCL.raw.sqlite3_rollback_hook(Connection.Handle, null, null); // paranoia
            Connection.ExecuteNonQuery("BEGIN;");
            Transaction.Rollback();
            Transaction = null;
            Trace.Assert(!IsTransactionInProgress);

            if (aborted)
                {
                // https://www.sqlite.org/lang_transaction.html
                throw new CommitFailedException(this, rc);
                }
            }

        public void AbortTransaction()
            {
            if (IsTransactionInProgress)
                {
                Transaction.Rollback();
                Transaction = null;
                Trace.Assert(!IsTransactionInProgress);
                }
            }

        public void Close()
            {
            if (Connection != null)
                {
                AbortTransaction();
                var handle = Connection.Handle;
                Connection.Close();
                Connection = null;

                /* Suppress finalization to avoid spurious second exception
                    Unhandled Exception: System.ObjectDisposedException: Safe handle has been closed
                       at System.Runtime.InteropServices.SafeHandle.DangerousAddRef(Boolean& success)
                       at System.StubHelpers.StubHelpers.SafeHandleAddRef(SafeHandle pHandle, Boolean& success)
                       at SQLitePCL.SQLite3Provider_dynamic_cdecl.SQLitePCL.ISQLite3Provider.sqlite3_rollback_hook(sqlite3 db, delegate_rollback func, Object v)
                       at Microsoft.Data.Sqlite.SqliteTransaction.RollbackExternal(Object userData)
                       at SQLitePCL.rollback_hook_info.call()
                       at SQLitePCL.SQLite3Provider_dynamic_cdecl.rollback_hook_bridge_impl(IntPtr p)
                       at SQLitePCL.SQLite3Provider_dynamic_cdecl.SQLitePCL.ISQLite3Provider.sqlite3_close_v2(IntPtr db)
                       at SQLitePCL.sqlite3.ReleaseHandle()
                       at System.Runtime.InteropServices.SafeHandle.InternalFinalize()
                       at System.Runtime.InteropServices.SafeHandle.Dispose(Boolean disposing)
                       at System.Runtime.InteropServices.SafeHandle.Finalize()
                 */
                if (handle != null)
                    {
                    GC.SuppressFinalize(handle);
                    }
                }
            }

        //---------------------------------------------------------------------------------------------------
        // Querying
        //---------------------------------------------------------------------------------------------------

        public void Load()
            {
            try { 
                Tables.Clear();
                Tables.Load();
                }
            catch (Exception e)
                {
                throw new CantLoadDatabaseException(programOptions, e);
                }

            ClearDataAccessLayer();
            LoadDataAccessLayer();
            }

        public void ValidateReadyForEqualization()
            {
            if (Tables.Quals.Rows.Count == 0)
                {
                throw new MatchScheduleNotCreatedException();
                }
            }

        void ClearDataAccessLayer()
            {
            TeamsById.Clear();
            TeamsByNumber.Clear();
            Teams.Clear();
            ScheduledMatchesByNumber.Clear();
            ScheduledMatchesById.Clear();
            PlayedMatchesByNumber.Clear();
            LeagueHistoryMatchesByEventAndMatchNumber.Clear();
            EventsByCode.Clear();

            EqualizationMatches.Clear();
            }

        public void LoadDataAccessLayer()
            {
            foreach (var row in Tables.LeagueMeets.Rows)
                {
                Event anEvent = new Event(this, row, Event.TEvent.LEAGUE_MEET, Event.TStatus.ARCHIVED);
                if (row.EventCode.NonNullValue == ThisEventCode)
                    {
                    anEvent.Type = EnumUtil.From<Event.TEvent>(int.Parse(Tables.Config.Map["type"].Value.NonNullValue));
                    anEvent.Status = EnumUtil.From<Event.TStatus>(int.Parse((Tables.Config.Map["status"].Value.NonNullValue)));
                    }
                EventsByCode[anEvent.EventCode] = anEvent;
                }

            foreach (var row in Tables.Team.Rows)
                {
                Team team = new Team(this, row);
                TeamsByNumber[team.TeamNumber] = team;
                TeamsById[team.FMSTeamId] = team;
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

            LeagueHistoryMatch.DetermineLeagueMatchesThatCount(this, programOptions.LeagueMatchesToConsider);
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

            writer.WriteLine();
            }

        public int ReportTeamsAndPlanMatches(IndentedTextWriter writer, bool verbose)
            {
            int averagingMatchCountGoal = ProgramOptions.AveragingMatchCountGoal ?? MaxAveragingMatchCount;

            writer.WriteLine($"Teams: averaging match count goal: {averagingMatchCountGoal}");
            writer.Indent++;

            int teamsReported = 0;
            int totalAveragingMatchesNeeded = 0;
            foreach (Team team in Teams)
                {
                int teamAveragingMatchesNeeded = averagingMatchCountGoal - team.AveragingMatchCount;
                totalAveragingMatchesNeeded += teamAveragingMatchesNeeded;

                // Report
                if (verbose || teamAveragingMatchesNeeded > 0)
                    { 
                    if (verbose)
                        {
                        writer.WriteLine();
                        }
                    team.Report(writer, verbose, averagingMatchCountGoal);
                    teamsReported += 1;
                    }
                }

            PlanMatches();

            if (teamsReported == 0)
                {
                writer.WriteLine();
                writer.WriteLine("All teams up to date");
                }
            else
                {
                writer.WriteLine("----------------");
                writer.WriteLine($"Total: needed {totalAveragingMatchesNeeded} averaging matches can be accomplished in {EqualizationMatches.Count} equalization matches");
                }

            writer.Indent--;
            return EqualizationMatches.Count;
            }

        // Equalization matches cannot be ties, lest that biases the scoring results. Hence,
        // we decree that matches shall be scored (manually, using ScoreKeeper) as a win
        // for Blue. Thus all blue participants in equalization matches need to be surrogates.
        protected void PlanMatches()
            {
            int averagingMatchCountGoal = ProgramOptions.AveragingMatchCountGoal ?? MaxAveragingMatchCount;

            ISet<Team> completedTeams = new HashSet<Team>();
            IDictionary<Team, int> matchesNeededByTeam = new ConcurrentDictionary<Team, int>();
            foreach (Team team in Teams)
                {
                int teamAveragingMatchesNeeded = averagingMatchCountGoal - team.AveragingMatchCount;
                if (teamAveragingMatchesNeeded == 0)
                    {
                    completedTeams.Add(team);
                    }
                else
                    {
                    matchesNeededByTeam[team] = teamAveragingMatchesNeeded;
                    }
                }

            List<Team> rotating = new List<Team>(completedTeams);
            EqualizationMatches.Clear();
            endOfTournament = End + TimeSpan.FromDays(2); // 1. haven't run eliminations yet 2. End is only day-granular
            endOfTournamentDuration = TimeSpan.FromMinutes(10);

            DateTime startTime = endOfTournament + endOfTournamentDuration + TimeSpan.FromMinutes(10);
            TimeSpan duration = TimeSpan.FromSeconds(5);
            TimeSpan interval = TimeSpan.FromSeconds(7); // arbitrary, but close enough that re-runs of this tool will still likely be later
            while (matchesNeededByTeam.Count > 0)
                {
                // Take at most two teams for the red side
                var teams = new List<Team>(matchesNeededByTeam.Keys.Take(2));
                var remainingIncomplete = new List<Team>(matchesNeededByTeam.Keys.Skip(2));

                // Round out to 4 with teams that will be surrogates. We can choose arbitrary teams,
                // but we need to be careful in events that have a very small team count. So, we start
                // with the remaining incomplete teams then move on to those who have completed. The 
                // later we rotate, mostly just for fun.
                int numSurrogatesNeeded = 4 - teams.Count;
                var surrogates = new List<Team>(remainingIncomplete.Take(numSurrogatesNeeded));
                var rotatingSurrogates = new List<Team>(rotating.Take(numSurrogatesNeeded - surrogates.Count));
                
                surrogates.AddRange(rotatingSurrogates);
                rotating.RemoveRange(0, rotatingSurrogates.Count);
                rotating.AddRange(rotatingSurrogates);

                teams.AddRange(surrogates);
                var isSurrogates = new List<bool>(teams.Select(team => surrogates.Contains(team)));

                EqualizationMatch equalizationMatch = new EqualizationMatch(this, teams, isSurrogates, startTime, duration);
                EqualizationMatches.Add(equalizationMatch);
                startTime = startTime + interval;

                foreach (Team team in teams)
                    {
                    if (!surrogates.Contains(team))
                        {
                        matchesNeededByTeam[team] = matchesNeededByTeam[team] - 1;
                        if (matchesNeededByTeam[team] == 0)
                            {
                            matchesNeededByTeam.Remove(team);
                            completedTeams.Add(team);
                            rotating.Add(team);
                            }
                        }
                    }
                }
            }

        public int SaveEqualizationMatches(IndentedTextWriter writer, bool verbose)
            {
            if (EqualizationMatches.Count > 0)
                {
                EqualizationMatch.SaveEndOfTournamentBlock(this, endOfTournament, endOfTournamentDuration);

                foreach (var equalizationMatch in EqualizationMatches)
                    {
                    equalizationMatch.SaveToDatabase();
                    }
                EqualizationMatch.SaveEqualizationMatchesBlock(this, EqualizationMatches.First().ScheduleStart, EqualizationMatches.Count);
                }

            int result = EqualizationMatches.Count;
            EqualizationMatches.Clear();
            return result;
            }

        public bool BackupFile()
            {
            bool result = true;
            string directoryName = Path.GetDirectoryName(fileName);
            string root = Path.GetFileNameWithoutExtension(fileName);
            string ext = Path.GetExtension(fileName);
            string path;
            int iteration = 1;
            for (;;)
                {
                string file = root + " - Backup";
                if (iteration > 1)
                    {
                    file += $" ({iteration})";
                    }
                path = Path.Combine(directoryName, file + ext);
                if (!File.Exists(path))
                    break;
                iteration ++;
                }

            try {
                using FileStream output = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None);
                using FileStream input = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                input.CopyTo(output);
                }
            catch (Exception e)
                {
                programOptions.StdErr.WriteLine($"Exception creating backup: {e.Message}");
                result = false;
                }
            return result;
            }
        }
    }
