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

        public Event Event => Database.EventsByCode[EventCode];

        //----------------------------------------------------------------------------------------
        // Construction
        //----------------------------------------------------------------------------------------
        
        protected Match(Database db) : base(db)
            {
            }

        }
    }
