using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEMC
    {
    class Team : DBObject
        {
        public FMSTeamId TeamId;
        public int TeamNumber;
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

        public int ScheduledMatchCount
            {
            get {
                int result = 0;
                foreach (ScheduledMatch scheduledMatch in Database.ScheduledMatchesById.Values)
                    {
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
            TeamNumber = (int)row.TeamNumber.Value.Value;
            Name = row.TeamNameShort.Value;
            }

        public void Report(TextWriter writer)
            {
            writer.WriteLine($"Team {TeamNumber}: {Name}:");
            writer.WriteLine($"    scheduled match count: {ScheduledMatchCount}");
            writer.WriteLine($"    played match count: {PlayedMatchCount}");
            }
        }
    }
