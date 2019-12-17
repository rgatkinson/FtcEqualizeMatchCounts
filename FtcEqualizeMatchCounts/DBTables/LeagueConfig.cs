using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEMC.DBTables
    {
    class LeagueConfig: Table<LeagueConfig.Row, Tuple<StringColumn, StringColumn>>
        {
        public class Row : TableRow<Tuple<StringColumn, StringColumn>>
            {
            StringColumn League;
            StringColumn Key;
            StringColumn Value;

            public override Tuple<StringColumn, StringColumn> PrimaryKey => new Tuple<StringColumn, StringColumn>(League, Key);
            }

        public LeagueConfig(Database database) : base(database)
            {
            }

        public override string TableName => "leagueConfig";
        }
    }
