using System;
using System.Globalization;

namespace FEMC
    {
    class DateTimeColumn : TableColumn
        {
        public System.DateTimeOffset? Value;

        public System.DateTime? DateTime => Value?.UtcDateTime.ToUniversalTime();

        public override string ToString()
            {
            // https://stackoverflow.com/questions/44788305/c-sharp-convert-datetime-object-to-iso-8601-string
            // ISO8601 with 3 decimal places
            return DateTime?.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK", CultureInfo.InvariantCulture) ?? "null";
            }

        public void SetValue(System.DateTimeOffset? dateTimeOffset)
            {
            Value = dateTimeOffset;
            }

        public void SetValue(string value)
            {
            SetValue(value == null ? (DateTimeOffset?)null : DateTimeOffset.Parse(value));
            }

        public void SetValue(long msSince1970UnixEpoch)
            {
            SetValue(DateTimeOffset.FromUnixTimeMilliseconds(msSince1970UnixEpoch));
            }
        }
    }