namespace FEMC.Enums
    {

    // ?? A historical analog to TMatchType ??
    // see SQLiteManagementDAO.saveFMSSchedule() for conversion
    public enum TTournamentLevel // org.usfirst.ftc.event.management.SQLiteManagementDAO.saveFMSSchedule
        {
        Unknown = -1,
        Other = 0,
        Qualification = 2,
        Eliminations = 3,
        }
    }