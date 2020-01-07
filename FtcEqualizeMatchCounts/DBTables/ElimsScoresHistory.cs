using System;

#pragma warning disable 649
namespace FEMC.DBTables
    {
    class ElimsScoresHistory : Table<ElimsScoresHistory.Row, (long, DateTimeOffset)>
        {
        public class Row : TableRow<Row, (long, DateTimeOffset)>
            {
            public NullableLong MatchNumber;
            public DateTimeAsInteger Ts;

            public NullableLong Alliance;
            public NullableLong Card;
            public BooleanAsInteger DQ;
            public BooleanAsInteger NoShow1;
            public BooleanAsInteger NoShow2;
            public BooleanAsInteger NoShow3;
            public NullableLong Major;
            public NullableLong Minor;
            public NullableLong Adjust;

            public override (long, DateTimeOffset) PrimaryKey => (MatchNumber.NonNullValue, Ts.DateTimeOffsetNonNull);
            }

        public override string TableName => "elimsScoresHistory";

        public ElimsScoresHistory(Database database) : base(database)
            {
            }
        }
    }