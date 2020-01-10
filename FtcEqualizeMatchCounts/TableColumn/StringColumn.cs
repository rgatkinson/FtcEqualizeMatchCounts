using System;
using System.Diagnostics;

namespace FEMC
    {
    class StringColumn : TableColumn
        {
        public string Value;

        public string NonNullValue
            {
            get { Trace.Assert(Value != null); return Value; }
            }

        public override string ToString()
            {
            return Value ?? "null";
            }

        public override bool Equals(object obj)
            {
            if (GetType() == obj?.GetType())
                {
                StringColumn them = (StringColumn)obj;
                return Value == them.Value;
                }
            return false;
            }

        public override int GetHashCode()
            {
            return HashCode.Combine(GetType(), Value, 0x9083);
            }

        public void SetValue(string value)
            {
            Value = value;
            }

        public override void SetRuntimeValue(object runtimeValue)
            {
            SetValue((string) runtimeValue);
            }

        public override object GetRuntimeValue()
            {
            return Value;
            }

        public override void SetDatabaseValue(object databaseValue)
            {
            SetValue((string) databaseValue);
            }

        public override object GetDatabaseValue()
            {
            return Value;
            }
        }
    }
