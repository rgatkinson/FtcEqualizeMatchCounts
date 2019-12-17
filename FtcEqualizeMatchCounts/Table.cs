using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
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

        public List<List<object>> SelectAll()
            {
            string query = $"SELECT * FROM { TableName }";
            List<List<object>> table = database.ExecuteQuery(query);
            return table;
            }

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


    abstract class TableRow
        {
        //----------------------------------------------------------------------------------------------------------------------
        // Accessing
        //----------------------------------------------------------------------------------------------------------------------

        public override string ToString()
            {
            Type type = GetType();
            StringBuilder result = new StringBuilder();

            bool first = true;
            foreach (FieldInfo field in type.GetFields())
                {
                object value = field.GetValue(this);
                if (!first)
                    {
                    result.Append(", ");
                    }
                result.Append(value?.ToString() ?? "null");
                first = false;
                }

            return result.ToString();
            }

        // See
        //  https://www.bricelam.net/2018/05/24/microsoft-data-sqlite-2-1.html#comment-3980760585
        //  https://stackoverflow.com/questions/51933421/system-data-sqlite-vs-microsoft-data-sqlite
        //
        // "we embrace the fact that SQLite only supports four primitive types (INTEGER, REAL, TEXT, and BLOB)
        // and implement ADO.NET APIs in a way that helps you coerce values between these and .NET types"
        // 
        public void SetField(int index, object value)
            {
            Type type = GetType();

            FieldInfo field = type.GetFields()[index];
            Trace.Assert(field.IsPublic);   // TableRow are of limited structure

            if (field.FieldType == typeof(string))
                {
                field.SetValue(this, value);
                }
            else if (field.FieldType == typeof(long))
                {
                field.SetValue(this, value);
                }
            else if (field.FieldType == typeof(double))
                {
                field.SetValue(this, value);
                }
            else if (field.FieldType.IsSubclassOf(typeof(TableColumn)))
                {
                TableColumn column = (TableColumn)Activator.CreateInstance(field.FieldType);
                column.SetValue(value);
                field.SetValue(this, column);
                }
            else
                {
                // not yet handled
                }
            }

        }

    //--------------------------------------------------------------------------------------------------------------------------
    // Columns
    //--------------------------------------------------------------------------------------------------------------------------


    }
