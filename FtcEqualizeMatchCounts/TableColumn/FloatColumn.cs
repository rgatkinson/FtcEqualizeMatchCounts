using System;
using System.Diagnostics;

namespace FEMC
    {
    class FloatColumn : TableColumn
        {
        public double? Value;

        public double NonNullValue
            {
            get { Trace.Assert(Value != null); return Value.Value; }
            }

        public override string ToString()
            {
            return Value?.ToString() ?? "null";
            }

        public override bool Equals(object obj)
            {
            if (GetType() == obj?.GetType())
                {
                FloatColumn them = (FloatColumn)obj;
                return Equals(Value, them.Value);
                }
            return false;
            }

        public override int GetHashCode()
            {
            return HashCode.Combine(GetType(), Value, 0x909383);
            }

        public void SetValue(double? value)
            {
            Value = value;
            }

        public void SetValue(string value)
            {
            SetValue(value != null ? (double?)double.Parse(value) : (double?)null);
            }

        public override void SetValue(object runtimeValue)
            {
            SetValue((double?) runtimeValue);
            }

        public override void LoadDatabaseValue(object databaseValue)
            {
            SetValue((string?) databaseValue);
            }

        public override object GetDatabaseValue()
            {
            return Value?.ToString();
            }
        }
    }
