using System;

namespace FEMC.DBTables
    {
    //--------------------------------------------------------------------------------------------------------------------------
    // ScheduleStation
    //--------------------------------------------------------------------------------------------------------------------------

    class ScheduledMatchStation : Table<ScheduledMatchStation.Row, Tuple<FMSScheduleDetailId, LongColumn, LongColumn>>
        {
        // Four records for each match Alliance (1,2) x Station (1,2)
        public class Row : TableRow<Tuple<FMSScheduleDetailId, LongColumn, LongColumn>>
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

            public override Tuple<FMSScheduleDetailId, LongColumn, LongColumn> PrimaryKey => new Tuple<FMSScheduleDetailId, LongColumn, LongColumn>(FMSScheduleDetailId, Alliance, Station);
            }

        public ScheduledMatchStation(Database database) : base(database)
            {
            }

        public override string TableName => "ScheduleStation";
        }
    }