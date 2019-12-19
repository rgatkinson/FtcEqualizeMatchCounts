using Microsoft.Data.Sqlite;

namespace FEMC
    {
    class GuidColumnAsString : GuidColumn
        {
        public override void SaveDatabaseValue(SqliteParameter parameter)
            {
            SetParameterValue(parameter, Value?.ToString("D"));
            }
        }
    }