using FEMC.DBTables;

namespace FEMC
    {
    class Tables
        {
        public DBTables.PlayedMatch PlayedMatch;
        public DBTables.MatchSchedule MatchSchedule;
        public DBTables.Quals Quals;
        public DBTables.QualsData QualsData;
        public DBTables.ScheduledMatch ScheduledMatch;
        public DBTables.ScheduledMatchStation ScheduledMatchStation;
        public DBTables.Team Team;

        public Tables(Database db)
            {
            PlayedMatch = new DBTables.PlayedMatch(db);
            MatchSchedule = new DBTables.MatchSchedule(db);
            Quals = new DBTables.Quals(db);
            QualsData = new DBTables.QualsData(db);
            ScheduledMatch = new DBTables.ScheduledMatch(db);
            ScheduledMatchStation = new DBTables.ScheduledMatchStation(db);
            Team = new DBTables.Team(db);
            }

        public void Load()
            {
            MatchSchedule.Load();
            PlayedMatch.Load();
            Quals.Load();
            QualsData.Load();
            ScheduledMatch.Load();
            ScheduledMatchStation.Load();
            Team.Load();
            }
        }
    }