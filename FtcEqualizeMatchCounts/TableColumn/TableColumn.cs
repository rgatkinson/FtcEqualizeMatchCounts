using System;
using System.Data;
using System.Text;
using Microsoft.Data.Sqlite;

namespace FEMC
    {
    abstract class TableColumn
        {
        public static T CreateFromRuntimeValue<T>(object value) where T: TableColumn, new()
            {
            T t = new T();
            t.SetRuntimeValue(value);
            return t;
            }

        public static T CreateFromDatabaseValue<T>(object databaseValue) where T : TableColumn, new()
            {
            T t = new T();
            t.SetDatabaseValue(databaseValue);
            return t;
            }

        public abstract void SetRuntimeValue(object runtimeValue);
        public abstract object GetRuntimeValue();
        public abstract void SetDatabaseValue(object databaseValue);
        public abstract object GetDatabaseValue();

        public void SaveDatabaseValue(SqliteParameter parameter)
            {
            SetParameterValue(parameter, GetDatabaseValue());
            }

        public static void SetParameterValue(SqliteParameter parameter, object databaseValue)
            {
            parameter.Value = databaseValue ?? DBNull.Value;
            }

        protected static Exception MustBeNonNull(string message)
            {
            return new NoNullAllowedException(message);
            }
        }
    }