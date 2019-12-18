using System;
using System.Globalization;

namespace FEMC
    {
    abstract class DateTimeColumn : TableColumn
        {
        public System.DateTimeOffset? Value;

        public System.DateTime? DateTime => Value?.UtcDateTime.ToUniversalTime();
        public System.DateTime? LocalDateTime => DateTime?.ToLocalTime();

        public override string ToString()
            {
            return Iso861String;
            }

        public void SetValue(System.DateTimeOffset? dateTimeOffset)
            {
            Value = dateTimeOffset;
            }

        public void LoadDatabaseValue(string value)
            {
            SetValue(value == null ? (DateTimeOffset?)null : DateTimeOffset.Parse(value));
            }

        public void LoadDatabaseValue(long msSince1970UnixEpoch)
            {
            SetValue(DateTimeOffset.FromUnixTimeMilliseconds(msSince1970UnixEpoch));
            }

        public long? MsSince1970UnixEpoch => Value?.ToUnixTimeMilliseconds();

        // https://stackoverflow.com/questions/44788305/c-sharp-convert-datetime-object-to-iso-8601-string
        // ISO8601 with 3 decimal places
        public string Iso861String => DateTime?.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK", CultureInfo.InvariantCulture) ?? "null";
        }
    }