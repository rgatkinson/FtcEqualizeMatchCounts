#pragma warning disable 649
namespace FEMC.DBTables
    {
    /**
     * Keeps track of Matches actually played: one row for each play of a match
     */
    class Match : Table<Match.Row, FMSMatchId>
        {
        public class Row : TableRow<Row, FMSMatchId>
            {
            // Comments for a stylized, manually-scored Equalization Match unless otherwise stated.
            public FMSMatchId FMSMatchId; // primary, a new allocated for every play of a given MatchNumber
            public FMSScheduleDetailId FMSScheduleDetailId;
            public NullableLong PlayNumber;
            public NullableLong FieldType;
            public DateTimeAsString InitialPrestartTime;    // null
            public DateTimeAsString FinalPreStartTime;      // null
            public NullableLong PreStartCount;              // 1
            public DateTimeAsString AutoStartTime;          // non-null
            public DateTimeAsString AutoEndTime;            // null
            public DateTimeAsString TeleopStartTime;        // null
            public DateTimeAsString TeleopEndTime;          // null
            public DateTimeAsString RefCommitTime;          // null
            public DateTimeAsString ScoreKeeperCommitTime;  // non-null, observed to always be just before AutoStartTime by a couple of ms
            public DateTimeAsString PostMatchTime;          // null
            public DateTimeAsString CancelMatchTime;        // null
            public DateTimeAsString CycleTime;              // null
            public NullableLong RedScore;                   // 0
            public NullableLong BlueScore;                  // 10
            public NullableLong RedPenalty;                 // 0
            public NullableLong BluePenalty;                // 0
            public NullableLong RedAutoScore;               // 0
            public NullableLong BlueAutoScore;              // 10
            public ScoreDetailsColumn ScoreDetails;         // blobified details: scoreDetailsGZIP
            public NullableLong HeadRefReview;              // 0
            public StringColumn VideoUrl;                   // null
            public DateTimeAsString CreatedOn;              // non-null
            public StringColumn CreatedBy;                  // e.g.: "Scorekeeper Commit"
            public DateTimeAsString ModifiedOn;             // same as CreatedOn
            public StringColumn ModifiedBy;                 // null
            public FMSEventId FMSEventId;                   // null
            public RowVersion RowVersion;                   // ??? 0 length?

            public override FMSMatchId PrimaryKey => FMSMatchId;
            }

        public Match(Database database) : base(database)
            {
            }

        public override string TableName => "Match";
        }
    }