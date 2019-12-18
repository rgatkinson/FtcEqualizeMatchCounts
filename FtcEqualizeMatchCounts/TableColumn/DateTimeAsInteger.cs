using System;
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
            SetParameterValue(parameter, MsSince1970UnixEpoch);
            }

        public static DateTimeOffset QualsDataDefault = DateTimeOffset.FromUnixTimeMilliseconds(-1);
        }
    }