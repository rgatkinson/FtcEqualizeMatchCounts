namespace FEMC
    {
    class FMSScheduleDetailIdAsString : GuidColumnAsString
        {
        public static FMSScheduleDetailIdAsString CreateFrom(GuidColumn t)
            {
            return GuidColumnAsString.CreateFrom<FMSScheduleDetailIdAsString>(t);
            }
        }
    }