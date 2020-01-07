#pragma warning disable 649
using System;

namespace FEMC.DBTables
    {
    class QualsScores : Table<QualsScores.Row, Tuple<long, long>>
        {
        public class Row : TableRow<Row, Tuple<long, long>>
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

            public override Tuple<long, long> PrimaryKey => new Tuple<long, long>(MatchNumber.NonNullValue, Alliance.NonNullValue);
            }

        public QualsScores(Database database) : base(database)
            {
            }

        public override string TableName => "qualsScores";
        }
    }
