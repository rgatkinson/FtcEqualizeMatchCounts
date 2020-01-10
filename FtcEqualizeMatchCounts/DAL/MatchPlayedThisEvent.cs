using FEMC.DAL.Support;
using FEMC.DBTables;
using FEMC.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.IO.Compression;

namespace FEMC.DAL
    {
    class MatchPlayedThisEvent : ThisEventMatch, IPlayedMatch
        {
        //----------------------------------------------------------------------------------------
        // State
        //----------------------------------------------------------------------------------------

        public FMSMatchId FmsMatchId = new FMSMatchId();
        public long PlayNumber;
        public long FieldType;
        public DateTimeOffset? InitialPreStartTime;
        public DateTimeOffset? FinalPreStartTime;
        public long PreStartCount;

        public long RedScore;
        public long RedPenalty;
        public long BlueScore;
        public long BluePenalty;
        public long RedAutoScore;
        public long BlueAutoScore;
        public byte[] ScoreDetails = new byte[0];

        public TRandomization Randomization = TRandomization.DefaultValue;
        public TMatchState MatchState = TMatchState.Unplayed;
        public DateTimeOffset StartTime = DateTimeAsInteger.NegativeOne;
        public DateTimeOffset AutoStartTime => StartTime; // redundantly stored in db
        public DateTimeOffset? AutoEndTime = null;
        public DateTimeOffset? TeleopStartTime = null;
        public DateTimeOffset? PostMatchTime = null;
        public DateTimeOffset? CancelMatchTime = null;
        public DateTimeOffset? CycleTime = null;

        public long HeadRefReview = 0;
        public string VideoUrl = null;

        public string CreatedBy = null;
        public string ModifiedBy = null;

        public RowVersion RowVersion = new RowVersion();

        public List<Commit> CommitHistory;

        public SkystoneScores RedScores;
        public SkystoneScores BlueScores;

        public bool IsQual => isQual;
        protected bool isQual = true;

        public long RedTotal => MatchType==TMatchType.ELIMS && RedScores.card1 >= 2 
            ? RedScores.ScoredPoints
            : Math.Max(0, RedScores.ScoredPoints + BlueScores.PenaltyPoints);
        
        public long BlueTotal => MatchType == TMatchType.ELIMS && BlueScores.card1 >= 2
            ? BlueScores.ScoredPoints
            : Math.Max(0, BlueScores.ScoredPoints + RedScores.PenaltyPoints);

        //----------------------------------------------------------------------------------------
        // Accessing
        //----------------------------------------------------------------------------------------

        public ICollection<MatchResult> MatchResults // MatchSubsystem.java / getResultsForMatch
            {
            get
                {
                List<MatchResult> results = new List<MatchResult>();
                ScheduledMatch s = Scheduled;

                RedScores.SetDqFromCard();
                BlueScores.SetDqFromCard();

                long redTotal = RedTotal;
                long blueTotal = BlueTotal;

                MatchResult red1Result = new MatchResult(s.Red1.TeamNumber, EventCode, MatchNumber, 0, 0, redTotal, RedScores.dq1 || RedScores.noshow1, TMatchOutcome.UNKNOWN);
                MatchResult red2Result = new MatchResult(s.Red2.TeamNumber, EventCode, MatchNumber, 0, 0, redTotal, RedScores.dq2 || RedScores.noshow2, TMatchOutcome.UNKNOWN);
                MatchResult blue1Result = new MatchResult(s.Blue1.TeamNumber, EventCode, MatchNumber, 0, 0, blueTotal, BlueScores.dq1 || BlueScores.noshow1, TMatchOutcome.UNKNOWN);
                MatchResult blue2Result = new MatchResult(s.Blue2.TeamNumber, EventCode, MatchNumber, 0, 0, blueTotal, BlueScores.dq2 || BlueScores.noshow2, TMatchOutcome.UNKNOWN);

                long redRP = 0;
                long blueRP = 0;
                bool tie = false;
                bool redWins = false;
                bool blueWins = false;

                if (redTotal > blueTotal)
                    {
                    redRP = 2;
                    redWins = true;
                    red1Result.Outcome = TMatchOutcome.WIN;
                    red2Result.Outcome = TMatchOutcome.WIN;
                    blue1Result.Outcome = TMatchOutcome.LOSS;
                    blue2Result.Outcome = TMatchOutcome.LOSS;
                    }
                else if (redTotal < blueTotal)
                    {
                    blueRP = 2;
                    blueWins = true;
                    red1Result.Outcome = TMatchOutcome.LOSS;
                    red2Result.Outcome = TMatchOutcome.LOSS;
                    blue1Result.Outcome = TMatchOutcome.WIN;
                    blue2Result.Outcome = TMatchOutcome.WIN;
                    }
                else
                    {
                    Trace.Assert(redTotal == blueTotal);
                    redRP = 1;
                    blueRP = 1;
                    tie = true;
                    red1Result.Outcome = TMatchOutcome.TIE;
                    red2Result.Outcome = TMatchOutcome.TIE;
                    blue1Result.Outcome = TMatchOutcome.TIE;
                    blue2Result.Outcome = TMatchOutcome.TIE;
                    }

                bool red1Counts = !red1Result.DQorNoShow && !s.Red1Surrogate;
                bool red2Counts = !red2Result.DQorNoShow && !s.Red2Surrogate;
                bool blue1Counts = !blue1Result.DQorNoShow && !s.Blue1Surrogate;
                bool blue2Counts = !blue2Result.DQorNoShow && !s.Blue2Surrogate;

                if (red1Counts) red1Result.RankingPoints += redRP;
                if (red2Counts) red2Result.RankingPoints += redRP;
                if (blue1Counts) blue1Result.RankingPoints += blueRP;
                if (blue2Counts) blue2Result.RankingPoints += blueRP;

                if (tie)
                    {
                    long tieBreaker = Math.Min(RedScores.ScoredPoints, BlueScores.ScoredPoints);
                    if (red1Counts) red1Result.TieBreakingPoints += tieBreaker;
                    if (red2Counts) red2Result.TieBreakingPoints += tieBreaker;
                    if (blue1Counts) blue1Result.TieBreakingPoints += tieBreaker;
                    if (blue2Counts) blue2Result.TieBreakingPoints += tieBreaker;
                    }
                else
                    {
                    bool bullet3 = redWins && BlueScores.dq1 && BlueScores.dq2 || blueWins && RedScores.dq1 && RedScores.dq2;
                    if (bullet3)
                        {
                        if (redWins)
                            { 
                            long tieBreaker = RedScores.ScoredPoints;
                            if (red1Counts) red1Result.TieBreakingPoints += tieBreaker;
                            if (red2Counts) red2Result.TieBreakingPoints += tieBreaker;
                            }
                        if (blueWins)
                            {
                            long tieBreaker = BlueScores.ScoredPoints;
                            if (blue1Counts) blue1Result.TieBreakingPoints += tieBreaker;
                            if (blue2Counts) blue2Result.TieBreakingPoints += tieBreaker;
                            }
                        }
                    else
                        {
                        long tieBreaker = redWins ? BlueScores.ScoredPoints : RedScores.ScoredPoints;
                        if (red1Counts) red1Result.TieBreakingPoints += tieBreaker;
                        if (red2Counts) red2Result.TieBreakingPoints += tieBreaker;
                        if (blue1Counts) blue1Result.TieBreakingPoints += tieBreaker;
                        if (blue2Counts) blue2Result.TieBreakingPoints += tieBreaker;
                        }
                    }

                if (!s.Red1Surrogate) results.Add(red1Result);
                if (!s.Red2Surrogate) results.Add(red2Result);
                if (!s.Blue1Surrogate) results.Add(blue1Result);
                if (!s.Blue2Surrogate) results.Add(blue2Result);

                return results;
                }
            }

        public void AddCommitHistory(Commit commit)
            {
            CommitHistory.Add(commit);
            }

        public IEnumerable<Commit> CommitHistoryAscending
            {
            get {
                CommitHistory.Sort((a, b) => a.Ts < b.Ts ? -1 : (a.Ts == b.Ts ? 0 : 1));
                return CommitHistory;
                }
            }

        public Commit CommitHistoryLatest => FindLatestCommit(commit => true);

        public Commit FindLatestCommit(Predicate<Commit> predicate)
            {
            Commit result = null;
            foreach (Commit commit in CommitHistory)
                {
                if (predicate(commit))
                    {
                    if (result == null || result.Ts < commit.Ts)
                        {
                        result = commit;
                        }
                    }
                }
            return result;
            }

        public DateTimeOffset NewCommitInstant
            {
            get {
                DateTimeOffset result = DateTimeOffset.UtcNow;
                Commit last = CommitHistoryLatest;
                if (last != null && result <= last.Ts)
                    {
                    result = last.Ts + TimeSpan.FromMilliseconds(1);
                    }
                return result;
                }
            }

        //----------------------------------------------------------------------------------------
        // Construction
        //----------------------------------------------------------------------------------------

        public MatchPlayedThisEvent(Database db, FMSScheduleDetailId scheduleDetailId) : base(db, db.ThisFMSEventId, scheduleDetailId)
            {
            Initialize();
            }

        public MatchPlayedThisEvent(Database db, DBTables.Match.Row row) : base(db, row.FMSEventId, row.FMSScheduleDetailId)
            {
            Initialize();
            Load(row);
            }

        protected void Initialize()
            {
            RedScores = new SkystoneScores(this);
            BlueScores = new SkystoneScores(this);
            RowVersion.Value = new byte[0];
            CommitHistory = new List<Commit>();
            }

        //----------------------------------------------------------------------------------------
        // Loading
        //----------------------------------------------------------------------------------------

        public void Load(DBTables.Match.Row row)
            {
            FmsMatchId = row.FMSMatchId;
            PlayNumber = row.PlayNumber.NonNullValue;
            FieldType = row.FieldType.NonNullValue;
            InitialPreStartTime = row.InitialPrestartTime.DateTimeOffset;
            FinalPreStartTime = row.FinalPreStartTime.DateTimeOffset;
            PreStartCount = row.PreStartCount.NonNullValue;

            AutoEndTime = row.AutoEndTime.DateTimeOffset;
            TeleopStartTime = row.TeleopStartTime.DateTimeOffset;
            PostMatchTime = row.PostMatchTime.DateTimeOffset;
            CancelMatchTime = row.CancelMatchTime.DateTimeOffset;
            CycleTime = row.CycleTime.DateTimeOffset;

            RedScore = row.RedScore.NonNullValue;
            RedPenalty = row.RedPenalty.NonNullValue;
            BlueScore = row.BlueScore.NonNullValue;
            BluePenalty = row.BluePenalty.NonNullValue;
            RedAutoScore = row.RedAutoScore.NonNullValue;
            BlueAutoScore = row.BlueAutoScore.NonNullValue;
            ScoreDetails = row.ScoreDetails.Value;

            HeadRefReview = row.HeadRefReview.NonNullValue;
            VideoUrl = row.VideoUrl.Value;

            CreatedBy = row.CreatedBy.Value;
            ModifiedBy = row.ModifiedBy.Value;

            // FMSEventId.Value = row.FMSEventId.Value; Don't load: our event id comes from our Scheduled guy
            RowVersion.Value = row.RowVersion.Value;
            }


        public void Load(PhaseData.Row row)
            {
            Randomization = EnumUtil.From<TRandomization>(row.Randomization.NonNullValue);
            MatchState = EnumUtil.From<TMatchState>(row.Status.NonNullValue);
            StartTime = row.Start.DateTimeNonNull;
            // more
            }

        public void Load(QualsScores.Row row)
            {
            isQual = true;
            if (row.Alliance.NonNullValue == 0)
                {
                RedScores.Load(row);
                }
            else
                {
                BlueScores.Load(row);
                }
            }

        public void Load(ElimsScores.Row row)
            {
            isQual = false;
            if (row.Alliance.NonNullValue == 0)
                {
                RedScores.Load(row);
                }
            else
                {
                BlueScores.Load(row);
                }
            }

        public void Load(PhaseGameSpecific.Row row)
            {
            if (row.Alliance.NonNullValue == 0)
                {
                RedScores.Load(row);
                }
            else
                {
                BlueScores.Load(row);
                }
            }

        public void Load(PhaseResults.Row row)
            {
            // This data seems redundant with that in DBTables.Match
            }

        public void Load(PhaseCommitHistory.Row row)
            {
            AddCommitHistory(new Commit(row.Ts.DateTimeOffsetNonNull, EnumUtil.From<TCommitType>(row.CommitType.NonNullValue)));
            // more
            }

        public void Load(QualsScoresHistory.Row row)
            {
            // We don't need history here
            }

        public void Load(ElimsScoresHistory.Row row)
            {
            // We don't need history here
            }

        public void Load(PhaseGameSpecificHistory.Row row)
            {
            // We don't need history here
            }

        //----------------------------------------------------------------------------------------
        // ScoreDetails
        //----------------------------------------------------------------------------------------

        // see SQLiteMatchDAO.java/commitMatch. We're a little simpler here because the C# type system is
        // stronger than the Java type system
        public byte[] EncodeScoreDetails()
            {
            FMSScoreDetails details = new FMSScoreDetails();
            details.RedAllianceScore = new FMSSkystoneScoreDetail(RedScores, BlueScores.PenaltyPoints);
            details.BlueAllianceScore = new FMSSkystoneScoreDetail(BlueScores, RedScores.PenaltyPoints);

            using MemoryStream ms = new MemoryStream();
            using (GZipStream gzipOut = new GZipStream(ms, CompressionMode.Compress))
                {
                DefaultContractResolver contractResolver = new DefaultContractResolver
                    {
                    NamingStrategy = new UpperCamelCaseNamingStrategy(true, false),
                    };
                JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings
                    {
                    ContractResolver = contractResolver
                    });
                using BsonDataWriter bsonDataWriter = new BsonDataWriter(gzipOut);
                serializer.Serialize(bsonDataWriter, details);
                }

            byte[] result = ms.ToArray();
            return result;
            }

        public FMSScoreDetails DecodeScoreDetails()
            {
            using GZipStream gzipIn = new GZipStream(new MemoryStream(ScoreDetails), CompressionMode.Decompress);
            MemoryStream scoreDetailsBson = MiscUtil.ReadFully(gzipIn);
            scoreDetailsBson.Seek(0, SeekOrigin.Begin);
            using BsonDataReader bsonReader = new BsonDataReader(scoreDetailsBson);

            DefaultContractResolver contractResolver = new DefaultContractResolver
                {
                NamingStrategy = new CamelCaseNamingStrategy(true, false)
                };
            JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings
                {
                ContractResolver = contractResolver
                });
            FMSScoreDetails details = serializer.Deserialize<FMSScoreDetails>(bsonReader);
            return details;
            }

        public string ScoreDetailsJson // mostly for debugging
            {
            get
                {
                using GZipStream input = new GZipStream(new MemoryStream(ScoreDetails), CompressionMode.Decompress);
                MemoryStream scoreDetailsBson = MiscUtil.ReadFully(input);
                scoreDetailsBson.Seek(0, SeekOrigin.Begin);
                using BsonDataReader bsonReader = new BsonDataReader(scoreDetailsBson);
                using StringWriter stringWriter = new StringWriter();
                using (JsonWriter jsonWriter = new JsonTextWriter(stringWriter))
                    {
                    while (bsonReader.Read())
                        {
                        jsonWriter.WriteToken(bsonReader.TokenType, bsonReader.Value);
                        }
                    }
                string json = stringWriter.GetStringBuilder().ToString();
                return json;
                }
            }

        //----------------------------------------------------------------------------------------
        // Saving
        //----------------------------------------------------------------------------------------

        public void SaveNonCommitMatchHistory(TCommitType commitType) // see SQLiteMatchDAO/saveNonCommitMatchHistory
            {
            MatchPlayedThisEvent m = this;

            m.AddCommitHistory(new Commit(m.NewCommitInstant, commitType));
            DateTimeOffset commitTime = m.CommitHistoryLatest.Ts;

            // BLOCK
                {
                var psHistory = Database.Tables.QualsCommitHistory.NewRow();
                psHistory.MatchNumber.Value = m.MatchNumber;
                psHistory.Ts.Value = commitTime;
                psHistory.Start.Value = m.StartTime;
                psHistory.Randomization.Value = (int)m.Randomization;
                psHistory.CommitType.Value = (int?)commitType;
                psHistory.Insert();
                }

            foreach (var s in new [] { m.RedScores, m.BlueScores })
                {
                int alliance = s == m.RedScores ? 0 : 1;

                // BLOCK
                    {
                    var psScoresHistory = Database.Tables.QualsScoresHistory.NewRow();
                    psScoresHistory.MatchNumber.Value = MatchNumber;
                    psScoresHistory.Ts.Value = commitTime;
                    psScoresHistory.Alliance.Value = alliance;
                    s.Save(psScoresHistory);
                    psScoresHistory.Insert();
                    }

                // BLOCK
                    {
                    var psGameHistory = Database.Tables.QualsGameSpecificHistory.NewRow();
                    psGameHistory.MatchNumber.Value = MatchNumber;
                    psGameHistory.Ts.Value = commitTime;
                    psGameHistory.Alliance.Value = alliance;
                    s.Save(psGameHistory);
                    psGameHistory.Insert();
                    }
                }
            }

        public void CommitMatch() // MatchSubsystem.java / commitMatch
            {
            MatchPlayedThisEvent m = this;

            m.AddCommitHistory(new Commit(m.NewCommitInstant, TCommitType.COMMIT));
            if (m.StartTime <= DateTimeAsInteger.NegativeOne)
                {
                m.StartTime = DateTimeOffset.UtcNow;
                }

            if (m.MatchType==TMatchType.TEST)
                {
                m.MatchState = TMatchState.Committed;
                }
            else if (m.MatchType == TMatchType.QUALS)
                {
                m.CommitMatchCore();
                m.MatchState = TMatchState.Committed;

                Database.ThisEvent.CalculateAndSetRankings();
                Database.LeagueSubsystem.CalculateAndSetAllLeagueRankings(); // more a formality/mirroring ScoreKeeper, as this makes no persistent updates
                Database.LeagueSubsystem.CreateExportTemp();
                }
            else // if (m.MatchType == TMatchType.ELIMS)
                {
                throw new NotImplementedException($"{GetType().Name}.{nameof(CommitMatch)}: {m.MatchType}");
                }
            }

        protected void CommitMatchCore() // MatchDAO.java / commitMatch
            {
            MatchPlayedThisEvent m = this;

            DateTimeOffset commitTime = m.CommitHistoryLatest.Ts;

            // BLOCK
                {
                var psData = Database.Tables.QualsData.NewRow();
                psData.MatchNumber.Value = m.MatchNumber;
                psData.Status.Value = (int)TMatchState.Committed;
                psData.Randomization.Value = (int)m.Randomization;
                psData.Start.Value = m.StartTime;
                psData.Update(psData.Columns(new[] { nameof(psData.Status), nameof(psData.Randomization), nameof(psData.Start) }), psData.Where(nameof(psData.MatchNumber), MatchNumber));
                }

            // BLOCK
                {
                // (MatchNumber, Ts) must be unique
                var psHistory = Database.Tables.QualsCommitHistory.NewRow();
                psHistory.MatchNumber.Value = m.MatchNumber;
                psHistory.Ts.Value = commitTime;
                psHistory.Start.Value = m.StartTime;
                psHistory.Randomization.Value = (int)m.Randomization;
                psHistory.CommitType.Value = (int?)m.CommitHistoryLatest.CommitType;
                psHistory.InsertOrReplace();
                }

            // BLOCK
                {
                var psResult = Database.Tables.QualsResults.NewRow();
                psResult.MatchNumber.Value = m.MatchNumber;
                psResult.RedScore.Value = m.RedScore;
                psResult.BlueScore.Value = m.BlueScore;
                psResult.RedPenaltyCommitted.Value = m.RedPenalty;
                psResult.BluePenaltyCommitted.Value = m.BluePenalty;
                psResult.InsertOrReplace();
                }

            // BLOCK
                {
                Commit matchEnd = null;
                Commit referee = null;
                Commit scoreKeeper = null;
                Commit firstScoreKeeper = null;
                foreach (var commit in CommitHistoryAscending)
                    {
                    switch (commit.CommitType)
                        {
                        case TCommitType.COMMIT:
                            scoreKeeper = commit;
                            if (firstScoreKeeper == null)
                                {
                                firstScoreKeeper = commit;
                                }
                            break;

                        case TCommitType.MATCH_END:
                            matchEnd = commit;
                            break;

                        case TCommitType.BLUE_REF_SUBMIT:
                        case TCommitType.RED_REF_SUBMIT:
                            referee = commit;
                            break;
                        }
                    }

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
                fmsMatch.TeleopEndTime.Value = matchEnd?.Ts;                // 11
                fmsMatch.RefCommitTime.Value = referee?.Ts;                 // 12
                fmsMatch.ScoreKeeperCommitTime.Value = scoreKeeper?.Ts;     // 13
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

                fmsMatch.CreatedOn.Value = firstScoreKeeper?.Ts ?? DateTimeAsInteger.NegativeOne; // 26
                fmsMatch.CreatedBy.Value = m.CreatedBy;                     // 27
                fmsMatch.ModifiedOn.Value = scoreKeeper?.Ts ?? DateTimeAsInteger.NegativeOne; // 28
                fmsMatch.ModifiedBy.Value = m.ModifiedBy;                   // 29
                fmsMatch.FMSEventId.Value = m.FMSEventId.Value;             // 30
                fmsMatch.RowVersion.Value = m.RowVersion.Value;             // 31

                fmsMatch.InsertOrReplace();

                Database.AddOrReplacePlayedMatch(this);
                }

            foreach (var s in new [] { m.RedScores, m.BlueScores })
                {
                int alliance = s == m.RedScores ? 0 : 1;

                // BLOCK
                    {
                    var psScores = Database.Tables.QualsScores.NewRow();
                    psScores.MatchNumber.Value = MatchNumber;
                    psScores.Alliance.Value = alliance;
                    s.Save(psScores);
                    psScores.InsertOrReplace();
                    }

                // BLOCK
                    {
                    var psScoresHistory = Database.Tables.QualsScoresHistory.NewRow();
                    psScoresHistory.MatchNumber.Value = MatchNumber;
                    psScoresHistory.Ts.Value = commitTime;
                    psScoresHistory.Alliance.Value = alliance;
                    s.Save(psScoresHistory);
                    psScoresHistory.InsertOrReplace();
                    }

                // BLOCK
                    {
                    var psGame = Database.Tables.QualsGameSpecific.NewRow();
                    psGame.MatchNumber.Value = MatchNumber;
                    psGame.Alliance.Value = alliance;
                    s.Save(psGame);
                    psGame.InsertOrReplace();
                    }

                // BLOCK
                    {
                    var psGameHistory = Database.Tables.QualsGameSpecificHistory.NewRow();
                    psGameHistory.MatchNumber.Value = MatchNumber;
                    psGameHistory.Ts.Value = commitTime;
                    psGameHistory.Alliance.Value = alliance;
                    s.Save(psGameHistory);
                    psGameHistory.InsertOrReplace();
                    }
                }
            }
        }
    }
