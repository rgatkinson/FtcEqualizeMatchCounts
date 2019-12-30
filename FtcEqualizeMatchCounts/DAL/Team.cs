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

        public FMSTeamId FMSTeamId;
        public int TeamNumber;
        public string Name;

        public override string ToString()
            {
            return $"Team {TeamNumber}: {Name}";
            }

        public int ScheduledMatchCountThisEvent => ScheduledMatchesThisEvent.Count;
        public int LeagueHistoryMatchCount => LeagueHistoryMatchesThatCount.Count;
        public int PlayedMatchCountThisEvent => PlayedMatchesThisEvent.Count;
        public int EqualizationMatchCount => EqualizationMatches.Count;
        public int AveragingMatchCount => AveragingMatches.Count;

        public List<LeagueHistoryMatch> LeagueHistoryMatchesThatCount = new List<LeagueHistoryMatch>();
        public List<Match> AveragingMatches
            {
            get {
                var result = new List<Match>();
                result.AddRange(LeagueHistoryMatchesThatCount);
                result.AddRange(EqualizationMatches);
                return result;
                }
            }

        public List<Match> EqualizationMatches
            {
            get
                {
                var result = new List<Match>();
                foreach (var match in ScheduledMatchesThisEvent)
                    {
                    if (match.IsEqualizationMatch)
                        {
                        result.Add(match);
                        }
                    }
                return result;
                }
            }

        public List<PlayedMatch> PlayedMatchesThisEvent
            {
            get {
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
            get {
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


        //----------------------------------------------------------------------------------------
        // Construction
        //----------------------------------------------------------------------------------------

        public Team(Database database, DBTables.Team.Row row) : base(database)
            {
            FMSTeamId = row.FMSTeamId;
            TeamNumber = (int)row.TeamNumber.NonNullValue;
            Name = row.TeamNameShort.NonNullValue;
            }

        //----------------------------------------------------------------------------------------
        // Reporting
        //----------------------------------------------------------------------------------------

        public void Report(IndentedTextWriter writer, bool verbose, int averagingMatchCountGoal)
            {
            int averagingMatchesNeeded = averagingMatchCountGoal - AveragingMatchCount;
            if (verbose)
                { 
                writer.WriteLine($"Team {TeamNumber}: {Name}:");
                writer.Indent++;
                writer.WriteLine($"existing averaging matches: { AveragingMatchCount }");
                writer.WriteLine($"averaging matches needed: { averagingMatchesNeeded }");
                writer.WriteLine($"previous events: matches played: { LeagueHistoryMatchCount }");
                // writer.WriteLine($"this event: equalization match already scheduled: { EqualizationMatchCount }");
                // writer.WriteLine($"this event: matched schedule: { ScheduledMatchCountThisEvent }");
                // writer.WriteLine($"this event: played match count: { PlayedMatchCountThisEvent }");
                writer.Indent--;
                }
            else
                {
                writer.WriteLine($"Team {TeamNumber}: {Name}: averaging matches needed: { averagingMatchesNeeded }");
                }
            }
        }
    }
