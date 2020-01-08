using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace FEMC
    {
    class NullableColumn<T> : TableColumn where T : struct, IEquatable<T>
        {
        public T? Value;

        public T NonNullValue => Value.Value;

        public override string ToString()
            {
            return Value?.ToString() ?? "null";
            }

        public override bool Equals(object obj)
            {
            if (GetType() == obj?.GetType())
                {
                NullableColumn<T> them = (NullableColumn<T>)obj;
                return Value.HasValue && them.Value.HasValue && Value.Value.Equals(them.Value.Value) || !Value.HasValue && !them.Value.HasValue;
                }
            return false;
            }

        public override int GetHashCode()
            {
            return HashCode.Combine(GetType(), Value, 0x83791);
            }

        public void LoadDatabaseValue(T? value)
            {
            Value = value;
            }

        public override void LoadDatabaseValue(object databaseValue)
            {
            LoadDatabaseValue((T?)databaseValue);
            }

        public override object GetDatabaseValue()
            {
            return Value;
            }

        public void SetValue(T? t)
            {
            Value = t;
            }

        public override void SetValue(object runtimeValue)
            {
            SetValue((T?)runtimeValue);
            }
        }
    }
