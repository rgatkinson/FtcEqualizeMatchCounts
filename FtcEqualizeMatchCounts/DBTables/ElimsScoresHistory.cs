using System;

#pragma warning disable 649
namespace FEMC.DBTables
    {
    class ElimsScoresHistory : Table<ElimsScoresHistory.Row, Tuple<NullableLong, DateTimeAsInteger>>
        {
        public class Row : TableRow<Row, Tuple<NullableLong, DateTimeAsInteger>>
            {
            public NullableLong MatchNumber;
            public DateTimeAsInteger Ts;

            public NullableLong Alliance;
            public NullableLong Card;
            public NullableLong DQ;
            public NullableLong NoShow1;
            public NullableLong NoShow2;
            public NullableLong NoShow3;
            public NullableLong Major;
            public NullableLong Minor;
            public NullableLong Adjust;

            public override Tuple<NullableLong, DateTimeAsInteger> PrimaryKey => new Tuple<NullableLong, DateTimeAsInteger>(MatchNumber, Ts);
            }

        public override string TableName => "elimsScoresHistory";

        public ElimsScoresHistory(Database database) : base(database)
            {
            }
        }
    }