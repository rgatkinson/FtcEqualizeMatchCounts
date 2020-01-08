using System;
using Microsoft.Data.Sqlite;

namespace FEMC
    {
    class DateTimeAsInteger : DateTimeColumn
        {
        public override object GetDatabaseValue()
            {
            return MsSince1970UnixEpoch;
            }

        public override bool Equals(object obj)
            {
            if (GetType() == obj?.GetType())
                {
                DateTimeAsInteger them = (DateTimeAsInteger)obj;
                return Equals(MsSince1970UnixEpoch, them.MsSince1970UnixEpoch);
                }
            return false;

            }

        public override int GetHashCode()
            {
            return HashCode.Combine(GetType(), MsSince1970UnixEpoch, 0x9081833);
            }


        public static System.DateTimeOffset NegativeOne = System.DateTimeOffset.FromUnixTimeMilliseconds(-1).UtcDateTime;
        }
    }