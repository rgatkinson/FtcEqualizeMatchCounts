namespace FEMC.DBTables
    {
    //--------------------------------------------------------------------------------------------------------------------------
    // ScheduleDetail
    //--------------------------------------------------------------------------------------------------------------------------

    class ScheduleDetail : Table<ScheduleDetail.Row, FMSScheduleDetailId>
        {
        public class Row : TableRow<FMSScheduleDetailId>
            {
            public FMSScheduleDetailId FMSScheduleDetailId; // primary
            public FMSEventId FMSEventId;
            public LongColumn TournamentLevel;    // 2 for quals?
            public LongColumn MatchNumber;        // see quals.Match
            public LongColumn FieldType;          // 1 for everything we've seen
            public StringColumn Description;
            public DateTimeAsString StartTime;
            public FieldConfigurationDetails FieldConfigurationDetails;
            public DateTimeAsString CreatedOn;    // null is ok
            public StringColumn CreatedBy;              // e.g.: "FTC Match Maker"
            public DateTimeAsString ModifiedOn;   // null is ok
            public StringColumn ModifiedBy;
            public RowVersion RowVersion;

            public override FMSScheduleDetailId PrimaryKey => FMSScheduleDetailId;
            }

        public ScheduleDetail(Database database) : base(database)
            {
            }

        public override string TableName => "ScheduleDetail";
        }
    }