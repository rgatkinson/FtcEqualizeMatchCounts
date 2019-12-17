using System.Collections.Generic;

namespace FEMC.DAL
    {
    class LeagueHistoryMatch : Match
        {
        //----------------------------------------------------------------------------------------
        // Accessing
        //----------------------------------------------------------------------------------------

        public long MatchNumber;
        public ISet<Team> Teams = new HashSet<Team>();
        private string eventCode;

        public override string EventCode => eventCode;

        //----------------------------------------------------------------------------------------
        // Construction
        //----------------------------------------------------------------------------------------

        public LeagueHistoryMatch(Database db) : base(db)
            {
            }

        public static void Process(Database db, DBTables.LeagueHistory.Row row)
            {
            Event theEvent = db.EventsByCode[row.EventCode.Value];
            if (!theEvent.PreviousEventMatchesByNumber.TryGetValue(row.Match.NonNullValue, out LeagueHistoryMatch previousEventMatch))
                {
                previousEventMatch = new LeagueHistoryMatch(db);
                previousEventMatch.eventCode = theEvent.EventCode;
                previousEventMatch.MatchNumber = row.Match.NonNullValue;
                theEvent.PreviousEventMatchesByNumber[previousEventMatch.MatchNumber] = previousEventMatch;
                }

            Team team = db.TeamsByNumber[row.Team.NonNullValue];
            previousEventMatch.Teams.Add(team);
            }
        }
    }
