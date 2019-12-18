using Microsoft.Data.Sqlite;

namespace FEMC
    {
    class DateTimeAsInteger : DateTimeColumn
        {
        public override void LoadDatabaseValue(object value)
            {
            LoadDatabaseValue((long)value);
            }

        public override void SaveDatabaseValue(SqliteParameter parameter)
            {
            parameter.Value = MsSince1970UnixEpoch;
            }
        }
    }