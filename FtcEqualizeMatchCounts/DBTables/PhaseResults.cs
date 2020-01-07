#pragma warning disable 649
namespace FEMC.DBTables
    {
    abstract class PhaseResults : Table<PhaseResults.Row, long>
        {
        public class Row : TableRow<Row, long>
            {
            public NullableLong MatchNumber;
            public NullableLong RedScore;
            public NullableLong BlueScore;
            public NullableLong RedPenaltyCommitted;
            public NullableLong BluePenaltyCommitted;

            public override long PrimaryKey => MatchNumber.NonNullValue;
            }

        protected PhaseResults(Database database) : base(database)
            {
            }
        }
    }