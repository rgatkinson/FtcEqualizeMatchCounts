using System;
using System.Data;
using System.Text;
using Microsoft.Data.Sqlite;

namespace FEMC
    {
    abstract class TableColumn
        {
        public abstract void LoadDatabaseValue(object value);
        public abstract void SaveDatabaseValue(SqliteParameter parameter);

        protected void SetParameterValue(SqliteParameter parameter, object value)
            {
            parameter.Value = value ?? DBNull.Value;
            }

        protected static Exception MustBeNonNull(string message)
            {
            return new NoNullAllowedException(message);
            }
        }
    }