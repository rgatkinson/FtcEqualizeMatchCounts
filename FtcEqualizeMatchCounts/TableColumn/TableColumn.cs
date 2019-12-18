using System;
using System.Text;
using Microsoft.Data.Sqlite;

namespace FEMC
    {
    abstract class TableColumn
        {
        public abstract void LoadDatabaseValue(object value);

        public abstract void SaveDatabaseValue(SqliteParameter parameter);
        }
    }