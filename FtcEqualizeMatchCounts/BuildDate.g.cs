﻿using System;

namespace FEMC
	{
    // https://stackoverflow.com/questions/1600962/displaying-the-build-date
    public static partial class Constants
        {
        public static DateTime BuildTimestamp { get 
            { 
            long buildNowUtcTicks = 637122058839301166;
            return new DateTime(buildNowUtcTicks).ToLocalTime();
            } }
        }
	}
