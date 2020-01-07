#pragma warning disable 649

namespace FEMC.DBTables
    {
    class LeagueMeets: Table<LeagueMeets.Row, string>
        {
        public class Row : TableRow<Row, string>
            {
            public StringColumn EventCode;
            public StringColumn Name;
            public DateTimeAsInteger Start;
            public DateTimeAsInteger End;

            public override string PrimaryKey => EventCode.NonNullValue;
            }

        public LeagueMeets(Database database) : base(database)
            {
            }

        public override string TableName => "leagueMeets";
        }
    }
