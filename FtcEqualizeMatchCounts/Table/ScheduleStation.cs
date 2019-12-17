namespace FEMC.DBTables
    {
    //--------------------------------------------------------------------------------------------------------------------------
    // ScheduleStation
    //--------------------------------------------------------------------------------------------------------------------------

    class ScheduleStation : Table<ScheduleStation.Row>
        {
        public class Row : TableRow // Four records for each match Alliance (1,2) x Station (1,2)
            {
            public FMSScheduleDetailId FMSScheduleDetailId;  // primary
            public LongColumn Alliance; // primary
            public LongColumn Station; // primary
            public FMSEventId FmsEventId;
            public FMSTeamId FmsTeamId;
            public LongColumn IsSurrogate;
            public DateTimeAsString CreatedOn; // can be null
            public StringColumn CreatedBy; // e.g. "FTC Match Maker"
            public DateTimeAsString ModifedOn;
            public StringColumn ModifiedBy;
            }

        public ScheduleStation(Database database) : base(database)
            {
            }

        public override string TableName => "ScheduleStation";
        }
    }