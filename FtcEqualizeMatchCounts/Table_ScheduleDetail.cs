namespace FEMC
    {
    //--------------------------------------------------------------------------------------------------------------------------
    // ScheduleDetail
    //--------------------------------------------------------------------------------------------------------------------------

    class Table_ScheduleDetail : Table<Table_ScheduleDetail.Row>
        {
        public class Row : TableRow
            {
            public FMSScheduleDetailId FMSScheduleDetailId;
            public FMSEventId FMSEventId;
            public long TournamentLevel;    // 2 for quals?
            public long MatchNumber;        // see quals.Match
            public long FieldType;          // 1 for everything we've seen
            public string Description;
            public DateTimeAsString StartTime;
            public FieldConfigurationDetails FieldConfigurationDetails;
            public DateTimeAsString CreatedOn;    // null is ok
            public string CreatedBy;            // e.g.: "FTC Match Maker"
            public DateTimeAsString ModifiedOn;   // null is ok
            public string ModifiedBy;
            public RowVersion RowVersion;
            }

        public Table_ScheduleDetail(Database database) : base(database)
            {
            }

        public override string TableName => "ScheduleDetail";
        }
    }