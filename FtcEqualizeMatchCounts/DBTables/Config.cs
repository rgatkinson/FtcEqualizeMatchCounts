﻿#pragma warning disable 649

namespace FEMC.DBTables
    {
    class Config: Table<Config.Row, string>
        {
        public class Row : TableRow<Row, string>
            {
            public StringColumn Key;
            public StringColumn Value;

            public override string PrimaryKey => Key.NonNullValue;
            }

        public Config(Database database) : base(database)
            {
            }

        public override string TableName => "config";
        }
    }
