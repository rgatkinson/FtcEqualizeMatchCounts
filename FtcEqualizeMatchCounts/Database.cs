using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Data.Sqlite;

namespace FEMC
    {
    class Database : IDisposable
        {
        //---------------------------------------------------------------------------------------------------
        // State
        //---------------------------------------------------------------------------------------------------

        public SqliteConnection Connection = null;
        public Tables Tables;

        string fileName = null;
        bool disposed = false;

        //---------------------------------------------------------------------------------------------------
        // Construction
        //---------------------------------------------------------------------------------------------------

        public Database(string fileName)
            {
            this.fileName = Path.GetFullPath(fileName);
            this.Tables = new Tables(this);
            
            Open();
            }

        ~Database()
            {
            Dispose(false);
            }

        public void Dispose() // called by consumers
            {
            Dispose(true);
            GC.SuppressFinalize(this);
            }

        protected virtual void Dispose(bool explicitlyDisposed)
            {
            if (!disposed)
                { 
                if (explicitlyDisposed)
                    {
                    // Free managed objects
                    Close();
                    }
                // Free native objects
                disposed = true;
                }
            }

        //---------------------------------------------------------------------------------------------------
        // Opening and closing
        //---------------------------------------------------------------------------------------------------

        public void Open()
            {
            Close();

            string uri = new System.Uri(fileName).AbsoluteUri;
            string cs = "Filename=" + fileName;

            Connection = new SqliteConnection(cs);
            Connection.Open();
            }

        public void Close()
            {
            if (Connection != null)
                {
                Connection.Close();
                Connection = null;
                }
            }

        //---------------------------------------------------------------------------------------------------
        // Querying
        //---------------------------------------------------------------------------------------------------

        public void Load()
            {
            Tables.Load();
            }

        // See
        //  https://www.bricelam.net/2018/05/24/microsoft-data-sqlite-2-1.html#comment-3980760585
        //  https://stackoverflow.com/questions/51933421/system-data-sqlite-vs-microsoft-data-sqlite
        //
        // "we embrace the fact that SQLite only supports four primitive types (INTEGER, REAL, TEXT, and BLOB)
        // and implement ADO.NET APIs in a way that helps you coerce values between these and .NET types"
        // 
        public List<List<object>> ExecuteQuery(string query)
            {
            using var cmd = Connection.CreateCommand();
            cmd.CommandText = query;

            List<List<object>> result = new List<List<object>>();
            SqliteDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
                {
                List<object> row = new List<object>();
                for (int i = 0; i < rdr.FieldCount; i++)
                    {
                    Type type = rdr.GetFieldType(i);
                    object value = null;
                    if (rdr.IsDBNull(i)) { value = null; }
                    else if (type == typeof(Int64))  { value = rdr.GetInt64(i); }
                    else if (type == typeof(Double)) { value = rdr.GetDouble(i); }
                    else if (type == typeof(string)) { value = rdr.GetString(i); }
                    else if (type == typeof(byte[])) { value = GetBytes(rdr, i); }
                    //
                    else { // not used in Microsoft.Data.Sqllite
                             if (type == typeof(Int32))  { value = rdr.GetInt32(i); }
                        else if (type == typeof(Int16))  { value = rdr.GetInt16(i); }
                        else if (type == typeof(bool))   { value = rdr.GetBoolean(i); }
                        else if (type == typeof(char))   { value = rdr.GetChar(i); }
                        else
                            {
                            try { 
                                value = GetBytes(rdr, i);
                                }
                            catch (Exception e)
                                {
                                Console.Out.WriteLine($"exception: {e}");
                                }
                            Console.Error.WriteLine($"unhandled type: {type}");
                            }
                        }
                    row.Add(value);
                    }
                result.Add(row);
                }

            return result;
            }

        protected byte[] GetBytes(SqliteDataReader rdr, int i)
            {
            long cb = rdr.GetBytes(i, 0, null, 0, 0);
            byte[] bytes = new byte[cb];
            rdr.GetBytes(i, 0, bytes, 0, bytes.Length);
            return bytes;
            }



        }
    }
