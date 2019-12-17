namespace FEMC
    {
    class Tables
        {
        public Table_Match Match;
        public Table_MatchSchedule MatchSchedule;
        public Table_Quals Quals;
        public Table_QualsData QualsData;
        public Table_ScheduleDetail ScheduleDetail;
        public Table_ScheduleStation ScheduleStation;
        public Table_Team Team;

        public Tables(Database db)
            {
            Match = new Table_Match(db);
            MatchSchedule = new Table_MatchSchedule(db);
            Quals = new Table_Quals(db);
            QualsData = new Table_QualsData(db);
            ScheduleDetail = new Table_ScheduleDetail(db);
            ScheduleStation = new Table_ScheduleStation(db);
            Team = new Table_Team(db);
            }

        public void Load()
            {
            ScheduleStation.Load();

            MatchSchedule.Load();
            Match.Load();
            Quals.Load();
            QualsData.Load();
            ScheduleDetail.Load();
            Team.Load();
            }
        }
    }