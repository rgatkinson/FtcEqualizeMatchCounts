using System;

namespace FEMC.DBTables
    {
    //--------------------------------------------------------------------------------------------------------------------------
    // ScheduleStation
    //--------------------------------------------------------------------------------------------------------------------------

    class ScheduledMatchStation : Table<ScheduledMatchStation.Row, Tuple<FMSScheduleDetailId, NullableLong, NullableLong>>
        {
        // Four records for each match Alliance (1,2) x Station (1,2)
        public class Row : TableRow<Tuple<FMSScheduleDetailId, NullableLong, NullableLong>>
            {
            public FMSScheduleDetailId FMSScheduleDetailId;  // primary
            public NullableLong Alliance; // primary
            public NullableLong Station; // primary
            public FMSEventId FmsEventId;
            public FMSTeamId FmsTeamId;
            public NullableLong IsSurrogate;
            public DateTimeAsString CreatedOn; // can be null
            public StringColumn CreatedBy; // e.g. "FTC Match Maker"
            public DateTimeAsString ModifedOn;
            public StringColumn ModifiedBy;

            public override Tuple<FMSScheduleDetailId, NullableLong, NullableLong> PrimaryKey => new Tuple<FMSScheduleDetailId, NullableLong, NullableLong>(FMSScheduleDetailId, Alliance, Station);
            }

        public ScheduledMatchStation(Database database) : base(database)
            {
            }

        public override string TableName => "ScheduleStation";
        }
    }