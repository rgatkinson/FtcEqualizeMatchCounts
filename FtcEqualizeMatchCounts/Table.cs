using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;


namespace FEMC
    {
    //--------------------------------------------------------------------------------------------------------------------------
    // Table
    //--------------------------------------------------------------------------------------------------------------------------

    abstract class AbstractTable<TRow>
        {
        public abstract string TableName { get; }
        public abstract void AddRow(TRow row);
        public abstract Database Database { get; }

        public ProgramOptions ProgramOptions => Database.ProgramOptions;
        }

    abstract class Table<TRow, TPrimaryKey> : AbstractTable<TableRow<TPrimaryKey>> where TRow : TableRow<TPrimaryKey>, new()
        {
        //----------------------------------------------------------------------------------------------------------------------
        // State
        //----------------------------------------------------------------------------------------------------------------------

        public List<TRow> Rows = new List<TRow>();
        public IDictionary<TPrimaryKey, TRow> Map = new Dictionary<TPrimaryKey, TRow>();
        protected Database database;

        public override Database Database => database;

        //----------------------------------------------------------------------------------------------------------------------
        // Construction
        //----------------------------------------------------------------------------------------------------------------------

        protected Table(Database database)
            {
            Rows = new List<TRow>();
            this.database = database;
            }

        //----------------------------------------------------------------------------------------------------------------------
        // Loading
        //----------------------------------------------------------------------------------------------------------------------

        public override void AddRow(TableRow<TPrimaryKey> row)
            {
            row.Table = this;
            Rows.Add((TRow) row);
            }

        public void Clear()
            {
            Rows.Clear();
            Map.Clear();
            }

        public void Load()
            {
            Clear();
            using (var cmd = database.Connection.CreateCommand())
                { 
                cmd.CommandText = $"SELECT * FROM { TableName }";

                SqliteDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                    {
                    TRow row = new TRow();
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
        }
    }
