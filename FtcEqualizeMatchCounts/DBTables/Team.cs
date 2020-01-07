#pragma warning disable 649
namespace FEMC.DBTables
    {
    //--------------------------------------------------------------------------------------------------------------------------
    // Team
    //--------------------------------------------------------------------------------------------------------------------------

    class Team : Table<Team.Row, FMSTeamId>
        {
        public class Row : TableRow<Row, FMSTeamId> // Four records for each match Alliance (1,2) x Station (1,2)
            {
            public FMSTeamId FMSTeamId;     // primary
            public FMSSeasonId FMSSeasonId;
            public FMSRegionId FMSRegionId;
            public NullableLong TeamId; // eg: 1029074, 1028838, 1029022, etc; not sure how used?
            public NullableLong TeamNumber;
            public StringColumn TeamNameLong;
            public StringColumn TeamNameShort;
            public StringColumn RobotName;
            public StringColumn City;
            public StringColumn StateProv;
            public StringColumn Country;
            public StringColumn Website;
            public NullableLong RookieYear;
            public BooleanAsInteger WasAddedFromUI;
            public BooleanAsInteger CMPPrequalified;
            public StringColumn SchoolName;
            public BooleanAsInteger DemoTeam;
            public FMSHomeCMPId FMSHomeCMPId;
            public GameSpecifics GameSpecifics;
            public DateTimeAsString CreatedOn;
            public StringColumn CreatedBy; // e.g. "Team Data Download"
            public DateTimeAsString ModifedOn;
            public StringColumn ModifiedBy; // e.g. "Team Data Download"

            public override FMSTeamId PrimaryKey => FMSTeamId;
            }

        public Team(Database database) : base(database)
            {
            }

        public override string TableName => "Team";
        }
    }