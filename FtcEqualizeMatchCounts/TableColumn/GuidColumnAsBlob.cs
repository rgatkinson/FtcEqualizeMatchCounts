namespace FEMC
    {
    class GuidColumnAsBlob : GuidColumn
        {
        public override void SetValue(object value)
            {
            this.SetValue((byte[])value);
            }
        }
    }