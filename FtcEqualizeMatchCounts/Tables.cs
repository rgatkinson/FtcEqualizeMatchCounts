using FEMC.DBTables;

namespace FEMC
    {
    class Tables
        {
        public DBTables.Config Config;
        public DBTables.LeagueHistory LeagueHistory;
        public DBTables.LeagueInfo LeagueInfo;
        public DBTables.LeagueMeets LeagueMeets;
        public DBTables.MatchSchedule MatchSchedule;
        public DBTables.PlayedMatch PlayedMatch;
        public DBTables.Quals Quals;
        public DBTables.QualsData QualsData;
        public DBTables.ScheduledMatch ScheduledMatch;
        public DBTables.ScheduledMatchStation ScheduledMatchStation;
        public DBTables.Team Team;

        public Tables(Database db)
            {
            Config = new DBTables.Config(db);
            LeagueHistory = new DBTables.LeagueHistory(db);
            LeagueInfo = new LeagueInfo(db);
            LeagueMeets = new LeagueMeets(db);
            MatchSchedule = new DBTables.MatchSchedule(db);
            PlayedMatch = new DBTables.PlayedMatch(db);
            Quals = new DBTables.Quals(db);
            QualsData = new DBTables.QualsData(db);
            ScheduledMatch = new DBTables.ScheduledMatch(db);
            ScheduledMatchStation = new DBTables.ScheduledMatchStation(db);
            Team = new DBTables.Team(db);
            }

        public void Load()
            {
            Config.Load();
            LeagueHistory.Load();
            LeagueInfo.Load();
            LeagueMeets.Load();
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