using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace FtcEqualizeMatchCounts
    {
    class Database : IDisposable
        {
        //---------------------------------------------------------------------------------------------------
        // State
        //---------------------------------------------------------------------------------------------------

        string fileName = null;
        SQLiteConnection connection = null;
        bool disposed = false;

        //---------------------------------------------------------------------------------------------------
        // Construction
        //---------------------------------------------------------------------------------------------------

        public Database(string fileName)
            {
            this.fileName = Path.GetFullPath(fileName);
            
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
            string cs = "URI=file://" + fileName;

            connection = new SQLiteConnection(cs);
            connection.Open();
            }

        public void Close()
            {
            if (connection != null)
                {
                connection.Close();
                connection = null;
                }
            }

        //---------------------------------------------------------------------------------------------------
        // Querying
        //---------------------------------------------------------------------------------------------------

        public List<List<object>> ExecuteQuery(string query)
            {
            using var cmd = new SQLiteCommand(connection);
            cmd.CommandText = query;
            cmd.Prepare();

            List<List<object>> result = new List<List<object>>();
            SQLiteDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
                {
                List<object> row = new List<object>();
                for (int i = 0; i < rdr.FieldCount; i++)
                    {
                    Type type = rdr.GetFieldType(i);
                    object value = null;
                    if (rdr.IsDBNull(i)) { value = null; }
                    else if (type == typeof(Int64))  { value = rdr.GetInt64(i); }
                    else if (type == typeof(Int32))  { value = rdr.GetInt32(i); }
                    else if (type == typeof(Int16))  { value = rdr.GetInt16(i); }
                    else if (type == typeof(bool))   { value = rdr.GetBoolean(i); }
                    else if (type == typeof(char))   { value = rdr.GetChar(i); }
                    else if (type == typeof(string)) { value = rdr.GetString(i); }
                    else if (type == typeof(byte[])) { value = GetBytes(rdr, i); }
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
                    row.Add(value);
                    }
                result.Add(row);
                }

            return result;
            }

        protected byte[] GetBytes(SQLiteDataReader rdr, int i)
            {
            long cb = rdr.GetBytes(i, 0, null, 0, 0);
            byte[] bytes = new byte[cb];
            rdr.GetBytes(i, 0, bytes, 0, bytes.Length);
            return bytes;
            }



        }
    }
