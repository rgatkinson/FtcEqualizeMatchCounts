namespace FEMC.DBTables
    {
    //--------------------------------------------------------------------------------------------------------------------------
    // Quals
    //--------------------------------------------------------------------------------------------------------------------------

    class QualsData : Table<QualsData.Row, LongColumn>
        {
        public class Row : TableRow<LongColumn>
            {
            public LongColumn Match; // small integer match number, primary
            public LongColumn Status;
            public LongColumn Randomization;
            public DateTimeAsInteger Start;      // format?
            public DateTimeAsInteger ScheduleStart;
            public DateTimeAsInteger PostedTime;
            public FMSMatchIdAsString FMSMatchId;
            public FMSScheduleDetailIdAsString FMSScheduleDetailId;

            public override LongColumn PrimaryKey => Match;
            }

        public QualsData(Database database) : base(database)
            {
            }

        public override string TableName => "qualsData";
        }
    }