using Microsoft.Data.Sqlite;

namespace FEMC
    {
    class GuidColumnAsString : GuidColumn
        {
        public override object GetDatabaseValue()
            {
            return Value?.ToString("D");
            }
        }
    }