using System;

#pragma warning disable 649
namespace FEMC.DBTables
    {
    class QualsScoresHistory : Table<QualsScoresHistory.Row, (long, DateTimeOffset)>
        {
        public class Row : TableRow<Row, (long, DateTimeOffset)>
            {
            public NullableLong MatchNumber;
            public DateTimeAsInteger Ts;

            public NullableLong Alliance;
            public NullableLong Card1;
            public NullableLong Card2;
            public BooleanAsInteger DQ1;
            public BooleanAsInteger DQ2;
            public BooleanAsInteger NoShow1;
            public BooleanAsInteger NoShow2;
            public NullableLong Major;
            public NullableLong Minor;
            public NullableLong Adjust;

            public override (long, DateTimeOffset) PrimaryKey => (MatchNumber.NonNullValue, Ts.DateTimeOffsetNonNull);
            }

        public override string TableName => "qualsScoresHistory";

        public QualsScoresHistory(Database database) : base(database)
            {
            }
        }
    }