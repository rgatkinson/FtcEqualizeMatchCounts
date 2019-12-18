using Microsoft.Data.Sqlite;

namespace FEMC
    {
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

        public override void LoadDatabaseValue(object value)
            {
            SetValue((byte[])value);
            }

        public override void SaveDatabaseValue(SqliteParameter parameter)
            {
            SetParameterValue(parameter, Value);
            }
        }
    }