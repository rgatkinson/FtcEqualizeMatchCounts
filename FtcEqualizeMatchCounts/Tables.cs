using FEMC.DBTables;

namespace FEMC
    {
    class Tables
        {
        public DBTables.Blocks Blocks;
        public DBTables.Config Config;
        public DBTables.LeagueConfig LeagueConfig;
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
            Blocks = new DBTables.Blocks(db);
            Config = new DBTables.Config(db);
            LeagueConfig = new DBTables.LeagueConfig(db);
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

        public void Clear()
            {
            Blocks.Clear();
            Config.Clear();
            LeagueConfig.Clear();
            LeagueHistory.Clear();
            LeagueInfo.Clear();
            LeagueMeets.Clear();
            MatchSchedule.Clear();
            PlayedMatch.Clear();
            Quals.Clear();
            QualsData.Clear();
            ScheduledMatch.Clear();
            ScheduledMatchStation.Clear();
            Team.Clear();
            }

        public void Load()
            {
            Blocks.Load();
            Config.Load();
            LeagueConfig.Load();
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