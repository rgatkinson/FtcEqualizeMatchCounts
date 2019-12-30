using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace FEMC.DAL
    {
    class Event : DBObject
        {
        //----------------------------------------------------------------------------------------
        // Types
        //----------------------------------------------------------------------------------------

        public enum TStatus
            {
            [StringValue("Future")] FUTURE = 0,
            [StringValue("Setup")] SETUP = 1,
            [StringValue("Inspection")] INSPECTION = 2,
            [StringValue("Qualifications")] QUALS = 3,
            [StringValue("Alliance Selection")] SELECTION = 4,
            [StringValue("Eliminations")] ELIMS = 5,
            [StringValue("Archived")] ARCHIVED = 6,
            }
    
        public enum TEvent // org.usfirst.ftc.event.EventData
            {
            [StringValue("Scrimmage")] SCRIMMAGE = 0,
            [StringValue("League Meet")] LEAGUE_MEET = 1,
            [StringValue("Qualifier")] QUALIFIER = 2,
            [StringValue("League Tournament")] LEAGUE_TOURNAMENT = 3,
            [StringValue("Championship")] CHAMPIONSHIP = 4,
            [StringValue("Other")] OTHER = 5
            }

        //----------------------------------------------------------------------------------------
        // Accessing
        //----------------------------------------------------------------------------------------

        public string EventCode;
        public string Name;
        public DateTime? Start;
        public TEvent Type;
        public TStatus Status;
        public List<Match> Matches = new List<Match>();
        public long LastMatchNumber { get
            {
            long result = 0;
            foreach (Match match in Matches)
                {
                result = Math.Max(result, match.MatchNumber);
                }
            return result;
            } }

        public DateTime StartNonNull => Start ?? StartIfNull;
        public static DateTime StartIfNull = new DateTime(2019, 09, 01); // arbitrary

        public override string ToString() => $"{GetType().Name}: EventCode={EventCode} Name={Name}";

        //----------------------------------------------------------------------------------------
        // Construction
        //----------------------------------------------------------------------------------------

        public Event(Database db, DBTables.LeagueMeets.Row row, TEvent type, TStatus status) : base(db)
            {
            EventCode = row.EventCode.Value;
            Name = row.Name.Value;
            Start = row.Start.LocalDateTime;
            Type = type;
            Status = status;
            }

        public void AddMatch(Match match)
            {
            Matches.Add(match);
            }

        //----------------------------------------------------------------------------------------
        // Reporting
        //----------------------------------------------------------------------------------------

        public void Report(IndentedTextWriter writer)
            {
            writer.WriteLine($"Name: {Name}");
            writer.WriteLine($"Event Code: {EventCode}");
            writer.WriteLine($"Event Type: {Type.GetStringValue()}");
            writer.WriteLine($"Event Status: {Status.GetStringValue()}");
            if (Start.HasValue)
                {
                writer.WriteLine($"Event Start: {Start.Value}");
                }
            }
        }
    }
