#pragma warning disable 649
using System;

namespace FEMC.DBTables
    {
    class QualsScores : Table<QualsScores.Row, Tuple<NullableLong, NullableLong>>
        {
        public class Row : TableRow<Tuple<NullableLong, NullableLong>>
            {
            public NullableLong Match;
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

            public override Tuple<NullableLong, NullableLong> PrimaryKey => new Tuple<NullableLong, NullableLong>(Match, Alliance);
            }

        public QualsScores(Database database) : base(database)
            {
            }

        public override string TableName => "qualsScores";
        }
    }
