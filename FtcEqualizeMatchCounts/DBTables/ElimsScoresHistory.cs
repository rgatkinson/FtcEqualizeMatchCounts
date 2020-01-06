using System;

#pragma warning disable 649
namespace FEMC.DBTables
    {
    class ElimsScoresHistory : Table<ElimsScoresHistory.Row, Tuple<NullableLong, DateTimeAsInteger>>
        {
        public class Row : TableRow<Tuple<NullableLong, DateTimeAsInteger>>
            {
            public NullableLong Match;
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

            public override Tuple<NullableLong, DateTimeAsInteger> PrimaryKey => new Tuple<NullableLong, DateTimeAsInteger>(Match, Ts);
            }

        public override string TableName => "elimsScoresHistory";

        public ElimsScoresHistory(Database database) : base(database)
            {
            }
        }
    }