﻿using System;
using System.Globalization;

namespace FEMC
    {
    abstract class DateTimeColumn : TableColumn
        {
        public System.DateTimeOffset? Value;

        public System.DateTimeOffset? DateTimeOffset => Value?.ToUniversalTime();
        public System.DateTimeOffset DateTimeOffsetNonNull => DateTimeOffset ?? throw MustBeNonNull(GetType().Name);

        public System.DateTime? DateTime => Value?.UtcDateTime.ToUniversalTime();
        public System.DateTime DateTimeNonNull => DateTime ?? throw MustBeNonNull(GetType().Name);

        public System.DateTime? LocalDateTime => DateTime?.ToLocalTime();
        public System.DateTime LocalNonNullDateTime => LocalDateTime ?? throw MustBeNonNull(GetType().Name);

        public override string ToString()
            {
            return Iso8601String ?? "null";
            }

        public void SetValue(System.DateTimeOffset? dateTimeOffset)
            {
            Value = dateTimeOffset;
            }

        public void LoadDatabaseValue(string value)
            {
            SetValue(value == null ? (System.DateTimeOffset?)null : System.DateTimeOffset.Parse(value));
            }

        public void LoadDatabaseValue(long msSince1970UnixEpoch)
            {
            SetValue(System.DateTimeOffset.FromUnixTimeMilliseconds(msSince1970UnixEpoch));
            }

        public override void LoadDatabaseValue(object value)
            {
            if (value == null || value is string)
                LoadDatabaseValue((string)value);
            else 
                LoadDatabaseValue((long)value);
            }

        public long? MsSince1970UnixEpoch => Value?.ToUnixTimeMilliseconds();

        // https://stackoverflow.com/questions/44788305/c-sharp-convert-datetime-object-to-iso-8601-string
        // ISO8601 with 3 decimal places
        public string Iso8601String => DateTime?.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK", CultureInfo.InvariantCulture);
        }
    }