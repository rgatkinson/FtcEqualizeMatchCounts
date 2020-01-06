using FEMC.Enums;
using System;
using System.Collections.Generic;

namespace FEMC.DAL
    {
    class EqualizationMatch : ScheduledMatch
        {
        //----------------------------------------------------------------------------------------
        // Construction
        //----------------------------------------------------------------------------------------

        private const long FirstEqualizationMatchNumber = 1000; // assume will be bigger than any real match number
        
        public EqualizationMatch(Database db, List<Team> teams, List<bool> isSurrogates, DateTime startTime, TimeSpan duration) : base(db, db.ThisFMSEventId, NewFMSScheduleDetailId())
            {
            CreatedBy = db.EqualizationMatchCreatorName;
            matchNumber = db.EqualizationMatches.Count==0 ? Math.Max(FirstEqualizationMatchNumber, Event.LastMatchNumber + 1) : Event.LastMatchNumber + 1;
            Description = $"Equalization {matchNumber}";
            tournamentLevel = (int)TTournamentLevel.Qualification;
            fieldType = (int)TFieldType.Usual;
            ScheduleStart = startTime;
            this.duration = duration;
            fmsMatchId = Guid.NewGuid();        // we are the source of the id (apparently?)

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

        //----------------------------------------------------------------------------------------
        // Playing
        //----------------------------------------------------------------------------------------

        // Play this match with the required Win for Blue
        public PlayedMatch PlayMatch()
            {
            PlayedMatch match = new PlayedMatch(Database, FMSScheduleDetailId);
            match.Start = DateTime.UtcNow;
            //
            // More to come
            //
            return match;
            }

        //----------------------------------------------------------------------------------------
        // Saving
        //----------------------------------------------------------------------------------------

        public static void SaveEndOfTournamentBlock(Database db, DateTime start, TimeSpan duration)
            {
            var blocksRow = db.Tables.Blocks.NewRow();
            var matchScheduleRow = db.Tables.MatchSchedule.NewRow();

            blocksRow.Start.Value = start;
            blocksRow.Type.Value = (int)TMatchScheduleType.AdminBreak;
            blocksRow.Duration.Value = (long)Math.Round(duration.TotalMinutes);
            blocksRow.Count.Value = 0;
            blocksRow.Label.Value = "End of Tournament - Administrative Pseudo-matches Follow";

            matchScheduleRow.Start.Value = blocksRow.Start.Value;
            matchScheduleRow.End.Value = matchScheduleRow.Start.Value + TimeSpan.FromMinutes(blocksRow.Duration.Value.Value);
            matchScheduleRow.Type.Value = blocksRow.Type.Value;
            matchScheduleRow.Label.Value = blocksRow.Label.Value;

            blocksRow.AddToTableAndSave();
            matchScheduleRow.AddToTableAndSave();
            }

        public static void SaveEqualizationMatchesBlock(Database db, DateTime start, int count)
            {
            var blocksRow = db.Tables.Blocks.NewRow();

            blocksRow.Start.Value = start;
            blocksRow.Type.Value = (int)TMatchScheduleType.Match;
            blocksRow.Duration.Value = 0;
            blocksRow.Count.Value = count;
            blocksRow.Label.Value = null;

            blocksRow.AddToTableAndSave();
            }

        public void SaveToDatabase(bool scoreMatch)
            {
            var scheduledMatchRow = Database.Tables.ScheduledMatch.NewRow();
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

            qualsRow.Match.Value = MatchNumber;
            qualsRow.Red1.Value = Red1.TeamNumber;
            qualsRow.Red2.Value = Red2.TeamNumber;
            qualsRow.Blue1.Value = Blue1.TeamNumber;
            qualsRow.Blue2.Value = Blue2.TeamNumber;
            qualsRow.Red1Surrogate.Value = Red1Surrogate;
            qualsRow.Red2Surrogate.Value = Red2Surrogate;
            qualsRow.Blue1Surrogate.Value = Blue1Surrogate;
            qualsRow.Blue2Surrogate.Value = Blue2Surrogate;

            qualsDataRow.Match.Value = MatchNumber;
            qualsDataRow.Status.Value = (int)Enums.TMatchState.Unplayed;
            qualsDataRow.Randomization.Value = (int)TRandomization.DefaultValue;
            qualsDataRow.Start.Value = DateTimeAsInteger.QualsDataDefault;
            qualsDataRow.ScheduleStart.Value = ScheduleStart;
            qualsDataRow.PostedTime.Value = DateTimeAsInteger.QualsDataDefault;
            qualsDataRow.FMSMatchId.Value = FMSMatchId;
            qualsDataRow.FMSScheduleDetailId = FMSScheduleDetailIdAsString.CreateFrom(FMSScheduleDetailId);

            matchScheduleRow.Start.Value = ScheduleStart;
            matchScheduleRow.End.Value = ScheduleStart + Duration;
            matchScheduleRow.Type.Value = (int)TMatchScheduleType.Match;
            matchScheduleRow.Label.Value = Description;

            scheduledMatchRow.AddToTableAndSave();
            qualsRow.AddToTableAndSave();
            qualsDataRow.AddToTableAndSave();
            matchScheduleRow.AddToTableAndSave();

            foreach (var alliance in EnumUtil.GetValues<TAlliance>())
                {
                foreach (var station in EnumUtil.GetValues<TStation>())
                    {
                    DBTables.ScheduleStation.Row row = Database.Tables.ScheduledMatchStation.NewRow();

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

                    row.AddToTableAndSave();
                    }
                }

            if (scoreMatch)
                {
                // See SQLiteMatchDAO.java.commitMatch()
                PlayedMatch match = PlayMatch();
                //
                match.Commit(TCommitType.COMMIT);
                //
                var updateRow = Database.Tables.QualsData.CopyRow(qualsDataRow);
                updateRow.Status.Value = (int)TMatchState.Committed;
                updateRow.Randomization.Value = (int)match.Randomization;
                updateRow.Start.Value = match.Start;
                updateRow.Update(qualsDataRow.Columns(new [] { "Status", "Randomization", "Start" }), qualsDataRow.Where("Match", MatchNumber));

                var qualsCommitHistory = Database.Tables.QualsCommitHistory.NewRow();
                qualsCommitHistory.MatchNumber.Value = match.MatchNumber;
                qualsCommitHistory.Ts.Value = match.LastCommitTime;
                qualsCommitHistory.Start.Value = match.Start;
                qualsCommitHistory.Randomization.Value = (int)match.Randomization;
                qualsCommitHistory.CommitType.Value = (int?)match.LastCommitType;
                qualsCommitHistory.AddToTableAndSave();

                var qualsResults = Database.Tables.QualsResults.NewRow();
                qualsResults.MatchNumber.Value = match.MatchNumber;
                qualsResults.RedScore.Value = match.RedScore;
                qualsResults.BlueScore.Value = match.BlueScore;
                qualsResults.RedPenaltyCommitted.Value = match.RedPenalty;
                qualsResults.BluePenaltyCommitted.Value = match.BluePenalty;
                qualsResults.AddToTableAndSave();

                byte[] scoreDetailsGZIP = null; // To come
                }
            }
        }
    }
