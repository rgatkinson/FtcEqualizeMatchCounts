using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEMC.DAL
    {
    class EqualizationMatch : ScheduledMatch
        {
        //----------------------------------------------------------------------------------------
        // Construction
        //----------------------------------------------------------------------------------------

        public EqualizationMatch(Database db, List<Team> teams, List<bool> isSurrogates) : base(db, db.FMSEventId, NewFMSScheduleDetailId())
            {
            CreatedBy = db.EqualizationMatchCreatorName;
            matchNumber = Event.LastMatchNumber + 1;
            Description = $"Evaluation Match {matchNumber}";

            Red1 = teams[0];
            Red2 = teams[1];
            Blue1 = teams[2];
            Blue2 = teams[3];

            Red1Surrogate = isSurrogates[0];
            Red2Surrogate = isSurrogates[1];
            Blue1Surrogate = isSurrogates[2];
            Blue2Surrogate = isSurrogates[3];

            AddToDatabase();
            }

        }
    }
