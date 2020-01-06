using FEMC.DBTables;

namespace FEMC
    {
    class Tables
        {
        public Blocks Blocks;
        public Config Config;
        public ElimsData ElimsData;
        public ElimsGameSpecific ElimsGameSpecific;
        public ElimsResults ElimsResults;
        public ElimsScores ElimsScores;
        public LeagueConfig LeagueConfig;
        public LeagueHistory LeagueHistory;
        public LeagueInfo LeagueInfo;
        public LeagueMeets LeagueMeets;
        public Match Match;
        public MatchSchedule MatchSchedule;
        public Quals Quals;
        public QualsData QualsData;
        public QualsGameSpecific QualsGameSpecific;
        public QualsResults QualsResults;
        public QualsScores QualsScores;
        public ScheduleDetail ScheduledMatch;
        public ScheduleStation ScheduledMatchStation;
        public Team Team;

        public Tables(Database db)
            {
            Blocks = new Blocks(db);
            Config = new Config(db);
            ElimsData = new ElimsData(db);
            ElimsGameSpecific = new ElimsGameSpecific(db);
            ElimsResults = new ElimsResults(db);
            ElimsScores = new ElimsScores(db);
            LeagueConfig = new LeagueConfig(db);
            LeagueHistory = new LeagueHistory(db);
            LeagueInfo = new LeagueInfo(db);
            LeagueMeets = new LeagueMeets(db);
            Match = new Match(db);
            MatchSchedule = new MatchSchedule(db);
            Quals = new Quals(db);
            QualsData = new QualsData(db);
            QualsGameSpecific = new QualsGameSpecific(db);
            QualsResults = new QualsResults(db);
            QualsScores = new QualsScores(db);
            ScheduledMatch = new ScheduleDetail(db);
            ScheduledMatchStation = new ScheduleStation(db);
            Team = new Team(db);
            }

        public void Clear()
            {
            Blocks.Clear();
            Config.Clear();
            ElimsData.Clear();
            ElimsGameSpecific.Clear();
            ElimsResults.Clear();
            ElimsScores.Clear();
            LeagueConfig.Clear();
            LeagueHistory.Clear();
            LeagueInfo.Clear();
            LeagueMeets.Clear();
            Match.Clear();
            MatchSchedule.Clear();
            Quals.Clear();
            QualsData.Clear();
            QualsGameSpecific.Clear();
            QualsResults.Clear();
            QualsScores.Clear();
            ScheduledMatch.Clear();
            ScheduledMatchStation.Clear();
            Team.Clear();
            }

        public void Load()
            {
            Blocks.Load();
            Config.Load();
            ElimsData.Load();
            ElimsGameSpecific.Load();
            ElimsResults.Load();
            ElimsScores.Load();
            LeagueConfig.Load();
            LeagueHistory.Load();
            LeagueInfo.Load();
            LeagueMeets.Load();
            Match.Load();
            MatchSchedule.Load();
            Quals.Load();
            QualsData.Load();
            QualsGameSpecific.Load();
            QualsResults.Load();
            QualsScores.Load();
            ScheduledMatch.Load();
            ScheduledMatchStation.Load();
            Team.Load();
            }
        }
    }