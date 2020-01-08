using System;
using System.Data;
using System.Text;
using Microsoft.Data.Sqlite;

namespace FEMC
    {
    abstract class TableColumn
        {
        public static T CreateFromValue<T>(object value) where T: TableColumn, new()
            {
            T t = new T();
            t.SetValue(value);
            return t;
            }

        public static T CreateFromDatabaseValue<T>(object databaseValue) where T : TableColumn, new()
            {
            T t = new T();
            t.LoadDatabaseValue(databaseValue);
            return t;
            }

        public abstract void SetValue(object runtimeValue);
        public abstract void LoadDatabaseValue(object databaseValue);
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