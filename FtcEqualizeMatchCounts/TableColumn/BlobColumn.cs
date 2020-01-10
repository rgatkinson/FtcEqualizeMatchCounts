using System;
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

        public override void SetRuntimeValue(object runtimeValue)
            {
            SetValue((byte[])runtimeValue);
            }

        public override object GetRuntimeValue()
            {
            return Value;
            }

        public override void SetDatabaseValue(object databaseValue)
            {
            SetValue((byte[])databaseValue);
            }

        public override object GetDatabaseValue()
            {
            return Value;
            }

        public override bool Equals(object obj)
            {
            if (GetType() == obj?.GetType())
                {
                BlobColumn them = (BlobColumn)obj;
                return Equals(Value, them.Value);
                }
            return false;
            }

        public override int GetHashCode()
            {
            return HashCode.Combine(GetType(), Value, 0x387199);
            }
        }
    }