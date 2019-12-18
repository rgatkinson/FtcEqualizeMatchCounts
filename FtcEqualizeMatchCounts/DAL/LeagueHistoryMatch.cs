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
            if (!db.LeagueHistoryMatchesByNumber.TryGetValue(row.Match.NonNullValue, out LeagueHistoryMatch historicalMatch))
                {
                historicalMatch = new LeagueHistoryMatch(db);
                historicalMatch.eventCode = row.EventCode.NonNullValue;
                historicalMatch.matchNumber = row.Match.NonNullValue;
                db.LeagueHistoryMatchesByNumber[historicalMatch.matchNumber] = historicalMatch;

                if (historicalMatch.Event != db.ThisEvent)
                    {
                    historicalMatch.Event.AddMatch(historicalMatch);
                    }
                }

            Team team = db.TeamsByNumber[row.Team.NonNullValue];
            historicalMatch.Teams.Add(team);
            }
        }
    }
