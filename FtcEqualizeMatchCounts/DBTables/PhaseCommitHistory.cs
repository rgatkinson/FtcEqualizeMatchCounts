using System;

#pragma warning disable 649
namespace FEMC.DBTables
    {
    abstract class PhaseCommitHistory : Table<PhaseCommitHistory.Row, (long, DateTimeOffset)>
        {
        public class Row : TableRow<Row, (long, DateTimeOffset)>
            {
            public NullableLong MatchNumber;
            public DateTimeAsInteger Ts;
            public DateTimeAsInteger Start;
            public NullableLong Randomization;
            public NullableLong CommitType;

            public override (long, DateTimeOffset) PrimaryKey => (MatchNumber.NonNullValue, Ts.DateTimeOffsetNonNull);
            }

        protected PhaseCommitHistory(Database database) : base(database)
            {
            }
        }
    }