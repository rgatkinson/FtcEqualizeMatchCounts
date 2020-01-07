#pragma warning disable 649
using System;

namespace FEMC.DBTables
    {
    class QualsScores : Table<QualsScores.Row, (long, long)>
        {
        public class Row : TableRow<Row, (long, long)>
            {
            public NullableLong MatchNumber;
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

            public override (long, long) PrimaryKey => (MatchNumber.NonNullValue, Alliance.NonNullValue);
            }

        public QualsScores(Database database) : base(database)
            {
            }

        public override string TableName => "qualsScores";
        }
    }
