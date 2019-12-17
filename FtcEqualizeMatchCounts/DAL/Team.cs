using System.CodeDom.Compiler;
using System.IO;

namespace FEMC.DAL
    {
    class Team : DBObject
        {
        //----------------------------------------------------------------------------------------
        // Accessing
        //----------------------------------------------------------------------------------------

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

        public int PreviousEventMatchCount
            {
            get {
                int result = 0;
                foreach (DBTables.LeagueHistory.Row row in Database.Tables.LeagueHistory.Rows)
                    {
                    if (row.Team.Value.Value == TeamNumber && row.MatchIsCounted && row.EventCode.Value != Database.ThisEventCode)
                        {
                        result += 1;
                        }
                    }
                return result;
                }
            }

        public int TotalMatchCountPlayed => PreviousEventMatchCount + PlayedMatchCount;

        //----------------------------------------------------------------------------------------
        // Construction
        //----------------------------------------------------------------------------------------

        public Team(Database database, DBTables.Team.Row row) : base(database)
            {
            TeamId = row.FMSTeamId;
            TeamNumber = (int)row.TeamNumber.Value.Value;
            Name = row.TeamNameShort.Value;
            }

        //----------------------------------------------------------------------------------------
        // Reporting
        //----------------------------------------------------------------------------------------

        public void Report(IndentedTextWriter writer)
            {
            writer.WriteLine($"Team {TeamNumber}: {Name}:");
            writer.Indent++;
            writer.WriteLine($"total: played match count: { TotalMatchCountPlayed }");
            writer.WriteLine($"previous events: played match count: { PreviousEventMatchCount }");
            writer.WriteLine($"this event: scheduled match count: { ScheduledMatchCount }");
            writer.WriteLine($"this event: played match count: { PlayedMatchCount }");
            writer.Indent--;
            }
        }
    }
