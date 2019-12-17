using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEMC
    {
    class ScheduledMatch : DBObject
        {
        public FMSScheduleDetailId FMSScheduleDetailId;
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

        public bool Plays(Team team)
            {
            if (!Red1Surrogate && Red1 == team) return true;
            if (!Red2Surrogate && Red2 == team) return true;
            if (!Blue1Surrogate && Blue1 == team) return true;
            if (!Blue2Surrogate && Blue2 == team) return true;
            return false;
            }

        public ScheduledMatch(Database db, DBTables.ScheduledMatch.Row row) : base(db)
            {
            FMSScheduleDetailId = row.FMSScheduleDetailId;
            MatchNumber = row.MatchNumber.Value.Value;
            Description = row.Description.Value;

            var qual = db.Tables.Quals.Map[row.MatchNumber];

            Red1 = db.TeamsByNumber[qual.Red1.Value.Value];
            Red2 = db.TeamsByNumber[qual.Red2.Value.Value];
            Blue1 = db.TeamsByNumber[qual.Blue1.Value.Value];
            Blue2 = db.TeamsByNumber[qual.Blue2.Value.Value];

            Red1Surrogate = qual.Red1Surrogate.Value.Value;
            Red2Surrogate = qual.Red2Surrogate.Value.Value;
            Blue1Surrogate = qual.Blue1Surrogate.Value.Value;
            Blue2Surrogate = qual.Blue2Surrogate.Value.Value;
            }
        }
    }
