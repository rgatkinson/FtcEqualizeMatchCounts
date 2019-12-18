﻿namespace FEMC
    {
    class DateTimeAsString : DateTimeColumn // Nullable always?
        {
        // e.g.:
        //  2019-12-15T00:41:31.957Z
        //  2019-12-15T00:40:28.120Z
        public override void LoadDatabaseValue(object value)
            {
            LoadDatabaseValue((string)value);
            }
        }
    }