namespace FEMC
    {
    class GuidColumnAsBlob : GuidColumn
        {
        public override void LoadDatabaseValue(object value)
            {
            this.LoadDatabaseValue((byte[])value);
            }
        }
    }