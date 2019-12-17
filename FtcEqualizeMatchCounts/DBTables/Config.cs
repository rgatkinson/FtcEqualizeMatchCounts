using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEMC.DBTables
    {
    class Config: Table<Config.Row, string>
        {
        public class Row : TableRow<string>
            {
            public StringColumn Key;
            public StringColumn Value;

            public override string PrimaryKey => Key.Value;
            }

        public Config(Database database) : base(database)
            {
            }

        public override string TableName => "config";
        }
    }
