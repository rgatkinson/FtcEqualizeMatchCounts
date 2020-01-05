#pragma warning disable 649
namespace FEMC.DBTables
    {
    abstract class PhaseData : Table<PhaseData.Row, NullableLong>
        {
        public class Row : TableRow<NullableLong>
            {
            public NullableLong Match;
            public NullableLong Status;
            public NullableLong Randomization;
            public DateTimeAsInteger Start;
            public DateTimeAsInteger ScheduleStart;
            public DateTimeAsInteger PostedTime;
            public FMSMatchIdAsString FMSMatchId;
            public FMSScheduleDetailIdAsString FMSScheduleDetailId;

            public override NullableLong PrimaryKey => Match;
            }

        public PhaseData(Database database) : base(database)
            {
            }
        }
    }