namespace FEMC
    {
    //--------------------------------------------------------------------------------------------------------------------------
    // Quals
    //--------------------------------------------------------------------------------------------------------------------------

    class Table_QualsData : Table<Table_QualsData.Row>
        {
        public class Row : TableRow
            {
            public long Match; // small integer match number
            public long Status;
            public long Randomization;
            public DateTimeAsInteger Start;      // format?
            public DateTimeAsInteger ScheduleStart;
            public DateTimeAsInteger PostedTime;
            public FMSMatchIdAsString FMSMatchId;
            public FMSScheduleDetailIdAsString FMSScheduleDetailId;
            }

        public Table_QualsData(Database database) : base(database)
            {
            }

        public override string TableName => "qualsData";
        }
    }