using System;
using System.Data;
using System.Text;
using Microsoft.Data.Sqlite;

namespace FEMC
    {
    abstract class TableColumn
        {
        public static T Create<T>(object value) where T: TableColumn, new()
            {
            T t = new T();
            t.LoadDatabaseValue(value);
            return t;
            }

        public abstract void LoadDatabaseValue(object value);
        public abstract object GetDatabaseValue();
        public void SaveDatabaseValue(SqliteParameter parameter)
            {
            SetParameterValue(parameter, GetDatabaseValue());
            }

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