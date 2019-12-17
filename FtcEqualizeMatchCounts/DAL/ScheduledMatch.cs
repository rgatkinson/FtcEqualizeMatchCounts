namespace FEMC.DAL
    {
    class ScheduledMatch : ThisEventMatch
        {
        //----------------------------------------------------------------------------------------
        // Accessing
        //----------------------------------------------------------------------------------------

        public long MatchNumber;
        public string Description;

        public Team Red1;
        public bool Red1Surrogate;
        public Team Red2;
        public bool Red2Surrogate;
        public Team Blue1;
        public bool Blue1Surrogate;
        public Team Blue2;
        public bool Blue2Surrogate;

        public override ScheduledMatch Scheduled => this;

        // Does this team play in this match?
        public bool Plays(Team team)
            {
            if (!Red1Surrogate && Red1 == team) return true;
            if (!Red2Surrogate && Red2 == team) return true;
            if (!Blue1Surrogate && Blue1 == team) return true;
            if (!Blue2Surrogate && Blue2 == team) return true;
            return false;
            }

        //----------------------------------------------------------------------------------------
        // Construction
        //----------------------------------------------------------------------------------------

        public ScheduledMatch(Database db, DBTables.ScheduledMatch.Row row) : base(db, row.FMSEventId, row.FMSScheduleDetailId)
            {
            FMSScheduleDetailId = row.FMSScheduleDetailId;
            MatchNumber = row.MatchNumber.NonNullValue;
            Description = row.Description.Value;

            var qual = db.Tables.Quals.Map[row.MatchNumber];

            Red1 = db.TeamsByNumber[qual.Red1.NonNullValue];
            Red2 = db.TeamsByNumber[qual.Red2.NonNullValue];
            Blue1 = db.TeamsByNumber[qual.Blue1.NonNullValue];
            Blue2 = db.TeamsByNumber[qual.Blue2.NonNullValue];

            Red1Surrogate = qual.Red1Surrogate.NonNullValue;
            Red2Surrogate = qual.Red2Surrogate.NonNullValue;
            Blue1Surrogate = qual.Blue1Surrogate.NonNullValue;
            Blue2Surrogate = qual.Blue2Surrogate.NonNullValue;
            }
        }
    }
