namespace FEMC
    {
    class DateTimeAsInteger : DateTimeColumn
        {
        public override void SetValue(object value)
            {
            SetValue((long)value);
            }
        }
    }