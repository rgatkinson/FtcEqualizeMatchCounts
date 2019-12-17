namespace FEMC.DAL
    {
    class PlayedMatch : ThisEventMatch
        {
        //----------------------------------------------------------------------------------------
        // Accessing
        //----------------------------------------------------------------------------------------

        public FMSMatchId FmsMatchId;
        public long PlayNumber;
        private long redScore;
        private long redPenalty;
        private long blueScore;
        private long bluePenalty;

        //----------------------------------------------------------------------------------------
        // Construction
        //----------------------------------------------------------------------------------------

        public PlayedMatch(Database db, DBTables.PlayedMatch.Row row) : base(db, row.FMSEventId, row.FMSScheduleDetailId)
            {
            FmsMatchId = row.FMSMatchId;
            PlayNumber = row.PlayNumber.NonNullValue;
            redScore = row.RedScore.NonNullValue;
            redPenalty = row.RedPenalty.NonNullValue;
            blueScore = row.BlueScore.NonNullValue;
            bluePenalty = row.BluePenalty.NonNullValue;
            }
        }
    }
