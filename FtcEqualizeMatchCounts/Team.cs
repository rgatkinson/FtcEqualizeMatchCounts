using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEMC
    {
    class Team : DBObject
        {
        public FMSTeamId TeamId;
        public long TeamNumber;
        public string Name;

        public override string ToString()
            {
            return $"Team {TeamNumber}: {Name}";
            }

        public int PlayedMatchCount
            {
            get {
                int result = 0;
                foreach (var row in Database.Tables.PlayedMatch.Rows)
                    {
                    ScheduledMatch scheduledMatch = Database.ScheduledMatchesById[row.FMSScheduleDetailId];
                    if (scheduledMatch.Plays(this))
                        {
                        result += 1;
                        }
                    }
                return result;
                }
            }

        public Team(Database database, DBTables.Team.Row row) : base(database)
            {
            TeamId = row.FMSTeamId;
            TeamNumber = row.TeamNumber.Value.Value;
            Name = row.TeamNameShort.Value;
            }
        }
    }
