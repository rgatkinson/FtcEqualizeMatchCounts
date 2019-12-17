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
        public Team Team;

        public Tables(Database db)
            {
            Match = new Match(db);
            MatchSchedule = new MatchSchedule(db);
            Quals = new Quals(db);
            QualsData = new QualsData(db);
            ScheduleDetail = new ScheduleDetail(db);
            ScheduleStation = new ScheduleStation(db);
            Team = new Team(db);
            }

        public void Load()
            {
            MatchSchedule.Load();
            Match.Load();
            Quals.Load();
            QualsData.Load();
            ScheduleDetail.Load();
            ScheduleStation.Load();
            Team.Load();
            }
        }
    }