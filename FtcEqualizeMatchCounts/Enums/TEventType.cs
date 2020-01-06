namespace FEMC.Enums
    {
    enum TEventType // org.usfirst.ftc.event.EventData
        {
        [StringValue("Scrimmage")] SCRIMMAGE = 0,
        [StringValue("League Meet")] LEAGUE_MEET = 1,
        [StringValue("Qualifier")] QUALIFIER = 2,
        [StringValue("League Tournament")] LEAGUE_TOURNAMENT = 3,
        [StringValue("Championship")] CHAMPIONSHIP = 4,
        [StringValue("Other")] OTHER = 5
        }
    }