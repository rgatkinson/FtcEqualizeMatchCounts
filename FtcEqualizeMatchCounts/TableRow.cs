using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace FEMC
    {
    abstract class TableRow<PrimaryKey_T>
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

        public abstract PrimaryKey_T PrimaryKey { get; }

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
            Type type = GetType();
            foreach (FieldInfo field in type.GetFields())
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
        }
    }