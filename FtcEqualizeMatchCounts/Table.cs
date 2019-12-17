using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using Microsoft.Data.Sqlite;


namespace FtcEqualizeMatchCounts
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

    abstract class Table
        {
        protected Database database;

        protected Table(Database database)
            {
            this.database = database;
            }

        public abstract string TableName { get; }

        public List<List<object>> SelectAll()
            {
            string query = $"SELECT * FROM { TableName }";
            List<List<object>> table = database.ExecuteQuery(query);
            return table;
            }

        public List<Row_T> Load<Row_T>() where Row_T : TableRow, new()
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

            return result;
            }
        }


    abstract class TableRow
        {
        //----------------------------------------------------------------------------------------------------------------------
        // Accessing
        //----------------------------------------------------------------------------------------------------------------------

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

    abstract class TableColumn
        {
        public virtual void SetValue(object value)
            {
            }
        }

    abstract class GuidColumn : TableColumn
        {
        public System.Guid? Value = null;

        public override string ToString()
            {
            return $"{GetType().Name}: { Value?.ToString() ?? "null" }";
            }

        public void SetValue(System.Guid? guid)
            {
            Value = guid;
            }

        public void SetValue(byte[] bytes)
            {
            if (bytes == null)
                SetValue((Guid?)null);
            else
                {
                if (bytes.Length == 0)
                    {
                    bytes = new byte[16]; // account for quirky RowVersion seen in practice (are they *really* guids?)
                    }
                SetValue(new Guid(bytes));
                }
            }

        public void SetValue(string value)
            {
            SetValue(new Guid(value));
            }
        }


    class GuidColumnAsString : GuidColumn
        {
        public override void SetValue(object value)
            {
            this.SetValue((string)value);
            }
        }

    class GuidColumnAsBlob : GuidColumn
        {
        public override void SetValue(object value)
            {
            this.SetValue((byte[]) value);
            }
        }

    class FMSMatchId : GuidColumnAsBlob
        {
        }

    class FMSMatchIdAsString : GuidColumnAsString
        {
        }

    class FMSEventId : GuidColumnAsBlob
        {
        }

    class FMSRegionId : GuidColumnAsBlob
        {
        }

    class FMSScheduleDetailId : GuidColumnAsBlob
        {
        }

    class FMSScheduleDetailIdAsString : GuidColumnAsString
        {
        }

    class FMSTeamId : GuidColumnAsBlob
        {
        }

    class FMSHomeCMPId : GuidColumnAsBlob
        {

        }

    class RowVersion : GuidColumnAsBlob // seen: size=0, size=16, but all zeros
        {

        }

    //------------------------------------

    class DateTimeColumn : TableColumn
        {
        public System.DateTimeOffset? Value;

        public System.DateTime? DateTime => Value?.UtcDateTime.ToUniversalTime();

        public override string ToString()
            {
            // https://stackoverflow.com/questions/44788305/c-sharp-convert-datetime-object-to-iso-8601-string
            // ISO8601 with 3 decimal places
            return DateTime?.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK", CultureInfo.InvariantCulture) ?? "null";
            }

        public void SetValue(System.DateTimeOffset? dateTimeOffset)
            {
            Value = dateTimeOffset;
            }

        public void SetValue(string value)
            {
            SetValue(value == null ? (DateTimeOffset?)null : DateTimeOffset.Parse(value));
            }

        public void SetValue(long msSince1970UnixEpoch)
            {
            SetValue(DateTimeOffset.FromUnixTimeMilliseconds(msSince1970UnixEpoch));
            }
        }

    class DateTimeAsInteger : DateTimeColumn
        {
        public override void SetValue(object value)
            {
            SetValue((long) value);
            }
        }

    class DateTimeAsString : DateTimeColumn // Nullable always?
        {
        // e.g.:
        //  2019-12-15T00:41:31.957Z
        //  2019-12-15T00:40:28.120Z
        public override void SetValue(object value)
            {
            SetValue((string) value);
            }
        }

    class BooleanAsInteger : TableColumn // boolean stored as 'integer' in schema instead of 'boolean'
        {
        public bool Value;

        public override string ToString()
            {
            return Value.ToString();
            }

        public void SetValue(bool value)
            {
            Value = value;
            }

        public void SetValue(long value)
            {
            SetValue(value != 0);
            }

        public override void SetValue(object value)
            {
            SetValue((long)value);
            }
        }

    //------------------------------------

    abstract class BlobColumn : TableColumn
        {
        public byte[] Value;

        public override string ToString()
            {
            return $"{GetType().Name}: { Value?.ToString() ?? "null" }";
            }

        public void SetValue(byte[] value)
            {
            Value = value;
            }

        public override void SetValue(object value)
            {
            SetValue((byte[]) value);
            }
        }

    class ScoreDetails : BlobColumn // size=348 bytes (!)
        {
        }

    class FieldConfigurationDetails : BlobColumn
        {
        }

    class GameSpecifics : BlobColumn
        {
        }
    }
