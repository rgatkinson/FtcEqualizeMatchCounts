namespace FEMC.DAL
    {
    class Event : DBObject
        {
        public string EventCode;
        public string Name;

        public Event(Database db, DBTables.LeagueMeets.Row row) : base(db)
            {
            EventCode = row.EventCode.Value;
            Name = row.Name.Value;
            }
        }
    }
