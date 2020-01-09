using System.Collections.Generic;
using FEMC.Enums;

namespace FEMC.DAL
    {
    abstract class Match : DBObject
        {
        //----------------------------------------------------------------------------------------
        // State
        //----------------------------------------------------------------------------------------

        public abstract string EventCode { get; }
        public abstract long MatchNumber { get; }
        public virtual bool IsEqualizationMatch => false;

        public abstract TMatchType MatchType { get; }
        
        public bool Plays(int teamNumber) => PlayedTeams.Contains(teamNumber);

        public abstract ICollection<int> PlayedTeams { get; }

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
