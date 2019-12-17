using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEMC.DAL
    {
    // A match played at this event (as opposed to one played historically in a previous (league) event)
    abstract class ThisEventMatch : Match
        {
        //----------------------------------------------------------------------------------------
        // Accessing
        //----------------------------------------------------------------------------------------

        public FMSEventId FMSEventId;
        public FMSScheduleDetailId FMSScheduleDetailId;

        public override string EventCode => Equals(Database.FMSEventId, FMSEventId) ? Database.ThisEventCode : null;
        public virtual ScheduledMatch Scheduled => Database.ScheduledMatchesById[FMSScheduleDetailId];

        public bool Plays(Team team) => Scheduled.Plays(team);

        //----------------------------------------------------------------------------------------
        // Construction
        //----------------------------------------------------------------------------------------

        protected ThisEventMatch(Database db, FMSEventId eventId, FMSScheduleDetailId detailId) : base(db)
            {
            FMSEventId = eventId;
            FMSScheduleDetailId = detailId;
            }

        }
    }
