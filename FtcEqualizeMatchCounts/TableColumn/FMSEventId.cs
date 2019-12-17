namespace FEMC
    {
    class FMSEventId : GuidColumnAsBlob
        {
        public FMSEventId()
            {
            }

        public FMSEventId(string value)
            {
            SetValue(value);
            }
        }

    }