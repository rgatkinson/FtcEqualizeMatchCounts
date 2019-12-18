namespace FEMC
    {
    class GuidColumnAsString : GuidColumn
        {
        public override void LoadDatabaseValue(object value)
            {
            this.LoadDatabaseValue((string)value);
            }
        }
    }