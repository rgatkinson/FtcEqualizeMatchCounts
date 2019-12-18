using System;
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
            if (value==null)
                {
                parameter.Value = DBNull.Value;
                }
            else
                {
                parameter.Value = value;
                }
            }
        }
    }