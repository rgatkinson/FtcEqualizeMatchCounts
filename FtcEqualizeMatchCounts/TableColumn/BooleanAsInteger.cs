using System;
using Microsoft.Data.Sqlite;

namespace FEMC
    {
    class BooleanAsInteger : TableColumn // boolean stored as 'integer' in schema instead of 'boolean'
        {
        public bool? Value;

        public bool NonNullValue => Value.Value;

        public override string ToString()
            {
            return Value?.ToString() ?? "null";
            }

        public void SetValue(bool? value)
            {
            Value = value;
            }

        public override void SetValue(object runtimeValue)
            {
            SetValue((bool?)runtimeValue);
            }

        public void LoadDatabaseValue(long? value)
            {
            SetValue(value==null ? (bool?)null : value.Value != 0);
            }

        public override void LoadDatabaseValue(object databaseValue)
            {
            LoadDatabaseValue((long?)databaseValue);
            }

        public override object GetDatabaseValue()
            {
            return Value;
            }

        public override bool Equals(object obj)
            {
            if (GetType() == obj?.GetType())
                {
                BooleanAsInteger them = (BooleanAsInteger)obj;
                return Equals(Value, them.Value);
                }
            return false;

            }

        public override int GetHashCode()
            {
            return HashCode.Combine(GetType(), Value, 0x9083183);
            }

        }
    }