#pragma warning disable 649
using System;

namespace FEMC.DBTables
    {
    class ElimsScores : Table<ElimsScores.Row, (long, long)>
        {
        public class Row : TableRow<Row, (long, long)>
            {
            public NullableLong MatchNumber;
            public NullableLong Alliance;
            public NullableLong Card;
            public BooleanAsInteger DQ;
            public BooleanAsInteger NoShow1;
            public BooleanAsInteger NoShow2;
            public BooleanAsInteger NoShow3;
            public NullableLong Major;
            public NullableLong Minor;
            public NullableLong Adjust;

            public override (long, long) PrimaryKey => (MatchNumber.NonNullValue, Alliance.NonNullValue);
            }

        public ElimsScores(Database database) : base(database)
            {
            }

        public override string TableName => "elimsScores";
        }
    }
