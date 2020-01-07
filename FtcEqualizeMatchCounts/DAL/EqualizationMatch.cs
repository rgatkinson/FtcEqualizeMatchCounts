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

        //----------------------------------------------------------------------------------------
        // Playing
        //----------------------------------------------------------------------------------------

        // Play this match with the required Win for Blue
        public PlayedMatch PlayMatch()
            {
            PlayedMatch m = new PlayedMatch(Database, FMSScheduleDetailId);
            // match.MatchNumber = MatchNumber; // not needed: match number comes via FMSScheduleDetailId
            m.FmsMatchId.Value = FMSMatchIdGuid;
            m.PlayNumber = 1; // odd, but that's what SQLiteMachDAO.commitMatch() does
            m.FieldType = 1; // ditto
            m.RedScores.SetRedEqualizationMatch();
            m.BlueScores.SetBlueEqualizationMatch();
            m.ScoreDetails = SkystoneScores.EqualizationScoreDetails();
            m.RedScore = m.RedScores.ScoredPoints;
            m.RedPenalty = m.RedScores.PenaltyPoints;
            m.BlueScore = m.BlueScores.ScoredPoints;
            m.BluePenalty = m.BlueScores.PenaltyPoints;
            m.RedAutoScore = m.RedScores.AutonomousPoints;
            m.BlueAutoScore = m.BlueScores.AutonomousPoints;
            
            m.Commit(TCommitType.COMMIT);
            m.ScoreKeeperCommitTime = m.LastCommitTime;
            m.CreatedBy = CreatedBy;
            m.CreatedOn = m.LastCommitTime;
            m.ModifiedOn = m.LastCommitTime;

            m.StartTime = DateTimeOffset.UtcNow;
            return m;
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

        public void SaveToDatabase(bool scoreThisMatch)
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
            qualsDataRow.Start.Value = DateTimeAsInteger.QualsDataDefault;
            qualsDataRow.ScheduleStart.Value = ScheduleStart;
            qualsDataRow.PostedTime.Value = DateTimeAsInteger.QualsDataDefault;
            qualsDataRow.FMSMatchId.Value = FMSMatchIdGuid;
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

                    row.AddToTableAndSave();
                    }
                }

            if (scoreThisMatch)
                {
                PlayedMatch m = PlayMatch();   // See SQLiteMatchDAO.java.commitMatch()

                // BLOCK
                    {
                    var psData = qualsDataRow.CopyRow();
                    psData.Status.Value = (int)TMatchState.Committed;
                    psData.Randomization.Value = (int)m.Randomization;
                    psData.Start.Value = m.StartTime;
                    psData.Update(qualsDataRow.Columns(new [] { "Status", "Randomization", "Start" }), qualsDataRow.Where("MatchNumber", MatchNumber));
                    }

                // BLOCK
                    { 
                    var psHistory = Database.Tables.QualsCommitHistory.NewRow();
                    psHistory.MatchNumber.Value = m.MatchNumber;
                    psHistory.Ts.Value = m.LastCommitTime;
                    psHistory.Start.Value = m.StartTime;
                    psHistory.Randomization.Value = (int)m.Randomization;
                    psHistory.CommitType.Value = (int?)m.LastCommitType;
                    psHistory.AddToTableAndSave();
                    }

                // BLOCK
                    {
                    var psResult = Database.Tables.QualsResults.NewRow();
                    psResult.MatchNumber.Value = m.MatchNumber;
                    psResult.RedScore.Value = m.RedScore;
                    psResult.BlueScore.Value = m.BlueScore;
                    psResult.RedPenaltyCommitted.Value = m.RedPenalty;
                    psResult.BluePenaltyCommitted.Value = m.BluePenalty;
                    psResult.AddToTableAndSave();
                    }

                // BLOCK
                    { 
                    var fmsMatch = Database.Tables.Match.NewRow();
                    fmsMatch.FMSMatchId.Value = m.FmsMatchId.Value;             // 1
                    fmsMatch.FMSScheduleDetailId.Value = m.FMSScheduleDetailId.Value; // 2
                    fmsMatch.PlayNumber.Value = m.PlayNumber;                   // 3
                    fmsMatch.FieldType.Value = m.FieldType;                     // 4
                    fmsMatch.InitialPrestartTime.Value = m.InitialPreStartTime; // 5
                    fmsMatch.FinalPreStartTime.Value = m.FinalPreStartTime;     // 6
                    fmsMatch.PreStartCount.Value = m.PreStartCount;             // 7
                    fmsMatch.AutoStartTime.Value = m.AutoStartTime;             // 8
                    fmsMatch.AutoEndTime.Value = m.AutoEndTime;                 // 9
                    fmsMatch.TeleopStartTime.Value = m.TeleopStartTime;         // 10
                    fmsMatch.TeleopEndTime.Value = m.TeleopEndTime;             // 11 - TODO: compute from history?
                    fmsMatch.RefCommitTime.Value = m.RefCommitTime;             // 12 - TODO: compute from history?
                    fmsMatch.ScoreKeeperCommitTime.Value = m.ScoreKeeperCommitTime;   // 13
                    fmsMatch.PostMatchTime.Value = m.PostMatchTime;
                    fmsMatch.CancelMatchTime.Value = m.CancelMatchTime;
                    fmsMatch.CycleTime.Value = m.CycleTime;                     // 16

                    fmsMatch.RedScore.Value = m.RedScore;
                    fmsMatch.BlueScore.Value = m.BlueScore;
                    fmsMatch.RedPenalty.Value = m.RedPenalty;
                    fmsMatch.BluePenalty.Value = m.BluePenalty;
                    fmsMatch.RedAutoScore.Value = m.RedAutoScore;
                    fmsMatch.BlueAutoScore.Value = m.BlueAutoScore;             // 22
                    
                    fmsMatch.ScoreDetails.Value = m.ScoreDetails;               // 23
                    fmsMatch.HeadRefReview.Value = m.HeadRefReview;             // 24
                    fmsMatch.VideoUrl.Value = m.VideoUrl;                       // 25
                    
                    fmsMatch.CreatedOn.Value = m.ScoreKeeperCommitTime;         // 26
                    fmsMatch.CreatedBy.Value = m.CreatedBy;                     // 27
                    fmsMatch.ModifiedOn.Value = m.ModifiedOn;                   // 28
                    fmsMatch.ModifiedBy.Value = m.ModifiedBy;                   // 29
                    fmsMatch.FMSEventId.Value = m.FMSEventId.Value;             // 30
                    fmsMatch.RowVersion.Value = m.RowVersion.Value;             // 31

                    fmsMatch.AddToTableAndSave();
                    }

                foreach (var s in new SkystoneScores[] { m.RedScores, m.BlueScores })
                    {
                    int alliance = s == m.RedScores ? 0 : 1;

                    // BLOCK
                        { 
                        var psScores = Database.Tables.QualsScores.NewRow();
                        psScores.MatchNumber.Value = MatchNumber;
                        psScores.Alliance.Value = alliance;
                        s.Save(psScores);
                        psScores.AddToTableAndSave();
                        }

                    // BLOCK
                        {
                        var psScoresHistory = Database.Tables.QualsScoresHistory.NewRow();
                        psScoresHistory.MatchNumber.Value = MatchNumber;
                        psScoresHistory.Ts.Value = m.LastCommitTime;
                        psScoresHistory.Alliance.Value = alliance;
                        s.Save(psScoresHistory);
                        psScoresHistory.AddToTableAndSave();
                        }

                    // BLOCK
                        {
                        var psGame = Database.Tables.QualsGameSpecific.NewRow();
                        psGame.MatchNumber.Value = MatchNumber;
                        psGame.Alliance.Value = alliance;
                        s.Save(psGame);
                        psGame.AddToTableAndSave();
                        }

                    // BLOCK
                        {
                        var psGameHistory = Database.Tables.QualsGameSpecificHistory.NewRow();
                        psGameHistory.MatchNumber.Value = MatchNumber;
                        psGameHistory.Ts.Value = m.LastCommitTime;
                        psGameHistory.Alliance.Value = alliance;
                        s.Save(psGameHistory);
                        psGameHistory.AddToTableAndSave();
                        }
                    }
                }
            }
        }
    }
