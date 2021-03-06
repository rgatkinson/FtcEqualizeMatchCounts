﻿using System.Collections.Generic;
using FEMC.Enums;

namespace FEMC.DAL
    {
    // A match played at this event (as opposed to one played historically in a previous (league) event)
    abstract class ThisEventMatch : Match
        {
        //----------------------------------------------------------------------------------------
        // Accessing
        //----------------------------------------------------------------------------------------

        protected FMSEventId fmsEventId;
        public FMSScheduleDetailId FMSScheduleDetailId;

        public virtual FMSEventId FMSEventId => fmsEventId?.Value==null ? Scheduled.FMSEventId : fmsEventId;
        public virtual ScheduledMatch Scheduled => Database.ScheduledMatchesById[FMSScheduleDetailId];

        public override string EventCode => Equals(Database.ThisFMSEventId, FMSEventId) ? Database.ThisEventCode : null;
        public override long MatchNumber => Scheduled.MatchNumber;
        public override bool IsEqualizationMatch => Scheduled.IsEqualizationMatch;
        public override TMatchType MatchType => Scheduled.MatchType;

        public override ICollection<int> PlayedTeams => Scheduled.PlayedTeams;

        //----------------------------------------------------------------------------------------
        // Construction
        //----------------------------------------------------------------------------------------

        protected ThisEventMatch(Database db, FMSEventId eventId, FMSScheduleDetailId detailId) : base(db)
            {
            fmsEventId = eventId;
            FMSScheduleDetailId = detailId;
            }

        }
    }
