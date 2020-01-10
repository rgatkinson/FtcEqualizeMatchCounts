using FEMC.Enums;
using System;
using System.Collections.Generic;
using FEMC.DAL.Support;

namespace FEMC.DAL
    {
    class EqualizationMatch : ScheduledMatch
        {
        //----------------------------------------------------------------------------------------
        // State
        //----------------------------------------------------------------------------------------

        protected TimeSpan? duration = null;
        public TimeSpan Duration => duration ?? throw new NotImplementedException("EqualizationMatch.Duration");

        public bool IsScored
            {
            get
                {
                // TODO: is there a better FMSEventId-based way to do this?
                return Database.Tables.QualsGameSpecific.Rows.Exists(row => Equals(row.MatchNumber.NonNullValue, MatchNumber))  // one of possibly many things we could test
                    || Database.Tables.ElimsGameSpecific.Rows.Exists(row => Equals(row.MatchNumber.NonNullValue, MatchNumber));
                }
            }


        //----------------------------------------------------------------------------------------
        // Construction
        //----------------------------------------------------------------------------------------

        public EqualizationMatch(Database db, List<Team> teams, List<bool> isSurrogates, DateTimeOffset startTime, TimeSpan duration) : base(db, db.ThisFMSEventId, NewFMSScheduleDetailId())
            {
            matchNumber = db.NewEqualizationMatches.Count == 0 ? Math.Max(db.FirstEqualizationMatchNumber, Event.LastMatchNumber + 1) : Event.LastMatchNumber + 1;
            Description = $"Equalization {matchNumber}";
            CreatedBy = db.EqualizationMatchCreatorName;
            SetMatchType(TMatchType.QUALS);
            fieldType = (int)TFieldType.Usual;
            ScheduleStart = startTime;

            this.duration = duration;
            fmsMatchIdGuid = Guid.NewGuid();        // we are the source of the id (apparently?)

            Red1 = teams[0];
            Red2 = teams[1];
            Blue1 = teams[2];
            Blue2 = teams[3];

            Red1Surrogate = isSurrogates[0];
            Red2Surrogate = isSurrogates[1];
            Blue1Surrogate = isSurrogates[2];
            Blue2Surrogate = isSurrogates[3];

            AddToDatabase();
            }

        public EqualizationMatch(Database db, DBTables.ScheduleDetail.Row row) : base(db, row)
            {
            }

        //----------------------------------------------------------------------------------------
        // Playing
        //----------------------------------------------------------------------------------------

        // Play this match with the required Win for Blue
        protected MatchPlayedThisEvent PlayMatch()
            {
            MatchPlayedThisEvent m = new MatchPlayedThisEvent(Database, FMSScheduleDetailId);
            // match.MatchNumber = MatchNumber; // not needed: match number comes via FMSScheduleDetailId
            m.FmsMatchId.Value = FMSMatchIdGuid;
            m.PlayNumber = 1; // odd, but that's what SQLiteMachDAO.commitMatch() does
            m.FieldType = 1; // ditto
            m.RedScores.SetRedEqualizationMatch();
            m.BlueScores.SetBlueEqualizationMatch();
            m.ScoreDetails = m.EncodeScoreDetails();
            m.RedScore = m.RedScores.ScoredPoints;
            m.RedPenalty = m.RedScores.PenaltyPoints;
            m.BlueScore = m.BlueScores.ScoredPoints;
            m.BluePenalty = m.BlueScores.PenaltyPoints;
            m.RedAutoScore = m.RedScores.AutonomousPoints;
            m.BlueAutoScore = m.BlueScores.AutonomousPoints;

            m.CreatedBy = CreatedBy;
            m.StartTime = DateTimeOffset.UtcNow;

            return m;
            }

        //----------------------------------------------------------------------------------------
        // Saving
        //----------------------------------------------------------------------------------------

        public static void DeleteEndOfTournamentBlock(Database db, DateTimeOffset tournamentEndBlockStart, TimeSpan tournamentEndBlockDuration)
            {
            var matchScheduleRow = db.Tables.MatchSchedule.NewRow();
            matchScheduleRow.Delete(matchScheduleRow.Where(new List<(string, SqlOperator, object)>
                {
                (nameof(matchScheduleRow.Start), SqlOperator.GE, tournamentEndBlockStart)
                }));

            var blocksRow = db.Tables.Blocks.NewRow();
            blocksRow.Delete(blocksRow.Where(new List<(string, SqlOperator, object)>
                {
                (nameof(blocksRow.Start), SqlOperator.GE, tournamentEndBlockStart)
                }));
            }

        public static void SaveEndOfTournamentBlock(Database db, DateTimeOffset tournamentEndBlockStart, TimeSpan tournamentEndBlockDuration)
            {
            var blocksRow = db.Tables.Blocks.NewRow();
            var matchScheduleRow = db.Tables.MatchSchedule.NewRow();

            blocksRow.Start.Value = tournamentEndBlockStart;
            blocksRow.Type.Value = (int)TMatchScheduleType.AdminBreak;
            blocksRow.Duration.Value = (long)Math.Round(tournamentEndBlockDuration.TotalMinutes);
            blocksRow.Count.Value = 0;
            blocksRow.Label.Value = "End of Tournament - Administrative Pseudo-matches Follow";

            matchScheduleRow.Start.Value = blocksRow.Start.Value;
            matchScheduleRow.End.Value = matchScheduleRow.Start.Value + TimeSpan.FromMinutes(blocksRow.Duration.Value.Value);
            matchScheduleRow.Type.Value = blocksRow.Type.Value;
            matchScheduleRow.Label.Value = blocksRow.Label.Value;

            blocksRow.Insert();
            matchScheduleRow.Insert();
            }

        public static void SaveEqualizationMatchesBlock(Database db, DateTimeOffset start, int count)
            {
            var blocksRow = db.Tables.Blocks.NewRow();

            blocksRow.Start.Value = start;
            blocksRow.Type.Value = (int)TMatchScheduleType.Match;
            blocksRow.Duration.Value = 0;
            blocksRow.Count.Value = count;
            blocksRow.Label.Value = null;

            blocksRow.Insert();
            }

        public void SaveToDatabase()
            {
            var scheduledMatchRow = Database.Tables.ScheduleDetail.NewRow();
            var qualsRow = Database.Tables.Quals.NewRow();
            var qualsDataRow = Database.Tables.QualsData.NewRow();
            var matchScheduleRow = Database.Tables.MatchSchedule.NewRow();

            scheduledMatchRow.FMSScheduleDetailId = FMSScheduleDetailId;
            scheduledMatchRow.FMSEventId = FMSEventId;
            scheduledMatchRow.TournamentLevel.Value = tournamentLevel;
            scheduledMatchRow.MatchNumber.Value = MatchNumber;
            scheduledMatchRow.FieldType.Value = fieldType;
            scheduledMatchRow.Description.Value = Description;
            scheduledMatchRow.StartTime.Value = ScheduleStart;
            scheduledMatchRow.FieldConfigurationDetails.Value = null;
            scheduledMatchRow.CreatedOn.Value = null;
            scheduledMatchRow.CreatedBy.Value = CreatedBy;
            scheduledMatchRow.ModifiedOn.Value = null;
            scheduledMatchRow.ModifiedBy.Value = null;
            scheduledMatchRow.RowVersion.Value = Guid.Empty.ToByteArray(); // ScheduleDetail seems to use 16 byte all-zero RowVersions; 'don't know why

            qualsRow.MatchNumber.Value = MatchNumber;
            qualsRow.Red1.Value = Red1.TeamNumber;
            qualsRow.Red2.Value = Red2.TeamNumber;
            qualsRow.Blue1.Value = Blue1.TeamNumber;
            qualsRow.Blue2.Value = Blue2.TeamNumber;
            qualsRow.Red1Surrogate.Value = Red1Surrogate;
            qualsRow.Red2Surrogate.Value = Red2Surrogate;
            qualsRow.Blue1Surrogate.Value = Blue1Surrogate;
            qualsRow.Blue2Surrogate.Value = Blue2Surrogate;

            qualsDataRow.MatchNumber.Value = MatchNumber;
            qualsDataRow.Status.Value = (int)Enums.TMatchState.Unplayed;
            qualsDataRow.Randomization.Value = (int)TRandomization.DefaultValue;
            qualsDataRow.Start.Value = DateTimeAsInteger.NegativeOne;
            qualsDataRow.ScheduleStart.Value = ScheduleStart;
            qualsDataRow.PostedTime.Value = DateTimeAsInteger.NegativeOne;
            qualsDataRow.FMSMatchId.Value = FMSMatchIdGuid;
            qualsDataRow.FMSScheduleDetailId = FMSScheduleDetailIdAsString.CreateFrom(FMSScheduleDetailId);

            matchScheduleRow.Start.Value = ScheduleStart;
            matchScheduleRow.End.Value = ScheduleStart + Duration;
            matchScheduleRow.Type.Value = (int)TMatchScheduleType.Match;
            matchScheduleRow.Label.Value = Description;

            scheduledMatchRow.Insert();
            qualsRow.Insert();
            qualsDataRow.Insert();
            matchScheduleRow.Insert();

            foreach (var alliance in EnumUtil.GetValues<TAlliance>())
                {
                foreach (var station in EnumUtil.GetValues<TStation>())
                    {
                    DBTables.ScheduleStation.Row row = Database.Tables.ScheduleStation.NewRow();

                    row.FMSScheduleDetailId = FMSScheduleDetailId;
                    row.Alliance.Value = (int)alliance;
                    row.Station.Value = (int)station;

                    row.FmsEventId = FMSEventId;
                    row.FmsTeamId = GetTeam(alliance, station).FMSTeamId;
                    row.IsSurrogate.Value = GetSurrogate(alliance, station);

                    row.CreatedOn = scheduledMatchRow.CreatedOn;
                    row.CreatedBy = scheduledMatchRow.CreatedBy;
                    row.ModifedOn = scheduledMatchRow.ModifiedOn;
                    row.ModifiedBy = scheduledMatchRow.ModifiedBy;

                    row.Insert();
                    }
                }
            }

        public void ScoreMatch()
            {
            MatchPlayedThisEvent m = PlayMatch();   // See SQLiteMatchDAO.java.commitMatch()
            m.SaveNonCommitMatchHistory(TCommitType.EDIT_SAVED); // mirror what we see ScoreKeeper do
            m.CommitMatch();
            }
        }
    }
