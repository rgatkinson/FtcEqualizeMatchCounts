namespace FtcEqualizeMatchCounts
    {
    //--------------------------------------------------------------------------------------------------------------------------
    // Match (probably don't need: written when match is *scored*)
    //--------------------------------------------------------------------------------------------------------------------------

    class Table_Match : Table
        {
        public class Row : TableRow
            {
            public FMSMatchId FMSMatchId;
            public FMSScheduleDetailId FMSScheduleDetailId;
            public long PlayNumber;
            public long FieldType;
            public DateTimeAsString InitialPrestartTime;
            public DateTimeAsString FinalPreStartTime;
            public long PreStartCount;
            public DateTimeAsString AutoStartTime;
            public DateTimeAsString AutoEndTime;
            public DateTimeAsString TeleopStartTime;
            public DateTimeAsString TeleopEndTime;
            public DateTimeAsString RefCommitTime;
            public DateTimeAsString ScoreKeeperCommitTime;
            public DateTimeAsString PostMatchTime;
            public DateTimeAsString CancelMatchTime;
            public DateTimeAsString CycleTime;
            public long RedScore;
            public long BlueScore;
            public long RedPenalty;
            public long BluePenalty;
            public long RedAutoScore;
            public long BlueAutoScore;
            public ScoreDetails ScoreDetails;   
            public long HeadRefReview;
            public string VideoUrl;
            public DateTimeAsString CreatedOn;
            public string CreatedBy;     // e.g.: "Scorekeeper Commit"
            public DateTimeAsString ModifiedOn;
            public string ModifiedBy;
            public FMSEventId FMSEventId;
            public RowVersion RowVersion; 
            }

        public Table_Match(Database database) : base(database)
            {
            }

        public override string TableName => "Match";

        public void Load()
            {
            Load<Row>();
            }
        }
    }