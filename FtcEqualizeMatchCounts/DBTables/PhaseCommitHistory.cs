using System;

#pragma warning disable 649
namespace FEMC.DBTables
    {
    abstract class PhaseCommitHistory : Table<PhaseCommitHistory.Row, Tuple<NullableLong, DateTimeAsInteger>>
        {
        public class Row : TableRow<Row, Tuple<NullableLong, DateTimeAsInteger>>
            {
            public NullableLong MatchNumber;
            public DateTimeAsInteger Ts;
            public DateTimeAsInteger Start;
            public NullableLong Randomization;
            public NullableLong CommitType;

            public override Tuple<NullableLong, DateTimeAsInteger> PrimaryKey => new Tuple<NullableLong, DateTimeAsInteger>(MatchNumber, Ts);
            }

        protected PhaseCommitHistory(Database database) : base(database)
            {
            }
        }
    }