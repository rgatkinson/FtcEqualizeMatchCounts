using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace FEMC.DAL
    {
    class Event : DBObject
        {
        //----------------------------------------------------------------------------------------
        // Accessing
        //----------------------------------------------------------------------------------------

        public string EventCode;
        public string Name;
        public DateTime? Start;

        public DateTime StartNonNull => Start ?? StartIfNull;
        public static DateTime StartIfNull = new DateTime(2019, 09, 01); // arbitrary

        public override string ToString() => $"{GetType().Name}: EventCode={EventCode} Name={Name}";

        //----------------------------------------------------------------------------------------
        // Construction
        //----------------------------------------------------------------------------------------

        public Event(Database db, DBTables.LeagueMeets.Row row) : base(db)
            {
            EventCode = row.EventCode.Value;
            Name = row.Name.Value;
            Start = row.Start.LocalDateTime;
            }

        public void Report(IndentedTextWriter writer)
            {
            writer.WriteLine($"Name: {Name}");
            writer.WriteLine($"Event Code: {EventCode}");
            if (Start.HasValue)
                {
                writer.WriteLine($"Event Start: {Start.Value}");
                }
            }
        }
    }
