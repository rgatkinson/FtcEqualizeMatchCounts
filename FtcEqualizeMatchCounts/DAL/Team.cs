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
        public int EqualizationMatchCount => EqualizationMatches.Count;

        public List<Match> EqualizationMatches
            {
            get {
                var result = new List<Match>();
                result.AddRange(LeagueHistoryMatches);
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

        // Only the matches not actually in *this*event*
        public List<LeagueHistoryMatch> LeagueHistoryMatches
            {
            get {
                var result = new List<LeagueHistoryMatch>();
                foreach (var match in Database.LeagueHistoryMatchesByNumber.Values)
                    {
                    if (match.EventCode != Database.ThisEventCode && match.Plays(this))
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

        public void Report(IndentedTextWriter writer, bool verbose, int max)
            {
            writer.WriteLine($"Team {TeamNumber}: {Name}:");
            writer.Indent++;
            writer.WriteLine($"equalization match count: { EqualizationMatchCount }");
            writer.WriteLine($"equalization match count deficit: { max - EqualizationMatchCount }");
            if (verbose)
                {
                writer.WriteLine($"previous events: played match count: { LeagueHistoryMatchCount }");
                writer.WriteLine($"this event: scheduled match count: { ScheduledMatchCountThisEvent }");
                writer.WriteLine($"this event: played match count: { PlayedMatchCountThisEvent }");
                }
            writer.Indent--;
            }
        }
    }
