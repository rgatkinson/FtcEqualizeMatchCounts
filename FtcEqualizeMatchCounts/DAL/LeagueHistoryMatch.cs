using System;
using System.Collections.Generic;

namespace FEMC.DAL
    {
    class LeagueHistoryMatch : Match
        {
        //----------------------------------------------------------------------------------------
        // Accessing
        //----------------------------------------------------------------------------------------

        private string eventCode;
        private long matchNumber;
        public ISet<Team> Teams = new HashSet<Team>();

        public override string EventCode => eventCode;
        public override long MatchNumber => matchNumber;

        public override bool Plays(Team team) => Teams.Contains(team);

        //----------------------------------------------------------------------------------------
        // Construction
        //----------------------------------------------------------------------------------------

        public LeagueHistoryMatch(Database db) : base(db)
            {
            }

        public static void Process(Database db, DBTables.LeagueHistory.Row row)
            {
            // (Team, Event Code, Match) is the logical primary key. We group

            // Find the right LeagueHistoryMatch that goes with this row
            var key = new Tuple<string,long>(row.EventCode.NonNullValue, row.Match.NonNullValue);
            if (!db.LeagueHistoryMatchesByEventAndMatchNumber.TryGetValue(key, out LeagueHistoryMatch historicalMatch))
                {
                historicalMatch = new LeagueHistoryMatch(db);
                historicalMatch.eventCode = row.EventCode.NonNullValue;
                historicalMatch.matchNumber = row.Match.NonNullValue;
                db.LeagueHistoryMatchesByEventAndMatchNumber[key] = historicalMatch;

                // Correlate LeagueHistoryMatch's with their event if they are, in fact, *historical*.
                // Using only historical matches avoids double-counting as matches accumulate in *this* event.
                if (historicalMatch.Event != db.ThisEvent)
                    {
                    historicalMatch.Event.AddMatch(historicalMatch);
                    }
                }

            // Add this team to the match
            Team team = db.TeamsByNumber[row.Team.NonNullValue];
            historicalMatch.Teams.Add(team);
            }
        }
    }
