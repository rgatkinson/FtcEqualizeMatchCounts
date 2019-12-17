using System.Collections.Generic;
using Microsoft.Data.Sqlite;


namespace FEMC
    {
    //--------------------------------------------------------------------------------------------------------------------------
    // Table
    //--------------------------------------------------------------------------------------------------------------------------

    abstract class Table<Row_T, PrimaryKey_T> where Row_T : TableRow<PrimaryKey_T>, new()
        {
        //----------------------------------------------------------------------------------------------------------------------
        // State
        //----------------------------------------------------------------------------------------------------------------------

        public List<Row_T> Rows = new List<Row_T>();
        public IDictionary<PrimaryKey_T, Row_T> Map = new Dictionary<PrimaryKey_T, Row_T>();
        protected Database database;

        public abstract string TableName { get; }

        //----------------------------------------------------------------------------------------------------------------------
        // Construction
        //----------------------------------------------------------------------------------------------------------------------

        protected Table(Database database)
            {
            Rows = new List<Row_T>();
            this.database = database;
            }

        //----------------------------------------------------------------------------------------------------------------------
        // Loading
        //----------------------------------------------------------------------------------------------------------------------

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
                    Row_T row = new Row_T();
                    for (int i = 0; i < rdr.FieldCount; i++)
                        {
                        object value = rdr.IsDBNull(i) ? null : rdr[i];
                        row.SetField(i, value);
                        }

                    Rows.Add(row);
                    Map[row.PrimaryKey] = row;
                    }
                }
            }
        }
    }
