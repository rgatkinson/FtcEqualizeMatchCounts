#pragma warning disable 649
namespace FEMC.DBTables
    {
    abstract class PhaseData : Table<PhaseData.Row, long>
        {
        public class Row : TableRow<Row, long>
            {
            public NullableLong MatchNumber;
            public NullableLong Status;
            public NullableLong Randomization;
            public DateTimeAsInteger Start;
            public DateTimeAsInteger ScheduleStart;
            public DateTimeAsInteger PostedTime;
            public FMSMatchIdAsString FMSMatchId;
            public FMSScheduleDetailIdAsString FMSScheduleDetailId;

            public override long PrimaryKey => MatchNumber.NonNullValue;
            }

        protected PhaseData(Database database) : base(database)
            {
            }
        }
    }