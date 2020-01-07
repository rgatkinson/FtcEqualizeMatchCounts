using System;
using System.Diagnostics;
using FEMC.DBTables;
using FEMC.Enums;

namespace FEMC.DAL
    {
    class PlayedMatch : ThisEventMatch
        {
        //----------------------------------------------------------------------------------------
        // State
        //----------------------------------------------------------------------------------------

        public FMSMatchId FmsMatchId;
        public long PlayNumber;
        public long FieldType;
        public DateTimeOffset? InitialPreStartTime;
        public DateTimeOffset? FinalPreStartTime;
        public long PreStartCount;

        public long RedScore;
        public long RedPenalty;
        public long BlueScore;
        public long BluePenalty;

        public TRandomization Randomization = TRandomization.DefaultValue;
        public TMatchState Status = TMatchState.Unplayed;
        public DateTimeOffset Start = DateTimeAsInteger.QualsDataDefault;

        public DateTimeOffset LastCommitTime = DateTimeAsInteger.QualsDataDefault;
        public TCommitType? LastCommitType = null;

        public SkystoneScores RedScores;
        public SkystoneScores BlueScores;

        //----------------------------------------------------------------------------------------
        // Construction
        //----------------------------------------------------------------------------------------

        public PlayedMatch(Database db, FMSScheduleDetailId scheduleDetailId) : base(db, db.ThisFMSEventId, scheduleDetailId)
            {
            Initialize();
            }

        public PlayedMatch(Database db, DBTables.Match.Row row) : base(db, row.FMSEventId, row.FMSScheduleDetailId)
            {
            Initialize();
            Load(row);
            }

        protected void Initialize()
            {
            RedScores = new SkystoneScores(this);
            BlueScores = new SkystoneScores(this);
            }

    public void Load(DBTables.Match.Row row)
            {
            FmsMatchId = row.FMSMatchId;
            PlayNumber = row.PlayNumber.NonNullValue;
            FieldType = row.FieldType.NonNullValue;
            InitialPreStartTime = row.InitialPrestartTime.DateTimeOffset;
            FinalPreStartTime = row.FinalPreStartTime.DateTimeOffset;
            PreStartCount = row.PreStartCount.NonNullValue;

            RedScore = row.RedScore.NonNullValue;
            RedPenalty = row.RedPenalty.NonNullValue;
            BlueScore = row.BlueScore.NonNullValue;
            BluePenalty = row.BluePenalty.NonNullValue;
            // more
            }

        public void Load(PhaseData.Row row)
            {
            Randomization = EnumUtil.From<TRandomization>(row.Randomization.NonNullValue);
            Status = EnumUtil.From<TMatchState>(row.Status.NonNullValue);
            Start = row.Start.DateTimeNonNull;
            // more
            }

        public void Load(QualsScores.Row row)
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

        public void Load(ElimsScores.Row row)
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
            if (LastCommitTime < row.Ts.DateTimeOffsetNonNull)
                {
                LastCommitTime = row.Ts.DateTimeOffsetNonNull;
                LastCommitType = EnumUtil.From<TCommitType>(row.CommitType.NonNullValue);
                }
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
        // Accessing
        //----------------------------------------------------------------------------------------

        public void Commit(TCommitType commitType)
            {
            DateTimeOffset commitTime = DateTimeOffset.Now.ToUniversalTime();
            Trace.Assert(LastCommitTime <= commitTime);

            LastCommitTime = commitTime;
            LastCommitType = commitType;
            }

        }
    }
