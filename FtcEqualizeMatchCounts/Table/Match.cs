namespace FEMC.DBTables
    {
    //--------------------------------------------------------------------------------------------------------------------------
    // Match (probably don't need: written when match is *scored*)
    //--------------------------------------------------------------------------------------------------------------------------

    /**
     * Keeps track of Matches actually played: one row for each play of a match
     */
    class Match : Table<Match.Row>
        {
        public class Row : TableRow
            {
            public FMSMatchId FMSMatchId; // primary
            public FMSScheduleDetailId FMSScheduleDetailId;
            public LongColumn PlayNumber;
            public LongColumn FieldType;
            public DateTimeAsString InitialPrestartTime;
            public DateTimeAsString FinalPreStartTime;
            public LongColumn PreStartCount;
            public DateTimeAsString AutoStartTime;
            public DateTimeAsString AutoEndTime;
            public DateTimeAsString TeleopStartTime;
            public DateTimeAsString TeleopEndTime;
            public DateTimeAsString RefCommitTime;
            public DateTimeAsString ScoreKeeperCommitTime;
            public DateTimeAsString PostMatchTime;
            public DateTimeAsString CancelMatchTime;
            public DateTimeAsString CycleTime;
            public LongColumn RedScore;
            public LongColumn BlueScore;
            public LongColumn RedPenalty;
            public LongColumn BluePenalty;
            public LongColumn RedAutoScore;
            public LongColumn BlueAutoScore;
            public ScoreDetails ScoreDetails;   
            public LongColumn HeadRefReview;
            public StringColumn VideoUrl;
            public DateTimeAsString CreatedOn;
            public StringColumn CreatedBy;     // e.g.: "Scorekeeper Commit"
            public DateTimeAsString ModifiedOn;
            public StringColumn ModifiedBy;
            public FMSEventId FMSEventId;
            public RowVersion RowVersion; 
            }

        public Match(Database database) : base(database)
            {
            }

        public override string TableName => "Match";
        }
    }