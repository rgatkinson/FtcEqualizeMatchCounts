using FEMC.Enums;

#pragma warning disable 649
namespace FEMC.DBTables
    {
    //--------------------------------------------------------------------------------------------------------------------------
    // ScheduleDetail
    //--------------------------------------------------------------------------------------------------------------------------

    class ScheduleDetail : Table<ScheduleDetail.Row, FMSScheduleDetailId>
        {
        public class Row : TableRow<Row, FMSScheduleDetailId>
            {
            public FMSScheduleDetailId FMSScheduleDetailId; // primary
            public FMSEventId FMSEventId;
            public NullableLong TournamentLevel;    // 2 for quals, 3 for elims, others zero; see also TMatchType
            public NullableLong MatchNumber;        // see quals.Match
            public NullableLong FieldType;          // always 1: see SQLiteManagementDAO.saveFMSSchedule()
            public StringColumn Description;
            public DateTimeAsString StartTime;
            public FieldConfigurationDetails FieldConfigurationDetails; // null is ok
            public DateTimeAsString CreatedOn;    // null is ok
            public StringColumn CreatedBy;        // e.g.: "FTC Match Maker"
            public DateTimeAsString ModifiedOn;   // null is ok
            public StringColumn ModifiedBy;       // null is ok
            public RowVersion RowVersion;

            public override FMSScheduleDetailId PrimaryKey => FMSScheduleDetailId;

            public bool IsEqualizationMatch(Database db) => Equals(CreatedBy.Value, db.EqualizationMatchCreatorName) && MatchNumber.NonNullValue >= db.FirstEqualizationMatchNumber;
            }

        public ScheduleDetail(Database database) : base(database)
            {
            }

        public override string TableName => "ScheduleDetail";
        }
    }