using FEMC.Enums;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FEMC
    {
    class TableRow<TRow, TPrimaryKey> where TRow : TableRow<TRow, TPrimaryKey>, new()
        {
        //----------------------------------------------------------------------------------------------------------------------
        // State
        //----------------------------------------------------------------------------------------------------------------------

        public AbstractTable<TableRow<TRow, TPrimaryKey>> Table = null;
        public List<FieldInfo> LocalStoredFields => localStoredFields;

        private List<FieldInfo> localStoredFields;

        //----------------------------------------------------------------------------------------------------------------------
        // Construction
        //----------------------------------------------------------------------------------------------------------------------

        protected TableRow()
            {
            localStoredFields = CalcLocalStoredFields;
            }

        protected List<FieldInfo> CalcLocalStoredFields
            {
            get
                {
                Type type = GetType();
                List<FieldInfo> result = new List<FieldInfo>();
                foreach (FieldInfo field in type.GetFields())
                    {
                    if (field.DeclaringType == type)
                        {
                        if (field.IsPublic)
                            {
                            result.Add(field);
                            }
                        }
                    }
                return result;
                }
            }

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

        public virtual TPrimaryKey PrimaryKey => throw new NotImplementedException();

        //----------------------------------------------------------------------------------------------------------------------
        // SQL
        //----------------------------------------------------------------------------------------------------------------------

        public List<FieldInfo> Columns(IEnumerable<string> fieldNames)
            {
            List<FieldInfo> result = new List<FieldInfo>();
            foreach (string name in fieldNames)
                {
                FieldInfo field = null;
                foreach (var candidate in LocalStoredFields)
                    {
                    if (candidate.Name == name)
                        {
                        field = candidate;
                        break;
                        }
                    }
                result.Add(field);
                }
            return result;
            }

        public List<(FieldInfo, SqlOperator, object)> Where(string fieldName, object value)
            {
            return Where(new string[] { fieldName }, new SqlOperator[] { SqlOperator.EQUAL }, new object[] { value });
            }

        public List<(FieldInfo, SqlOperator, object)> Where(IEnumerable<string> fieldNames, IEnumerable<SqlOperator> operators, IEnumerable<object> values)
            {
            List<(FieldInfo, SqlOperator, object)> result = new List<(FieldInfo, SqlOperator, object)>();
            var fieldInfos = Columns(fieldNames);
            foreach (var pair in MiscUtil.Zip(fieldInfos, operators, values, (f,o,v) => new { field = f, op = o, value = v }))
                {
                result.Add((pair.field, pair.op, pair.value));
                }
            return result;
            }

        public List<(FieldInfo, SqlOperator, object)> Where(IEnumerable<(string, SqlOperator, object)> fieldNamesAndValues)
            {
            // ReSharper disable once PossibleMultipleEnumeration
            IEnumerable<string> fieldNames = fieldNamesAndValues.Select((pair) => pair.Item1);
            // ReSharper disable once PossibleMultipleEnumeration
            IEnumerable<SqlOperator> operators = fieldNamesAndValues.Select((pair) => pair.Item2);
            // ReSharper disable once PossibleMultipleEnumeration
            IEnumerable<object> values = fieldNamesAndValues.Select((pair) => pair.Item3);
            return Where(fieldNames, operators, values);
            }

        // See
        //  https://www.bricelam.net/2018/05/24/microsoft-data-sqlite-2-1.html#comment-3980760585
        //  https://stackoverflow.com/questions/51933421/system-data-sqlite-vs-microsoft-data-sqlite
        //
        // "we embrace the fact that SQLite only supports four primitive types (INTEGER, REAL, TEXT, and BLOB)
        // and implement ADO.NET APIs in a way that helps you coerce values between these and .NET types"
        // 
        public void SetFieldFromDatabase(int index, object databaseValue)
            {
            FieldInfo field = LocalStoredFields[index];

            if (field.FieldType == typeof(string))
                {
                field.SetValue(this, databaseValue);
                }
            else if (field.FieldType == typeof(long))
                {
                field.SetValue(this, databaseValue);
                }
            else if (field.FieldType == typeof(double))
                {
                field.SetValue(this, databaseValue);
                }
            else if (field.FieldType.IsSubclassOf(typeof(TableColumn)))
                {
                TableColumn column = (TableColumn)Activator.CreateInstance(field.FieldType);
                column.LoadDatabaseValue(databaseValue);
                field.SetValue(this, column);
                }
            else
                {
                throw new NotImplementedException($"FileType={field.FieldType} not yet implemented");
                }
            }

        public object AsDatabaseValue(FieldInfo field, object value)
            {
            if (field.FieldType == typeof(string))
                {
                return value;
                }
            else if (field.FieldType == typeof(long))
                {
                return value;
                }
            else if (field.FieldType == typeof(double))
                {
                return value;
                }
            else if (field.FieldType.IsSubclassOf(typeof(TableColumn)))
                {
                TableColumn column = (TableColumn)Activator.CreateInstance(field.FieldType);
                column.SetValue(value);
                return column.GetDatabaseValue();
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

        public void Delete(IEnumerable<(FieldInfo, SqlOperator, object)> where)
            {
            var whereArray = where.ToArray();
            using var cmd = Table.Database.Connection.CreateCommand();

            StringBuilder builder = new StringBuilder();
            builder.Append($"DELETE FROM { Table.TableName } WHERE ");

            AppendWhere(builder, cmd, whereArray);
            builder.Append(";");

            cmd.CommandText = builder.ToString();
            var cRows = cmd.ExecuteNonQuery();
            }

        protected void AppendWhere(StringBuilder builder, SqliteCommand cmd, IEnumerable<(FieldInfo, SqlOperator, object)> whereArray)
            {
            bool first = true;
            foreach (var term in whereArray)
                {
                SqliteParameter parameter = NewParameter(cmd);
                if (!first)
                    {
                    builder.Append(" AND ");
                    }
                FieldInfo field = term.Item1;
                builder.Append($"{GetColumnName(field)}{term.Item2.GetStringValue()}{parameter.ParameterName}");

                object databaseValue = AsDatabaseValue(field, term.Item3);
                TableColumn.SetParameterValue(parameter, databaseValue);

                first = false;
                }
            }

        protected SqliteParameter NewParameter(SqliteCommand cmd)
            {
            SqliteParameter parameter = cmd.CreateParameter();
            parameter.ParameterName = $"$param{cmd.Parameters.Count}";
            parameter.IsNullable = true; // everything is nullable in our schema
            cmd.Parameters.Add(parameter);
            return parameter;
            }

        public void Update(IEnumerable<FieldInfo> columns, IEnumerable<(FieldInfo, SqlOperator, object)> where)
            {
            FieldInfo[] columnsArray = columns.ToArray();
            var whereArray = where.ToArray();
            using var cmd = Table.Database.Connection.CreateCommand();

            StringBuilder builder = new StringBuilder();
            builder.Append($"UPDATE { Table.TableName } SET ");

            bool first = true;
            foreach (FieldInfo field in columnsArray)
                {
                SqliteParameter parameter = NewParameter(cmd);
                if (!first)
                    {
                    builder.Append(", ");
                    }
                builder.Append($"{GetColumnName(field)}={parameter.ParameterName}");
                SetParameterValue(parameter, field);
                first = false;
                }

            builder.Append(" WHERE ");
            AppendWhere(builder, cmd, whereArray);
            builder.Append(";");

            cmd.CommandText = builder.ToString();
            var cRows = cmd.ExecuteNonQuery();
            if (cRows != 1)
                {
                throw new UnexpectedRowCountException("UPDATE", Table.TableName, 1, cRows);
                }
            }

        public void SaveToDatabase()
            {
            using var cmd = Table.Database.Connection.CreateCommand();

            StringBuilder builder = new StringBuilder();
            builder.Append($"INSERT INTO { Table.TableName } VALUES (");

            bool first = true;
            foreach (FieldInfo field in LocalStoredFields)
                {
                SqliteParameter parameter = NewParameter(cmd);
                if (!first)
                    {
                    builder.Append(", ");
                    }
                builder.Append(parameter.ParameterName);
                SetParameterValue(parameter, field);
                first = false;
                }

            builder.Append(");");

            cmd.CommandText = builder.ToString();
            var cRows = cmd.ExecuteNonQuery();
            if (cRows != 1)
                {
                throw new UnexpectedRowCountException("INSERT", Table.TableName, 1, cRows);
                }
            }

        public void AddToTable()
            {
            Table.AddRow(this);
            }

        protected void SetParameterValue(SqliteParameter parameter, FieldInfo field)
            {
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
            }

        protected string GetColumnName(FieldInfo fieldInfo)
            {
            string result = fieldInfo.GetColumnName();
            if (result == null)
                {
                int columnNumberZ = 0;
                foreach (var f in LocalStoredFields)
                    {
                    if (f == fieldInfo)
                        {
                        result = Table.ColumnNames[columnNumberZ];
                        break;
                        }
                    columnNumberZ++;
                    }
                }
            return result;
            }

        public void AddToTableAndSave()
            {
            AddToTable();
            SaveToDatabase();
            }

        }
    }