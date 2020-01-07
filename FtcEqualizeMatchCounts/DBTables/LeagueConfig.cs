#pragma warning disable 649
using System;

namespace FEMC.DBTables
    {
    class LeagueConfig: Table<LeagueConfig.Row, (string, string)>
        {
        public class Row : TableRow<Row, (string, string)>
            {
            StringColumn League;
            StringColumn Key;
            StringColumn Value;

            public override (string, string) PrimaryKey => (League.NonNullValue, Key.NonNullValue);
            }

        public LeagueConfig(Database database) : base(database)
            {
            }

        public override string TableName => "leagueConfig";
        }
    }
