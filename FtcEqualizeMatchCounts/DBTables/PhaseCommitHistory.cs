using System;

#pragma warning disable 649
namespace FEMC.DBTables
    {
    abstract class PhaseCommitHistory : Table<PhaseCommitHistory.Row, Tuple<NullableLong, DateTimeAsInteger>>
        {
        public class Row : TableRow<Tuple<NullableLong, DateTimeAsInteger>>
            {
            public NullableLong Match;
            public DateTimeAsInteger CommitTime;
            public DateTimeAsInteger Start;
            public NullableLong Random;
            public NullableLong CommitType;

            public override Tuple<NullableLong, DateTimeAsInteger> PrimaryKey => new Tuple<NullableLong, DateTimeAsInteger>(Match, CommitTime);
            }

        protected PhaseCommitHistory(Database database) : base(database)
            {
            }
        }
    }