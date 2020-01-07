using System;
using System.Collections.Generic;
using System.Linq;
using FEMC.Enums;

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

        public override TMatchType MatchType => EventCode == Database.ThisEventCode
            ? Database.ScheduledMatchesByNumber[MatchNumber].MatchType
            : TMatchType.QUALS; // by definition: historical matches are in meets, which only quals

        public override bool Plays(Team team) => Teams.Contains(team);

        //----------------------------------------------------------------------------------------
        // Construction
        //----------------------------------------------------------------------------------------

        public LeagueHistoryMatch(Database db) : base(db)
            {
            }

        public static void DetermineLeagueMatchesThatCount(Database db, int matchesToConsider) // modeled after LeagueSubsystem.CalculateLeagueRankings
            {
            MakeLeagueHistoryMatches(db);

            IDictionary<long, ISet<MatchResult>> history = GetLeagueHistory(db);

            var teamsToConsider = db.TeamsByNumber.Keys;
            foreach (var tx in teamsToConsider)
                {
                ISet<MatchResult> teamHistorySet = history.ContainsKey(tx) ? history[tx] : null;
                List<MatchResult> teamHistory = new List<MatchResult>(teamHistorySet ?? new HashSet<MatchResult>());
                List<MatchResult> historyFromLeagueTournament = new List<MatchResult>();

                // Remove matches for *this* event: we want history only
                if (db.ThisEvent.Type == TEventType.LEAGUE_TOURNAMENT || true) // w/o the "|| true", this tool could only be run pre-match: this event's matches are in other tables
                    {
                    for (int i = 0; i < teamHistory.Count; ++i)
                        {
                        if (teamHistory[i].EventCode == db.ThisEventCode)
                            {
                            historyFromLeagueTournament.Add(teamHistory[i]);
                            teamHistory.RemoveAt(i);
                            i--;
                            }
                        }
                    }

                teamHistory.Sort(); // MatchResult has a semantically significant sort-order

                List<MatchResult> usedMatchResults = new List<MatchResult>(teamHistory.Take(Math.Min(matchesToConsider, teamHistory.Count)));

                List<LeagueHistoryMatch> usedMatches = new List<LeagueHistoryMatch>();
                foreach (var matchResult in usedMatchResults)
                    {
                    Tuple<string, long> key = new Tuple<string, long>(matchResult.EventCode, matchResult.MatchNumber);
                    usedMatches.Add(db.LeagueHistoryMatchesByEventAndMatchNumber[key]);
                    }

                if (db.TeamsByNumber.TryGetValue(tx, out Team team))
                    {
                    team.LeagueHistoryMatchesThatCount = usedMatches;
                    }
                }
            }

        public static void MakeLeagueHistoryMatches(Database db)
            {
            // (Team, Event Code, Match) is the logical primary key. We clump four teams together into a match

            db.LeagueHistoryMatchesByEventAndMatchNumber.Clear();
            foreach (var row in db.Tables.LeagueHistory.Rows)
                { 
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
                    // Matches for *this* event are culled from other tables
                    if (historicalMatch.Event != db.ThisEvent)
                        {
                        historicalMatch.Event.AddMatch(historicalMatch);
                        }
                    }

                // Add this team to the match if it's a team we know about in this event (if it isn't, then we don't care about it)
                if (db.TeamsByNumber.TryGetValue(row.Team.NonNullValue, out Team team))
                    {
                    historicalMatch.Teams.Add(team);
                    }
                }
            }

        // Returns map from team number to league history matches involving that team
        public static IDictionary<long, ISet<MatchResult>> GetLeagueHistory(Database db) // see SQLiteLeagueDAO.getLeagueHistory()
            {
            IDictionary<long, ISet<MatchResult>> result = new Dictionary<long, ISet<MatchResult>>();
            foreach (var row in db.Tables.LeagueHistory.Rows)
                {
                MatchResult matchResult = new MatchResult()
                    {
                    TeamNumber = row.Team.NonNullValue,
                    EventCode = row.EventCode.NonNullValue,
                    MatchNumber = row.Match.NonNullValue,
                    RankingPoints = row.RankingPoints.NonNullValue,
                    TieBreakingPoints = row.TieBreakingPoints.NonNullValue,
                    Score = row.Score.NonNullValue,
                    DQorNoShow = row.DQorNoShow.NonNullValue,
                    Outcome = EnumUtil.From<TMatchOutcome>(row.MatchOutcome.NonNullValue)
                    };

                if (!result.TryGetValue(matchResult.TeamNumber, out ISet<MatchResult> set))
                    {
                    set = new HashSet<MatchResult>();
                    result[matchResult.TeamNumber] = set;
                    }

                set.Add(matchResult);
                }

            foreach (var teamNumber in db.TeamsByNumber.Keys)
                {
                if (!result.ContainsKey(teamNumber))
                    {
                    result[teamNumber] = new HashSet<MatchResult>();
                    }
                }

            return result;
            }
        }
    }
