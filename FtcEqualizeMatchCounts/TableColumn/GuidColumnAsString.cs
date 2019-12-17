namespace FEMC
    {
    class GuidColumnAsString : GuidColumn
        {
        public override void SetValue(object value)
            {
            this.SetValue((string)value);
            }
        }
    }