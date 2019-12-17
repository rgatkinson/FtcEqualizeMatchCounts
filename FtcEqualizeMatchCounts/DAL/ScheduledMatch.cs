using System.Security.Cryptography.X509Certificates;

namespace FEMC.DAL
    {
    class ScheduledMatch : ThisEventMatch
        {
        //----------------------------------------------------------------------------------------
        // Accessing
        //----------------------------------------------------------------------------------------

        private long matchNumber;
        public string Description;
        public string CreatedBy;

        public Team Red1;
        public bool Red1Surrogate;
        public Team Red2;
        public bool Red2Surrogate;
        public Team Blue1;
        public bool Blue1Surrogate;
        public Team Blue2;
        public bool Blue2Surrogate;

        public override ScheduledMatch Scheduled => this;

        public override long MatchNumber => matchNumber;

        public override bool IsEqualizationMatch => Equals(CreatedBy, Database.EqualizationMatchCreatorName);


        // Does this team play in this match?
        public override bool Plays(Team team)
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
            matchNumber = row.MatchNumber.NonNullValue;
            Description = row.Description.Value;
            CreatedBy = row.CreatedBy.Value;

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
