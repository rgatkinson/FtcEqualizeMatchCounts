﻿namespace FEMC.DAL
    {
    class PlayedMatch : ThisEventMatch
        {
        //----------------------------------------------------------------------------------------
        // Accessing
        //----------------------------------------------------------------------------------------

        public FMSMatchId FmsMatchId;
        public long PlayNumber;

        //----------------------------------------------------------------------------------------
        // Construction
        //----------------------------------------------------------------------------------------

        public PlayedMatch(Database db, DBTables.PlayedMatch.Row row) : base(db, row.FMSEventId, row.FMSScheduleDetailId)
            {
            FmsMatchId = row.FMSMatchId;
            PlayNumber = row.PlayNumber.NonNullValue;
            }
        }
    }
