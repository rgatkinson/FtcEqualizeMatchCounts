using System;

#pragma warning disable 649

namespace FEMC.DBTables
    {
    //--------------------------------------------------------------------------------------------------------------------------
    // Match Schedule
    //--------------------------------------------------------------------------------------------------------------------------

    class MatchSchedule : Table<MatchSchedule.Row, DateTimeOffset>
        {
        public class Row : TableRow<Row, DateTimeOffset>
            {
            public DateTimeAsInteger Start; // primary
            public DateTimeAsInteger End;
            public NullableLong      Type;      // see TMatchScheduleType: 0 == Qualification, 2 == 5 minute break for consecutive match 
            public StringColumn      Label;

            public override DateTimeOffset PrimaryKey => Start.DateTimeOffsetNonNull;
            }

        public MatchSchedule(Database database) : base(database)
            {
            }

        public override string TableName => "matchSchedule";
        }
    }