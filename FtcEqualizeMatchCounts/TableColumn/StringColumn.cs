using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public override void SetValue(object value)
            {
            SetValue((string) value);
            }
        }
    }
