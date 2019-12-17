namespace FtcEqualizeMatchCounts
    {
    //--------------------------------------------------------------------------------------------------------------------------
    // Quals
    //--------------------------------------------------------------------------------------------------------------------------

    class Table_qualsDatq : Table
        {
        public class Row : TableRow
            {
            public long Match; // small integer match number
            public long Status;
            public long Randomization;
            public DateTimeAsString Start;      // format?
            public DateTimeAsString ScheduleStart;
            public DateTimeAsString PostedTime;
            public FMSMatchIdAsString FMSMatchId;
            public FMSScheduleDetailIdAsString FMSScheduleDetailId;
            }

        public Table_qualsDatq(Database database) : base(database)
            {
            }

        public override string TableName => "qualsData";
        }
    }