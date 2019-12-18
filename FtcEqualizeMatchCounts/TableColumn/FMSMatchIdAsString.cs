using System;

namespace FEMC
    {
    class FMSMatchIdAsString : GuidColumnAsString
        {
        public static FMSMatchIdAsString CreateFrom(GuidColumn t)
            {
            return GuidColumnAsString.CreateFrom<FMSMatchIdAsString>(t);
            }
        }
    }