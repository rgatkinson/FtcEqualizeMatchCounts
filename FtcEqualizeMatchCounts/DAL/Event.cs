using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using FEMC.DAL.Support;
using FEMC.Enums;

namespace FEMC.DAL
    {
    abstract class Event : DBObject
        {
        //----------------------------------------------------------------------------------------
        // Accessing
        //----------------------------------------------------------------------------------------

        public string EventCode;
        public string Name;
        public DateTime? Start;
        public TEventType Type;
        public TEventStatus Status;
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

        public abstract ICollection<SimpleTeam> SimpleTeams
            {
            get;
            }

        //----------------------------------------------------------------------------------------
        // Construction
        //----------------------------------------------------------------------------------------

        public Event(Database db, DBTables.LeagueMeets.Row row, TEventType type, TEventStatus status) : base(db)
            {
            EventCode = row.EventCode.Value;
            Name = row.Name.Value;
            Start = row.Start.LocalDateTime;
            Type = type;
            Status = status;
            }

        public void AddMatch(LeagueHistoryMatch match)
            {
            Matches.Add(match);
            }

        public void AddMatch(ScheduledMatch match)
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
