#pragma warning disable 649
using System;

namespace FEMC.DBTables
    {
    //--------------------------------------------------------------------------------------------------------------------------
    // ScheduleStation
    //--------------------------------------------------------------------------------------------------------------------------

    class ScheduleStation : Table<ScheduleStation.Row, (FMSScheduleDetailId, long, long)>
        {
        // Four records for each match Alliance (1,2) x Station (1,2)
        public class Row : TableRow<Row, (FMSScheduleDetailId, long, long)>
            {
            public FMSScheduleDetailId FMSScheduleDetailId;  // primary
            public NullableLong Alliance; // primary
            public NullableLong Station; // primary
            public FMSEventId FmsEventId;
            public FMSTeamId FmsTeamId;
            public BooleanAsInteger IsSurrogate;
            public DateTimeAsString CreatedOn; // can be null
            public StringColumn CreatedBy; // e.g. "FTC Match Maker"
            public DateTimeAsString ModifedOn;
            public StringColumn ModifiedBy;

            public override (FMSScheduleDetailId, long, long) PrimaryKey => (FMSScheduleDetailId, Alliance.NonNullValue, Station.NonNullValue);
            }

        public ScheduleStation(Database database) : base(database)
            {
            }

        public override string TableName => "ScheduleStation";
        }
    }