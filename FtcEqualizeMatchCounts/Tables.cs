using FEMC.DBTables;

namespace FEMC
    {
    class Tables
        {
        public Blocks Blocks;
        public Config Config;
        public ElimsCommitHistory ElimsCommitHistory;
        public ElimsData ElimsData;
        public ElimsGameSpecific ElimsGameSpecific;
        public ElimsGameSpecificHistory ElimsGameSpecificHistory;
        public ElimsResults ElimsResults;
        public ElimsScores ElimsScores;
        public ElimsScoresHistory ElimsScoresHistory;
        public LeagueConfig LeagueConfig;
        public LeagueHistory LeagueHistory;
        public LeagueInfo LeagueInfo;
        public LeagueMeets LeagueMeets;
        public Match Match;
        public MatchSchedule MatchSchedule;
        public Quals Quals;
        public QualsCommitHistory QualsCommitHistory;
        public QualsData QualsData;
        public QualsGameSpecific QualsGameSpecific;
        public QualsGameSpecificHistory QualsGameSpecificHistory;
        public QualsResults QualsResults;
        public QualsScores QualsScores;
        public QualsScoresHistory QualsScoresHistory;
        public ScheduleDetail ScheduleDetail;
        public ScheduleStation ScheduleStation;
        public Team Team;
        public TeamRanking TeamRanking;

        public Tables(Database db)
            {
            Blocks = new Blocks(db);
            Config = new Config(db);
            ElimsCommitHistory = new ElimsCommitHistory(db);
            ElimsData = new ElimsData(db);
            ElimsGameSpecific = new ElimsGameSpecific(db);
            ElimsGameSpecificHistory = new ElimsGameSpecificHistory(db);
            ElimsResults = new ElimsResults(db);
            ElimsScores = new ElimsScores(db);
            ElimsScoresHistory = new ElimsScoresHistory(db);
            LeagueConfig = new LeagueConfig(db);
            LeagueHistory = new LeagueHistory(db);
            LeagueInfo = new LeagueInfo(db);
            LeagueMeets = new LeagueMeets(db);
            Match = new Match(db);
            MatchSchedule = new MatchSchedule(db);
            Quals = new Quals(db);
            QualsCommitHistory = new QualsCommitHistory(db);
            QualsData = new QualsData(db);
            QualsGameSpecific = new QualsGameSpecific(db);
            QualsGameSpecificHistory = new QualsGameSpecificHistory(db);
            QualsResults = new QualsResults(db);
            QualsScores = new QualsScores(db);
            QualsScoresHistory = new QualsScoresHistory(db);
            ScheduleDetail = new ScheduleDetail(db);
            ScheduleStation = new ScheduleStation(db);
            Team = new Team(db);
            TeamRanking = new TeamRanking(db);
            }

        public void Clear()
            {
            Blocks.Clear();
            Config.Clear();
            ElimsCommitHistory.Clear();
            ElimsData.Clear();
            ElimsGameSpecific.Clear();
            ElimsGameSpecificHistory.Clear();
            ElimsResults.Clear();
            ElimsScores.Clear();
            ElimsScoresHistory.Clear();
            LeagueConfig.Clear();
            LeagueHistory.Clear();
            LeagueInfo.Clear();
            LeagueMeets.Clear();
            Match.Clear();
            MatchSchedule.Clear();
            Quals.Clear();
            QualsCommitHistory.Clear();
            QualsData.Clear();
            QualsGameSpecific.Clear();
            QualsGameSpecificHistory.Clear();
            QualsResults.Clear();
            QualsScores.Clear();
            QualsScoresHistory.Clear();
            ScheduleDetail.Clear();
            ScheduleStation.Clear();
            Team.Clear();
            TeamRanking.Clear();
            }

        public void Load()
            {
            Blocks.Load();
            Config.Load();
            ElimsCommitHistory.Load();
            ElimsData.Load();
            ElimsGameSpecific.Load();
            ElimsGameSpecificHistory.Load();
            ElimsResults.Load();
            ElimsScores.Load();
            ElimsScoresHistory.Load();
            LeagueConfig.Load();
            LeagueHistory.Load();
            LeagueInfo.Load();
            LeagueMeets.Load();
            Match.Load();
            MatchSchedule.Load();
            Quals.Load();
            QualsCommitHistory.Load();
            QualsData.Load();
            QualsGameSpecific.Load();
            QualsGameSpecificHistory.Load();
            QualsResults.Load();
            QualsScores.Load();
            QualsScoresHistory.Load();
            ScheduleDetail.Load();
            ScheduleStation.Load();
            Team.Load();
            TeamRanking.Load();
            }
        }
    }