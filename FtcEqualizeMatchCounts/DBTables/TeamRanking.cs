#pragma warning disable 649
namespace FEMC.DBTables
    {
    //--------------------------------------------------------------------------------------------------------------------------
    // TeamRanking
    //--------------------------------------------------------------------------------------------------------------------------

    class TeamRanking : Table<TeamRanking.Row, (FMSEventId, FMSTeamId)>
        {
        public class Row : TableRow<Row, (FMSEventId, FMSTeamId)> // Four records for each match Alliance (1,2) x Station (1,2)
            {
            public FMSEventId FMSEventId;
            public FMSTeamId FMSTeamId; 
            public NullableLong Ranking;
            public NullableLong RankChange;
            public NullableLong Wins;
            public NullableLong Losses;
            public NullableLong Ties;
            public FloatColumn QualifyingScore; 
            public NullableLong PointsScoredTotal;
            public FloatColumn PointsScoredAverage;
            public NullableLong PointsScoredAverageChange;
            public NullableLong MatchesPlayed;
            public NullableLong Disqualified;
            public FloatColumn SortOrder1;
            public FloatColumn SortOrder2;
            public FloatColumn SortOrder3;
            public FloatColumn SortOrder4;
            public FloatColumn SortOrder5;
            public FloatColumn SortOrder6;
            public DateTimeAsString ModifiedOn;

            public override (FMSEventId, FMSTeamId) PrimaryKey => (FMSEventId, FMSTeamId);
            }

        public TeamRanking(Database database) : base(database)
            {
            }

        public override string TableName => "TeamRanking";
        }
    }