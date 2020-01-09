using System;
using System.Collections.Generic;

namespace FEMC.DAL.Support
    {
    // Teams that we don't know a whole lot about! Here, used for historical league teams that 
    // aren't in the current event.
    class SimpleTeam
        {
        //--------------------------------------------------------------------------------
        // State
        //--------------------------------------------------------------------------------

        public int TeamNumber;

        //--------------------------------------------------------------------------------
        // Construction
        //--------------------------------------------------------------------------------

        public SimpleTeam(int teamNumber)
            {
            TeamNumber = teamNumber;
            }

        public SimpleTeam(long teamNumber) : this((int)teamNumber)
            {
            }

        public SimpleTeam(Team team) : this(team.TeamNumber)
            {
            }

        //--------------------------------------------------------------------------------
        // Support
        //--------------------------------------------------------------------------------

        public class TCompareByTeamNumber : IEqualityComparer<SimpleTeam>
            {
            public bool Equals(SimpleTeam teamA, SimpleTeam teamB)
                {
                return object.Equals(teamA?.TeamNumber, teamB?.TeamNumber);
                }

            public int GetHashCode(SimpleTeam team)
                {
                return HashCode.Combine(GetType(), team, 0x38903);
                }
            }

        public static TCompareByTeamNumber CompareByTeamNumber = new TCompareByTeamNumber();
        }
    }
