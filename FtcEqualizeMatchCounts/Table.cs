using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Security.Cryptography;
using System.Security.Permissions;
using Microsoft.Data.Sqlite;


namespace FEMC
    {
    // Notes:
    //      FMSMatchId is a UUID
    //          in qualsData, stored as text
    //
    //      FMSScheduleDetailId is a UUID
    //          in qualsData, stored as text
    //          in ScheduleDetail, stored as blob
    //              so is FMSEventId: is this *also* a UUID?
    //          ditto in ScheduleStation
    //              ditto FMSEventId, FMSTeamId therein

    //--------------------------------------------------------------------------------------------------------------------------
    // Table
    //--------------------------------------------------------------------------------------------------------------------------

    abstract class Table<Row_T> where Row_T : TableRow, new()
        {
        public List<Row_T> Rows;
        protected Database database;
        
        protected Table(Database database)
            {
            Rows = new List<Row_T>();
            this.database = database;
            }

        public abstract string TableName { get; }

        public void Load()
            {
            List<Row_T> result = new List<Row_T>();

            using var cmd = database.Connection.CreateCommand();
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
                result.Add(row);
                }

            Rows = result;
            }
        }
    }
