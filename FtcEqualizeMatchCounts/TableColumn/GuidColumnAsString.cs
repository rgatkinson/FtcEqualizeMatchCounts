﻿using Microsoft.Data.Sqlite;

namespace FEMC
    {
    class GuidColumnAsString : GuidColumn
        {
        public override void LoadDatabaseValue(object value)
            {
            this.LoadDatabaseValue((string)value);
            }

        public override void SaveDatabaseValue(SqliteParameter parameter)
            {
            SetParameterValue(parameter, Value?.ToString("D"));
            }
        }
    }