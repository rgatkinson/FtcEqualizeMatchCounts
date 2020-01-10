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
using FEMC.Enums;

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
        public readonly IDictionary<long, List<MatchPlayedThisEvent>>    PlayedMatchesByNumber = new Dictionary<long, List<MatchPlayedThisEvent>>();
        public readonly IDictionary<string, Event>                       EventsByCode = new Dictionary<string, Event>();
        public readonly List<EqualizationMatch>                          NewEqualizationMatches = new List<EqualizationMatch>();
        public readonly List<EqualizationMatch>                          LoadedEqualizationMatches = new List<EqualizationMatch>();
        public readonly LeagueSubsystem                                  LeagueSubsystem;
        public List<EqualizationMatch> UnscoredEqualizationMatches => new List<EqualizationMatch>((NewEqualizationMatches.FindAll(match => !match.IsScored)).Concat(LoadedEqualizationMatches.FindAll(match => !match.IsScored)));
        public List<EqualizationMatch> UnscoredLoadedEqualizationMatches => new List<EqualizationMatch>(LoadedEqualizationMatches.FindAll(match => !match.IsScored));

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
        public int            FirstEqualizationMatchNumber => 1000;
        private DateTimeOffset tournamentEndBlockStart;
        private TimeSpan      tournamentEndBlockDuration;
        public string         ThisEventCode => Tables.Config.Map["code"].Value.NonNullValue;
        public ThisEvent      ThisEvent => (ThisEvent)EventsByCode[ThisEventCode];
        public FMSEventId     ThisFMSEventId => TableColumn.CreateFromDatabaseValue<FMSEventId>(Tables.Config.Map["FMSEventId"].Value.NonNullValue);
        public int            ThisEventMatchesPerTeam => int.Parse(Tables.Config.Map["matchesPerTeam"].Value.NonNullValue);
        public DateTimeOffset TournamentNominalStart => TableColumn.CreateFromDatabaseValue<DateTimeAsInteger>(long.Parse(Tables.Config.Map["start"].Value.NonNullValue)).DateTimeOffsetNonNull;
        public DateTimeOffset TournamentNominalEnd => TableColumn.CreateFromDatabaseValue<DateTimeAsInteger>(long.Parse(Tables.Config.Map["end"].Value.NonNullValue)).DateTimeOffsetNonNull;

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
            LeagueSubsystem = new LeagueSubsystem(this);
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

            var cmd = Connection.CreateCommand();
            cmd.CommandText = "PRAGMA foreign_keys = ON;";
            cmd.ExecuteNonQuery();
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

        public void ClearAndLoad()
            {
            Tables.Clear();
            ClearDataAccessLayer();

            try { 
                Tables.Load();
                }
            catch (Exception e)
                {
                throw new CantLoadDatabaseException(programOptions, e);
                }
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
            LeagueSubsystem.Clear();
            EventsByCode.Clear();

            NewEqualizationMatches.Clear();
            LoadedEqualizationMatches.Clear();
            }

        public void LoadDataAccessLayer()
            {
            foreach (var row in Tables.LeagueMeets.Rows)
                {
                if (row.EventCode.NonNullValue == ThisEventCode)
                    {
                    ThisEvent thisEvent = new ThisEvent(this, row,
                        EnumUtil.From<TEventType>(int.Parse(Tables.Config.Map["type"].Value.NonNullValue)),
                        EnumUtil.From<TEventStatus>(int.Parse((Tables.Config.Map["status"].Value.NonNullValue)))
                        );
                    EventsByCode[thisEvent.EventCode] = thisEvent;
                    }
                else
                    {
                    HistoricalLeagueMeet anEvent = new HistoricalLeagueMeet(this, row, TEventType.LEAGUE_MEET, TEventStatus.ARCHIVED);
                    EventsByCode[anEvent.EventCode] = anEvent;
                    }
                }

            foreach (var row in Tables.Team.Rows)
                {
                Team team = new Team(this, row);
                TeamsByNumber[team.TeamNumber] = team;
                TeamsById[team.FMSTeamId] = team;
                Teams.Add(team);
                }
            Teams.Sort((a, b) => a.TeamNumber - b.TeamNumber);

            foreach (var row in Tables.ScheduleDetail.Rows)
                {
                if (row.IsEqualizationMatch(this))
                    {
                    EqualizationMatch equalizationMatch = new EqualizationMatch(this, row);
                    Trace.Assert(equalizationMatch.IsEqualizationMatch);
                    LoadedEqualizationMatches.Add(equalizationMatch);
                    }
                else
                    {
                    ScheduledMatch scheduledMatch = new ScheduledMatch(this, row);
                    Trace.Assert(!scheduledMatch.IsEqualizationMatch);
                    }
                }

            // fmsMatch
            foreach (var row in Tables.Match.Rows)
                {
                MatchPlayedThisEvent matchPlayed = new MatchPlayedThisEvent(this, row);
                if (!PlayedMatchesByNumber.TryGetValue(matchPlayed.MatchNumber, out List<MatchPlayedThisEvent> playedMatches))
                    {
                    playedMatches = new List<MatchPlayedThisEvent>();
                    PlayedMatchesByNumber[matchPlayed.MatchNumber] = playedMatches;
                    }
                playedMatches.Add(matchPlayed);
                }
            foreach (var matches in PlayedMatchesByNumber.Values)
                {
                matches.Sort((m1, m2) => (int)(m2.PlayNumber - m1.PlayNumber)); // descending by play number
                }

            // psData
            foreach (var row in Tables.QualsData.Rows.Concat(Tables.ElimsData.Rows))
                {
                if (PlayedMatchesByNumber.TryGetValue(row.MatchNumber.NonNullValue, out List<MatchPlayedThisEvent> playedMatches))
                    {
                    foreach (MatchPlayedThisEvent match in playedMatches)
                        {
                        match.Load(row);
                        }
                    }
                }

            // psScores
            foreach (var row in Tables.QualsScores.Rows)
                {
                if (PlayedMatchesByNumber.TryGetValue(row.MatchNumber.NonNullValue, out List<MatchPlayedThisEvent> playedMatches))
                    {
                    foreach (MatchPlayedThisEvent match in playedMatches)
                        {
                        match.Load(row);
                        }
                    }
                }
            foreach (var row in Tables.ElimsScores.Rows)
                {
                if (PlayedMatchesByNumber.TryGetValue(row.MatchNumber.NonNullValue, out List<MatchPlayedThisEvent> playedMatches))
                    {
                    foreach (MatchPlayedThisEvent match in playedMatches)
                        {
                        match.Load(row);
                        }
                    }
                }

            // psGame
            foreach (var row in Tables.QualsGameSpecific.Rows.Concat(Tables.ElimsGameSpecific.Rows))
                {
                if (PlayedMatchesByNumber.TryGetValue(row.MatchNumber.NonNullValue, out List<MatchPlayedThisEvent> playedMatches))
                    {
                    foreach (MatchPlayedThisEvent match in playedMatches)
                        {
                        match.Load(row);
                        }
                    }
                }

            // psResult
            foreach (var row in Tables.QualsResults.Rows.Concat(Tables.ElimsResults.Rows))
                {
                if (PlayedMatchesByNumber.TryGetValue(row.MatchNumber.NonNullValue, out List<MatchPlayedThisEvent> playedMatches))
                    {
                    foreach (MatchPlayedThisEvent match in playedMatches)
                        {
                        match.Load(row);
                        }
                    }
                }

            // psHistory
            foreach (var row in Tables.QualsCommitHistory.Rows.Concat(Tables.ElimsCommitHistory.Rows))
                {
                if (PlayedMatchesByNumber.TryGetValue(row.MatchNumber.NonNullValue, out List<MatchPlayedThisEvent> playedMatches))
                    {
                    foreach (MatchPlayedThisEvent match in playedMatches)
                        {
                        match.Load(row);
                        }
                    }
                }

            // psScoresHistory
            foreach (var row in Tables.QualsScoresHistory.Rows)
                {
                if (PlayedMatchesByNumber.TryGetValue(row.MatchNumber.NonNullValue, out List<MatchPlayedThisEvent> playedMatches))
                    {
                    foreach (MatchPlayedThisEvent match in playedMatches)
                        {
                        match.Load(row);
                        }
                    }
                }
            foreach (var row in Tables.ElimsScoresHistory.Rows)
                {
                if (PlayedMatchesByNumber.TryGetValue(row.MatchNumber.NonNullValue, out List<MatchPlayedThisEvent> playedMatches))
                    {
                    foreach (MatchPlayedThisEvent match in playedMatches)
                        {
                        match.Load(row);
                        }
                    }
                }

            // psGameHistory
            foreach (var row in Tables.QualsGameSpecificHistory.Rows.Concat(Tables.ElimsGameSpecificHistory.Rows))
                {
                if (PlayedMatchesByNumber.TryGetValue(row.MatchNumber.NonNullValue, out List<MatchPlayedThisEvent> playedMatches))
                    {
                    foreach (MatchPlayedThisEvent match in playedMatches)
                        {
                        match.Load(row);
                        }
                    }
                }

            LeagueSubsystem.Load();
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

        public int ReportTeamsAndPlanMatches(IndentedTextWriter writer, bool verbose, bool afterUpdates)
            {
            int averagingMatchCountGoal = ProgramOptions.AveragingMatchCountGoal ?? MaxAveragingMatchCount;

            if (afterUpdates) writer.Indent++;
            writer.WriteLine($"Teams: averaging match count goal: {averagingMatchCountGoal}");
            if (!afterUpdates) writer.Indent++;

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

            PlanEqualizationMatches();

            List<EqualizationMatch> unscoredLoadedEqualizationMatches = UnscoredLoadedEqualizationMatches;
            string existing = afterUpdates ? "" : "existing ";

            if (teamsReported == 0)
                {
                if (!afterUpdates) writer.WriteLine();
                writer.WriteLine("Averaging match count for all teams is up to date.");
                }
            else
                {
                writer.WriteLine("----------------");
                writer.WriteLine($"Total: the needed {totalAveragingMatchesNeeded} team averaging matches can be accomplished in {NewEqualizationMatches.Count} new equalization matches");
                }

            if (LoadedEqualizationMatches.Count > 0)
                {
                writer.WriteLine($"{LoadedEqualizationMatches.Count} {existing}equalization matches exist, of which {unscoredLoadedEqualizationMatches.Count} remain to be scored.");
                }

            writer.Indent--;
            return NewEqualizationMatches.Count;
            }

        // Equalization matches cannot be ties, lest that biases the scoring results. Hence,
        // we decree that matches shall be scored (manually, using ScoreKeeper) as a win
        // for Blue. Thus all blue participants in equalization matches need to be surrogates.
        protected void PlanEqualizationMatches()
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
            NewEqualizationMatches.Clear();
            tournamentEndBlockStart = TournamentNominalEnd + TimeSpan.FromDays(2); // two days: 1. haven't run eliminations yet 2. End is only day-granular
            tournamentEndBlockDuration = TimeSpan.FromMinutes(10);

            DateTimeOffset equalizationMatchStart = tournamentEndBlockStart + tournamentEndBlockDuration + TimeSpan.FromMinutes(10);
            TimeSpan equalizationMatchDuration = TimeSpan.FromSeconds(5);
            TimeSpan equalizationMatchInterval = TimeSpan.FromSeconds(7); // arbitrary, but close enough that re-runs of this tool will still likely be later
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

                EqualizationMatch equalizationMatch = new EqualizationMatch(this, teams, isSurrogates, equalizationMatchStart, equalizationMatchDuration);
                NewEqualizationMatches.Add(equalizationMatch);
                equalizationMatchStart = equalizationMatchStart + equalizationMatchInterval;

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

        public void ClearNewEqualizationMatches()
            {
            NewEqualizationMatches.Clear();
            }

        public void SaveNewEqualizationMatches(IndentedTextWriter writer, bool verbose)
            {
            if (NewEqualizationMatches.Count > 0)
                {
                EqualizationMatch.SaveEndOfTournamentBlock(this, tournamentEndBlockStart, tournamentEndBlockDuration);

                foreach (var equalizationMatch in NewEqualizationMatches)
                    {
                    equalizationMatch.SaveToDatabase();
                    }
                EqualizationMatch.SaveEqualizationMatchesBlock(this, NewEqualizationMatches.First().ScheduleStart.LocalDateTime, NewEqualizationMatches.Count);
                }
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
