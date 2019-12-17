using FEMC.DBTables;

namespace FEMC
    {
    class Tables
        {
        public Match Match;
        public MatchSchedule MatchSchedule;
        public Quals Quals;
        public QualsData QualsData;
        public ScheduleDetail ScheduleDetail;
        public ScheduleStation ScheduleStation;
        public DBTables.Team Team;

        public Tables(Database db)
            {
            Match = new Match(db);
            MatchSchedule = new MatchSchedule(db);
            Quals = new Quals(db);
            QualsData = new QualsData(db);
            ScheduleDetail = new ScheduleDetail(db);
            ScheduleStation = new ScheduleStation(db);
            Team = new DBTables.Team(db);
            }

        public void Load()
            {
            Team.Load();

            MatchSchedule.Load();
            Match.Load();
            Quals.Load();
            QualsData.Load();
            ScheduleDetail.Load();
            ScheduleStation.Load();
            }
        }
    }