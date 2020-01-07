#pragma warning disable 649
using System;

namespace FEMC.DBTables
    {
    class LeagueConfig: Table<LeagueConfig.Row, Tuple<StringColumn, StringColumn>>
        {
        public class Row : TableRow<Row, Tuple<StringColumn, StringColumn>>
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
