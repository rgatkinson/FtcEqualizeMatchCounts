using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEMC.DBTables
    {
    class LeagueInfo: Table<LeagueInfo.Row, StringColumn>
        {
        public class Row : TableRow<StringColumn>
            {
            public StringColumn Code;
            public StringColumn Name;
            public StringColumn Country;
            public StringColumn State;
            public StringColumn City;

            public override StringColumn PrimaryKey => Code;
            }

        public LeagueInfo(Database database) : base(database)
            {
            }

        public override string TableName => "leagueInfo";
        }
    }
