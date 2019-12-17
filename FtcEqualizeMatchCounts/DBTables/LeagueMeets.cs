using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEMC.DBTables
    {
    class LeagueMeets: Table<LeagueMeets.Row, StringColumn>
        {
        public class Row : TableRow<StringColumn>
            {
            public StringColumn EventCode;
            public StringColumn Name;
            public DateTimeAsInteger Start;
            public DateTimeAsInteger End;

            public override StringColumn PrimaryKey => EventCode;
            }

        public LeagueMeets(Database database) : base(database)
            {
            }

        public override string TableName => "leagueMeets";
        }
    }
