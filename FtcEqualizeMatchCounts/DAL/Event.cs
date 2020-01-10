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
        public DateTimeOffset? Start;
        public DateTimeOffset? End;
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

        public DateTimeOffset StartNonNull => Start ?? StartIfNull;
        public static DateTimeOffset StartIfNull = new DateTimeOffset(new DateTime(2019, 09, 01, 0, 0, 0, DateTimeKind.Utc)); // arbitrary

        public override string ToString() => $"{GetType().Name}: EventCode={EventCode} Name={Name}";

        public abstract ICollection<SimpleTeam> SimpleTeams
            {
            get;
            }

        //----------------------------------------------------------------------------------------
        // Construction
        //----------------------------------------------------------------------------------------

        protected Event(Database db, DBTables.LeagueMeets.Row row, TEventType type, TEventStatus status) : base(db)
            {
            EventCode = row.EventCode.Value;
            Name = row.Name.Value;
            Start = row.Start.DateTimeOffset;
            End = row.End.DateTimeOffset;
            Type = type;
            Status = status;
            }

        public void AddMatch(HistoricalMatch match)
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
