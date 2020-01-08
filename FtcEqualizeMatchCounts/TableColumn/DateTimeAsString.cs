using System;
using Microsoft.Data.Sqlite;

namespace FEMC
    {
    class DateTimeAsString : DateTimeColumn
        {
        public override object GetDatabaseValue()
            {
            return Iso8601String;
            }

        public override bool Equals(object obj)
            {
            if (GetType() == obj?.GetType())
                {
                DateTimeAsString them = (DateTimeAsString)obj;
                return Equals(Iso8601String, them.Iso8601String);
                }
            return false;
            }

        public override int GetHashCode()
            {
            return HashCode.Combine(GetType(), Iso8601String, 0xacdef38971);
            }

        }
    }