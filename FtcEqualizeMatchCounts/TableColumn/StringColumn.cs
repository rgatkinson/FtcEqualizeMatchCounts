using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

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

        public override void SetValue(object runtimeValue)
            {
            SetValue((string) runtimeValue);
            }

        public override void LoadDatabaseValue(object databaseValue)
            {
            SetValue((string) databaseValue);
            }

        public override object GetDatabaseValue()
            {
            return Value;
            }
        }
    }
