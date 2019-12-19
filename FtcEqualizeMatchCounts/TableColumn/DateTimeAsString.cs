using Microsoft.Data.Sqlite;

namespace FEMC
    {
    class DateTimeAsString : DateTimeColumn // Nullable always?
        {
        public override void SaveDatabaseValue(SqliteParameter parameter)
            {
            SetParameterValue(parameter, Iso8601String);
            }
        }
    }