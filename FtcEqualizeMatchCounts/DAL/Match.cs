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
