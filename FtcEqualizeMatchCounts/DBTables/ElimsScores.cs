#pragma warning disable 649
using System;

namespace FEMC.DBTables
    {
    class ElimsScores : Table<ElimsScores.Row, Tuple<NullableLong, NullableLong>>
        {
        public class Row : TableRow<Tuple<NullableLong, NullableLong>>
            {
            public NullableLong Match;
            public NullableLong Alliance;
            public NullableLong Card;
            public BooleanAsInteger DQ;
            public BooleanAsInteger NoShow1;
            public BooleanAsInteger NoShow2;
            public BooleanAsInteger NoShow3;
            public NullableLong Major;
            public NullableLong Minor;
            public NullableLong Adjust;

            public override Tuple<NullableLong, NullableLong> PrimaryKey => new Tuple<NullableLong, NullableLong>(Match, Alliance);
            }

        public ElimsScores(Database database) : base(database)
            {
            }

        public override string TableName => "elimsScores";
        }
    }
