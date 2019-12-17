using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEMC
    {
    class LongColumn : TableColumn
        {
        public long? Value;

        public override string ToString()
            {
            return Value?.ToString() ?? "null";
            }

        public override bool Equals(object obj)
            {
            if (GetType() == obj?.GetType())
                {
                LongColumn them = (LongColumn)obj;
                return Value == them.Value;
                }
            return false;
            }

        public override int GetHashCode()
            {
            return HashCode.Combine(GetType(), Value, 0x83791);
            }

        public void SetValue(long? value)
            {
            Value = value;
            }

        public override void SetValue(object value)
            {
            SetValue((long?) value);
            }
        }
    }
