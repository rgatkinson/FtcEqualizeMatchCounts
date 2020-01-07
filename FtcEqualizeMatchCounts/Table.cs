using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Data.Sqlite;


namespace FEMC
    {
    //--------------------------------------------------------------------------------------------------------------------------
    // Table
    //--------------------------------------------------------------------------------------------------------------------------

    abstract class AbstractTable<TRowAbstract>
        {
        public abstract string TableName { get; }
        public abstract void AddRow(TRowAbstract row);

        public abstract Database Database { get; }
        public abstract List<string> ColumnNames { get; }

        public ProgramOptions ProgramOptions => Database.ProgramOptions;
        }

    abstract class Table<TRow, TPrimaryKey> : AbstractTable<TableRow<TRow, TPrimaryKey>> where TRow : TableRow<TRow, TPrimaryKey>, new()
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

        public TRow NewRow()
            {
            TRow result = new TRow();
            result.Table = this;
            result.InitializeFields();
            return result;
            }

        //----------------------------------------------------------------------------------------------------------------------
        // Loading
        //----------------------------------------------------------------------------------------------------------------------

        public override void AddRow(TableRow<TRow, TPrimaryKey> row)
            {
            Trace.Assert(row.Table == this);
            Rows.Add((TRow) row);
            }

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
                        object value = rdr.IsDBNull(i) ? null : rdr[i];
                        row.SetField(i, value);
                        }

                    AddRow(row);
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
