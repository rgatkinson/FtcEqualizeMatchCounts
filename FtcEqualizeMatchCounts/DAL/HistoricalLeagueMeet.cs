using System.Collections.Generic;
using FEMC.DAL.Support;
using FEMC.Enums;

namespace FEMC.DAL
    {
    class HistoricalLeagueMeet : Event
        {
        //----------------------------------------------------------------------------------------
        // Construction
        //----------------------------------------------------------------------------------------

        public HistoricalLeagueMeet(Database db, DBTables.LeagueMeets.Row row, TEventType type, TEventStatus status) : base(db, row, type, status)
            {
            }

        //----------------------------------------------------------------------------------------
        // Accessing
        //----------------------------------------------------------------------------------------

        public override ICollection<SimpleTeam> SimpleTeams
            {
            get {
                ISet<SimpleTeam> result = new HashSet<SimpleTeam>(SimpleTeam.CompareByTeamNumber);

                foreach (var row in Database.Tables.LeagueHistory.Rows)
                    {
                    if (row.EventCode.NonNullValue == EventCode)
                        {
                        result.Add(new SimpleTeam(row.TeamNumber.NonNullValue));
                        }
                    }

                return result;
                }
            }

        }
    }
