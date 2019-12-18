using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEMC.DAL
    {
    abstract class Match : DBObject
        {
        //----------------------------------------------------------------------------------------
        // Types
        //----------------------------------------------------------------------------------------

        public enum TAlliance
            {
            Red = 1,
            Blue = 2,
            }

        public enum TStation
            {
            Station1 = 1,
            Station2 = 2,
            }

        public enum TTournamentLevel
            {
            Unknown = -1,
            Qualification = 2,
            }

        public enum TFieldType
            {
            Unknown = -1,
            Usual = 1,      // don't actually know what this means
            }

        public enum TStatus
            {
            DefaultValue = 0,   // semantic being 'unplayed' is just a guess
            }

        public enum TRandomization
            {
            DefaultValue = -1,
            }

        public enum TMatchScheduleType
            {
            Match = 0,              // MAY be only qual match; not sure
            AdminBreak = 1,         // admin-created break, such as lunch
            InterMatchBreak = 2,    // scoring-system-created break, such as 5 minute break between matches
            }

        //----------------------------------------------------------------------------------------
        // Accessing
        //----------------------------------------------------------------------------------------

        public abstract string EventCode { get; }
        public abstract long MatchNumber { get; }
        public virtual bool IsEqualizationMatch => false;
        
        public abstract bool Plays(Team team);

        public Event Event => Database.EventsByCode[EventCode];

        public override string ToString() => $"{GetType().Name}: Event={EventCode} MatchNumber={MatchNumber}";

        //----------------------------------------------------------------------------------------
        // Construction
        //----------------------------------------------------------------------------------------
        
        protected Match(Database db) : base(db)
            {
            }
        }
    }
