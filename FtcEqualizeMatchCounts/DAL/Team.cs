using System.CodeDom.Compiler;
using System.Collections.Generic;
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

        public int ScheduledMatchCountThisEvent => ScheduledMatchesThisEvent.Count;
        public int LeagueHistoryMatchCount => LeagueHistoryMatches.Count;
        public int PlayedMatchCountThisEvent => PlayedMatchesThisEvent.Count;

        public List<PlayedMatch> PlayedMatchesThisEvent
            {
            get
                {
                var result = new List<PlayedMatch>();
                foreach (var matches in Database.PlayedMatchesByNumber.Values)
                    {
                    foreach (var match in matches)
                        { 
                        if (match.Plays(this))
                            {
                            result.Add(match);
                            }
                        }
                    }
                return result;
                }
            }

        public List<ScheduledMatch> ScheduledMatchesThisEvent
            {
            get
                {
                var result = new List<ScheduledMatch>();
                foreach (var match in Database.ScheduledMatchesByNumber.Values)
                    {
                    if (match.Plays(this))
                        {
                        result.Add(match);
                        }
                    }
                return result;
                }
            }

        public List<LeagueHistoryMatch> LeagueHistoryMatches
            {
            get
                {
                var result = new List<LeagueHistoryMatch>();
                foreach (var match in Database.LeagueHistoryMatchesByNumber.Values)
                    {
                    if (match.Plays(this))
                        {
                        result.Add(match);
                        }
                    }
                return result;
                }
            }

        public int TotalMatchCountPlayed => LeagueHistoryMatchCount + PlayedMatchCountThisEvent;

        //----------------------------------------------------------------------------------------
        // Construction
        //----------------------------------------------------------------------------------------

        public Team(Database database, DBTables.Team.Row row) : base(database)
            {
            TeamId = row.FMSTeamId;
            TeamNumber = (int)row.TeamNumber.NonNullValue;
            Name = row.TeamNameShort.NonNullValue;
            }

        //----------------------------------------------------------------------------------------
        // Reporting
        //----------------------------------------------------------------------------------------

        public void Report(IndentedTextWriter writer)
            {
            writer.WriteLine($"Team {TeamNumber}: {Name}:");
            writer.Indent++;
            writer.WriteLine($"total: played match count: { TotalMatchCountPlayed }");
            writer.WriteLine($"previous events: played match count: { LeagueHistoryMatchCount }");
            writer.WriteLine($"this event: scheduled match count: { ScheduledMatchCountThisEvent }");
            writer.WriteLine($"this event: played match count: { PlayedMatchCountThisEvent }");
            writer.Indent--;
            }
        }
    }
