namespace FEMC.DBTables
    {
    //--------------------------------------------------------------------------------------------------------------------------
    // Quals
    //--------------------------------------------------------------------------------------------------------------------------

    class QualsData : Table<QualsData.Row>
        {
        public class Row : TableRow
            {
            public LongColumn Match; // small integer match number
            public LongColumn Status;
            public LongColumn Randomization;
            public DateTimeAsInteger Start;      // format?
            public DateTimeAsInteger ScheduleStart;
            public DateTimeAsInteger PostedTime;
            public FMSMatchIdAsString FMSMatchId;
            public FMSScheduleDetailIdAsString FMSScheduleDetailId;
            }

        public QualsData(Database database) : base(database)
            {
            }

        public override string TableName => "qualsData";
        }
    }