namespace FEMC
    {
    //--------------------------------------------------------------------------------------------------------------------------
    // Team
    //--------------------------------------------------------------------------------------------------------------------------

    class Team : Table<Team.Row>
        {
        public class Row : TableRow // Four records for each match Alliance (1,2) x Station (1,2)
            {
            public FMSEventId FMSEventId;
            public FMSTeamId FMSTeamId;
            public FMSRegionId FMSRegionId;
            public long TeamId; // eg: 1029074, 1028838, 1029022, etc; not sure how used?
            public long TeamNumber;
            public string TeamNameLong;
            public string TeamNameShort;
            public string RobotName;
            public string City;
            public string StateProv;
            public string Country;
            public string Website;
            public long RookieYear;
            public BooleanAsInteger WasAddedFromUI;
            public BooleanAsInteger CMPPrequalified;
            public string SchoolName;
            public BooleanAsInteger DemoTeam;
            public FMSHomeCMPId FMSHomeCMPId;
            public GameSpecifics GameSpecifics;
            public DateTimeAsString CreatedOn;
            public string CreatedBy; // e.g. "Team Data Download"
            public DateTimeAsString ModifedOn;
            public string ModifiedBy; // e.g. "Team Data Download"
            }

        public Team(Database database) : base(database)
            {
            }

        public override string TableName => "Team";
        }
    }