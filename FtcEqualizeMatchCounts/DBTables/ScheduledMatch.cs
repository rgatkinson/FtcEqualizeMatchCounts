#pragma warning disable 649
namespace FEMC.DBTables
    {
    //--------------------------------------------------------------------------------------------------------------------------
    // ScheduleDetail
    //--------------------------------------------------------------------------------------------------------------------------

    class ScheduledMatch : Table<ScheduledMatch.Row, FMSScheduleDetailId>
        {
        public class Row : TableRow<FMSScheduleDetailId>
            {
            public FMSScheduleDetailId FMSScheduleDetailId; // primary
            public FMSEventId FMSEventId;
            public NullableLong TournamentLevel;    // 2 for quals?
            public NullableLong MatchNumber;        // see quals.Match
            public NullableLong FieldType;          // 1 for everything we've seen
            public StringColumn Description;
            public DateTimeAsString StartTime;
            public FieldConfigurationDetails FieldConfigurationDetails;
            public DateTimeAsString CreatedOn;    // null is ok
            public StringColumn CreatedBy;        // e.g.: "FTC Match Maker"
            public DateTimeAsString ModifiedOn;   // null is ok
            public StringColumn ModifiedBy;
            public RowVersion RowVersion;

            public override FMSScheduleDetailId PrimaryKey => FMSScheduleDetailId;
            }

        public ScheduledMatch(Database database) : base(database)
            {
            }

        public override string TableName => "ScheduleDetail";
        }
    }