namespace FEMC
    {
    //--------------------------------------------------------------------------------------------------------------------------
    // ScheduleStation
    //--------------------------------------------------------------------------------------------------------------------------

    class ScheduleStation : Table<ScheduleStation.Row>
        {
        public class Row : TableRow // Four records for each match Alliance (1,2) x Station (1,2)
            {
            public FMSScheduleDetailId FMSScheduleDetailId;
            public long Alliance;
            public long Station;
            public FMSEventId FmsEventId;
            public FMSTeamId FmsTeamId;
            public long IsSurrogate;
            public DateTimeAsString CreatedOn; // can be null
            public string CreatedBy; // e.g. "FTC Match Maker"
            public DateTimeAsString ModifedOn;
            public string ModifiedBy;
            }

        public ScheduleStation(Database database) : base(database)
            {
            }

        public override string TableName => "ScheduleStation";
        }
    }