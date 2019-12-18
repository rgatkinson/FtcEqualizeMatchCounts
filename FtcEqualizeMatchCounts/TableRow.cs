using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Microsoft.Data.Sqlite;

namespace FEMC
    {
    class TableRow<TPrimaryKey>
        {
        //----------------------------------------------------------------------------------------------------------------------
        // Accessing
        //----------------------------------------------------------------------------------------------------------------------

        public AbstractTable<TableRow<TPrimaryKey>> Table = null;

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

        public virtual TPrimaryKey PrimaryKey => throw new NotImplementedException();

        protected List<FieldInfo> LocalStoredFields
            {
            get {
                Type type = GetType();
                List<FieldInfo> result = new List<FieldInfo>();
                foreach (FieldInfo field in type.GetFields())
                    {
                    if (field.DeclaringType == type)
                        {
                        result.Add(field);
                        }
                    }
                return result;
                }
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
            FieldInfo field = LocalStoredFields[index];
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
                column.LoadDatabaseValue(value);
                field.SetValue(this, column);
                }
            else
                {
                throw new NotImplementedException($"FileType={field.FieldType} not yet implemented");
                }
            }

        public void InitializeFields()
            {
            foreach (FieldInfo field in LocalStoredFields)
                {
                if (field.FieldType == typeof(string))
                    {
                    field.SetValue(this, null);
                    }
                else if (field.FieldType == typeof(long))
                    {
                    field.SetValue(this, 0);
                    }
                else if (field.FieldType == typeof(double))
                    {
                    field.SetValue(this, 0.0);
                    }
                else if (field.FieldType.IsSubclassOf(typeof(TableColumn)))
                    {
                    TableColumn column = (TableColumn)Activator.CreateInstance(field.FieldType);
                    field.SetValue(this, column);
                    }
                }
            }

        public void SaveToDatabase()
            {
            using var cmd = Table.Database.Connection.CreateCommand();

            StringBuilder builder = new StringBuilder();
            builder.Append($"INSERT INTO { Table.TableName } VALUES (");

            int iField = 0;
            foreach (FieldInfo field in LocalStoredFields)
                {
                SqliteParameter parameter = cmd.CreateParameter();
                parameter.ParameterName = $"$param{iField}";
                parameter.IsNullable = true; // everything is nullable in our schema
                cmd.Parameters.Add(parameter);

                if (iField > 0)
                    {
                    builder.Append(", ");
                    }
                builder.Append(parameter.ParameterName);

                if (field.FieldType == typeof(string))
                    {
                    parameter.Value = field.GetValue(this);
                    }
                else if (field.FieldType == typeof(long))
                    {
                    parameter.Value = field.GetValue(this);
                    }
                else if (field.FieldType == typeof(double))
                    {
                    parameter.Value = field.GetValue(this);
                    }
                else if (field.FieldType.IsSubclassOf(typeof(TableColumn)))
                    {
                    TableColumn column = (TableColumn)field.GetValue(this);
                    column.SaveDatabaseValue(parameter);
                    }

                iField++;
                }

            builder.Append(");");

            cmd.CommandText = builder.ToString();
            var cRows = cmd.ExecuteNonQuery();
            if (cRows != 1)
                {
                Table.ProgramOptions.StdErr.WriteLine($"INSERT expected to affect 1 row; affected {cRows} rows");
                }
            }
        }
    }