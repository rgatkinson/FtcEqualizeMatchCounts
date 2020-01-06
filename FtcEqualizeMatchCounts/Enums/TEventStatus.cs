namespace FEMC.Enums
    {
    enum TEventStatus
        {
        [StringValue("Future")] FUTURE = 0,
        [StringValue("Setup")] SETUP = 1,
        [StringValue("Inspection")] INSPECTION = 2,
        [StringValue("Qualifications")] QUALS = 3,
        [StringValue("Alliance Selection")] SELECTION = 4,
        [StringValue("Eliminations")] ELIMS = 5,
        [StringValue("Archived")] ARCHIVED = 6,
        }
    }