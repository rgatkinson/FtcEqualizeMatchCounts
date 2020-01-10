using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Security.Permissions;
using FEMC.DBTables;
using FEMC.Enums;
using Microsoft.Data.Sqlite;


namespace FEMC
    {
    //--------------------------------------------------------------------------------------------------------------------------
    // Table
    //--------------------------------------------------------------------------------------------------------------------------

    abstract class AbstractTable<TRowAbstract, TPrimaryKey>
        {
        public abstract string TableName { get; }
        public abstract void InsertRow(TRowAbstract row);
        public abstract bool DeleteRow(TRowAbstract row);
        public abstract bool DeleteRowWithKey(TPrimaryKey key);
        public abstract IEnumerable<TRowAbstract> RowsWhere(IEnumerable<(FieldInfo, SqlOperator, object)> conditions);

        public abstract Database Database { get; }
        public abstract List<string> ColumnNames { get; }

        public ProgramOptions ProgramOptions => Database.ProgramOptions;
        }

    abstract class Table<TRow, TPrimaryKey> : AbstractTable<TableRow<TRow, TPrimaryKey>, TPrimaryKey> where TRow : TableRow<TRow, TPrimaryKey>, new()
        {
        //----------------------------------------------------------------------------------------------------------------------
        // State
        //----------------------------------------------------------------------------------------------------------------------

        public List<TRow> Rows = new List<TRow>();
        public IDictionary<TPrimaryKey, TRow> Map = new Dictionary<TPrimaryKey, TRow>();
        protected Database database;

        public override Database Database => database;

        public override List<string> ColumnNames => columnNames;

        private List<string> columnNames = new List<string>();

        //----------------------------------------------------------------------------------------------------------------------
        // Construction
        //----------------------------------------------------------------------------------------------------------------------

        protected Table(Database database)
            {
            Rows = new List<TRow>();
            this.database = database;
            }

        //----------------------------------------------------------------------------------------------------------------------
        // Row Management
        //----------------------------------------------------------------------------------------------------------------------

        public TRow NewRow()
            {
            TRow result = new TRow();
            result.Table = this;
            result.InitializeFields();
            return result;
            }

        public override void InsertRow(TableRow<TRow, TPrimaryKey> row)
            {
            Trace.Assert(row.Table == this);
            Rows.Add((TRow) row);
            Map[row.PrimaryKey] = (TRow)row;
            }

        public override bool DeleteRow(TableRow<TRow, TPrimaryKey> row)
            {
            if (Rows.Remove((TRow)row))
                {
                Map.Remove(row.PrimaryKey);
                return true;
                }
            return false;
            }

        public override bool DeleteRowWithKey(TPrimaryKey key)
            {
            if (Map.TryGetValue(key, out TRow row))
                {
                DeleteRow(row);
                return true;
                }
            return false;
            }

        public override IEnumerable<TableRow<TRow, TPrimaryKey>> RowsWhere(IEnumerable<(FieldInfo, SqlOperator, object)> conditions)
            {
            List<(FieldInfo, SqlOperator, object)> conditionsList = new List<(FieldInfo, SqlOperator, object)>(conditions);
            foreach (var row in new List<TRow>(Rows))
                {
                bool passes = true;
                foreach (var term in conditionsList)
                    {
                    object value = term.Item1.GetValue(row);
                    passes = term.Item2.Test(value, term.Item3);
                    if (!passes)
                        break;
                    }
                if (passes)
                    {
                    yield return row;
                    }
                }
            }

        //----------------------------------------------------------------------------------------------------------------------
        // Loading
        //----------------------------------------------------------------------------------------------------------------------

        public void Clear()
            {
            Rows.Clear();
            columnNames.Clear();
            Map.Clear();
            }

        public void Load()
            {
            Clear();
            using (var cmd = database.Connection.CreateCommand())
                {
                cmd.CommandText = $"PRAGMA table_info('{ TableName }');";
                SqliteDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                    {
                    string columnName = (string)rdr[1];
                    columnNames.Add(columnName);
                    }
                }

            using (var cmd = database.Connection.CreateCommand())
                { 
                cmd.CommandText = $"SELECT * FROM { TableName }";

                SqliteDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                    {
                    TRow row = new TRow();
                    row.Table = this;
                    for (int i = 0; i < rdr.FieldCount; i++)
                        {
                        object databaseValue = rdr.IsDBNull(i) ? null : rdr[i];
                        row.SetDatabaseValue(i, databaseValue);
                        }

                    InsertRow(row);
                    Map[row.PrimaryKey] = row;
                    }
                }
            }

        public TRow FindFirstRow(Predicate<TRow> predicate)
            {
            foreach (TRow row in Rows)
                {
                if (predicate(row))
                    {
                    return row;
                    }
                }
            return null;
            }
        }
    }
