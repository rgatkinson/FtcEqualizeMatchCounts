#pragma warning disable 649
namespace FEMC.DBTables
    {
    //--------------------------------------------------------------------------------------------------------------------------
    // Match (probably don't need: written when match is *scored*)
    //--------------------------------------------------------------------------------------------------------------------------

    /**
     * Keeps track of Matches actually played: one row for each play of a match
     */
    class PlayedMatch : Table<PlayedMatch.Row, FMSMatchId>
        {
        public class Row : TableRow<FMSMatchId>
            {
            public FMSMatchId FMSMatchId; // primary
            public FMSScheduleDetailId FMSScheduleDetailId;
            public NullableLong PlayNumber;
            public NullableLong FieldType;
            public DateTimeAsString InitialPrestartTime;
            public DateTimeAsString FinalPreStartTime;
            public NullableLong PreStartCount;
            public DateTimeAsString AutoStartTime;
            public DateTimeAsString AutoEndTime;
            public DateTimeAsString TeleopStartTime;
            public DateTimeAsString TeleopEndTime;
            public DateTimeAsString RefCommitTime;
            public DateTimeAsString ScoreKeeperCommitTime;
            public DateTimeAsString PostMatchTime;
            public DateTimeAsString CancelMatchTime;
            public DateTimeAsString CycleTime;
            public NullableLong RedScore;
            public NullableLong BlueScore;
            public NullableLong RedPenalty;
            public NullableLong BluePenalty;
            public NullableLong RedAutoScore;
            public NullableLong BlueAutoScore;
            public ScoreDetails ScoreDetails;   
            public NullableLong HeadRefReview;
            public StringColumn VideoUrl;
            public DateTimeAsString CreatedOn;
            public StringColumn CreatedBy;     // e.g.: "Scorekeeper Commit"
            public DateTimeAsString ModifiedOn;
            public StringColumn ModifiedBy;
            public FMSEventId FMSEventId;
            public RowVersion RowVersion;

            public override FMSMatchId PrimaryKey => FMSMatchId;
            }

        public PlayedMatch(Database database) : base(database)
            {
            }

        public override string TableName => "Match";
        }
    }