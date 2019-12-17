namespace FEMC.DAL
    {
    class PlayedMatch : DBObject
        {
        public FMSMatchId FmsMatchId;
        public FMSScheduleDetailId FMSScheduleDetailId;

        public ScheduledMatch ScheduledMatch => Database.ScheduledMatchesById[FMSScheduleDetailId];

        public PlayedMatch(Database db, DBTables.PlayedMatch.Row row) : base(db)
            {
            FmsMatchId = row.FMSMatchId;
            FMSScheduleDetailId = row.FMSScheduleDetailId;
            }
        }
    }
