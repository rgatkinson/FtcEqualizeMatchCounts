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
        public long RedScore;
        public long RedPenalty;
        public long BlueScore;
        public long BluePenalty;

        public TRandomization Randomization = TRandomization.DefaultValue;
        public TMatchState Status = TMatchState.Unplayed;
        public DateTimeOffset Start = DateTimeAsInteger.QualsDataDefault;

        public DateTimeOffset LastCommitTime = DateTimeAsInteger.QualsDataDefault;
        public TCommitType? LastCommitType = null;

        //----------------------------------------------------------------------------------------
        // Construction
        //----------------------------------------------------------------------------------------

        public PlayedMatch(Database db, FMSScheduleDetailId scheduleDetailId) : base(db, db.ThisFMSEventId, scheduleDetailId)
            {
            }

        public PlayedMatch(Database db, DBTables.Match.Row row) : base(db, row.FMSEventId, row.FMSScheduleDetailId)
            {
            Load(row);
            }

        public void Load(DBTables.Match.Row row)
            {
            FmsMatchId = row.FMSMatchId;
            PlayNumber = row.PlayNumber.NonNullValue;
            RedScore = row.RedScore.NonNullValue;
            RedPenalty = row.RedPenalty.NonNullValue;
            BlueScore = row.BlueScore.NonNullValue;
            BluePenalty = row.BluePenalty.NonNullValue;
            }

        public void Load(PhaseData.Row row)
            {
            Randomization = EnumUtil.From<TRandomization>(row.Randomization.NonNullValue);
            Status = EnumUtil.From<TMatchState>(row.Status.NonNullValue);
            Start = row.Start.DateTimeNonNull;
            // more
            }

        public void Load(PhaseCommitHistory.Row row)
            {
            if (LastCommitTime < row.Ts.DateTimeOffsetNonNull)
                {
                LastCommitTime = row.Ts.DateTimeNonNull;
                LastCommitType = EnumUtil.From<TCommitType>(row.CommitType.NonNullValue);
                }
            // more
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
