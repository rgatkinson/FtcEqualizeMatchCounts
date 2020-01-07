using System;
using System.Diagnostics;
using System.Security.Permissions;
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
        public long RedAutoScore;
        public long BlueAutoScore;
        public byte[] ScoreDetails = new byte[0];

        public TRandomization Randomization = TRandomization.DefaultValue;
        public TMatchState Status = TMatchState.Unplayed;
        public DateTimeOffset StartTime = DateTimeAsInteger.QualsDataDefault;
        public DateTimeOffset AutoStartTime => StartTime; // redundantly stored
        public DateTimeOffset? AutoEndTime = null;
        public DateTimeOffset? TeleopStartTime = null;
        public DateTimeOffset? TeleopEndTime = null;
        public DateTimeOffset? RefCommitTime = null;
        public DateTimeOffset? ScoreKeeperCommitTime = DateTimeAsInteger.QualsDataDefault; // never null in the DB
        public DateTimeOffset? PostMatchTime = null;
        public DateTimeOffset? CancelMatchTime = null;
        public DateTimeOffset? CycleTime = null;

        public long HeadRefReview = 0;
        public string VideoUrl = null;

        public DateTimeOffset? CreatedOn = null;
        public string CreatedBy = null;
        public DateTimeOffset? ModifiedOn = null;
        public string ModifiedBy = null;

        public FMSEventId FmsEventId = new FMSEventId();
        public RowVersion RowVersion = new RowVersion();

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

            AutoEndTime = row.AutoEndTime.DateTimeOffset;
            TeleopStartTime = row.TeleopStartTime.DateTimeOffset;
            TeleopEndTime = row.TeleopEndTime.DateTimeOffset;
            RefCommitTime = row.RefCommitTime.DateTimeOffset;
            ScoreKeeperCommitTime = row.ScoreKeeperCommitTime.DateTimeOffset;
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

            CreatedOn = row.CreatedOn.DateTimeOffset;
            CreatedBy = row.CreatedBy.Value;
            ModifiedOn = row.ModifiedOn.DateTimeOffset;
            ModifiedBy = row.ModifiedBy.Value;

            FMSEventId.Value = row.FMSEventId.Value;
            RowVersion.Value = row.RowVersion.Value;
            }

        public void Load(PhaseData.Row row)
            {
            Randomization = EnumUtil.From<TRandomization>(row.Randomization.NonNullValue);
            Status = EnumUtil.From<TMatchState>(row.Status.NonNullValue);
            StartTime = row.Start.DateTimeNonNull;
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
