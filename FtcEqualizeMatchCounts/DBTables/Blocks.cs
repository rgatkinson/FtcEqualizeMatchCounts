using System;

#pragma warning disable 649

namespace FEMC.DBTables
    {
    //--------------------------------------------------------------------------------------------------------------------------
    // Match Schedule
    //--------------------------------------------------------------------------------------------------------------------------

    class Blocks : Table<Blocks.Row, DateTimeOffset>
        {
        public class Row : TableRow<Row, DateTimeOffset>
            {
            public DateTimeAsInteger Start;     // primary
            public NullableLong      Type;      // see TMatchScheduleType: 0 == Qualification, 2 == 5 minute break for consecutive match 
            public NullableLong      Duration;  // 0 for matches, minutes for admin breaks
            public NullableLong      Count;
            public StringColumn      Label;

            public override DateTimeOffset PrimaryKey => Start.DateTimeNonNull;
            }

        public Blocks(Database database) : base(database)
            {
            }

        public override string TableName => "blocks";
        }
    }