namespace FEMC
    {
    class DateTimeAsInteger : DateTimeColumn
        {
        public override void LoadDatabaseValue(object value)
            {
            LoadDatabaseValue((long)value);
            }
        }
    }