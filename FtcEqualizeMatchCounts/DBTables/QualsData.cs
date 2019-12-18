#pragma warning disable 649
namespace FEMC.DBTables
    {
    //--------------------------------------------------------------------------------------------------------------------------
    // Quals
    //--------------------------------------------------------------------------------------------------------------------------

    class QualsData : Table<QualsData.Row, NullableLong>
        {
        public class Row : TableRow<NullableLong>
            {
            public NullableLong Match; // small integer match number, primary
            public NullableLong Status;
            public NullableLong Randomization;
            public DateTimeAsInteger Start;
            public DateTimeAsInteger ScheduleStart;
            public DateTimeAsInteger PostedTime;
            public FMSMatchIdAsString FMSMatchId;
            public FMSScheduleDetailIdAsString FMSScheduleDetailId;

            public override NullableLong PrimaryKey => Match;
            }

        public QualsData(Database database) : base(database)
            {
            }

        public override string TableName => "qualsData";
        }
    }