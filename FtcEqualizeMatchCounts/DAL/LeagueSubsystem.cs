using FEMC.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FEMC.DAL
    {
    class LeagueSubsystem
        {
        //---------------------------------------------------------------------------------------------------
        // State
        //---------------------------------------------------------------------------------------------------

        protected readonly Database db;
        protected readonly IDictionary<(string, long), LeagueHistoryMatch> MatchesByEventAndMatchNumber = new Dictionary<(string, long), LeagueHistoryMatch>(); // key: event code, match number
        protected IDictionary<long, ISet<MatchResult>> history; // team -> MatchResults

        //---------------------------------------------------------------------------------------------------
        // State
        //---------------------------------------------------------------------------------------------------

        public LeagueSubsystem(Database db)
            {
            this.db = db;
            }

        //---------------------------------------------------------------------------------------------------
        // Rankings
        //---------------------------------------------------------------------------------------------------


        //---------------------------------------------------------------------------------------------------
        // Loading
        //---------------------------------------------------------------------------------------------------

        public void Clear()
            {
            MatchesByEventAndMatchNumber.Clear();
            }

        public void DetermineLeagueMatchesThatCount(int matchesToConsider) // modeled after LeagueSubsystem.CalculateLeagueRankings
            {
            MakeLeagueHistoryMatches();

            history = GetLeagueHistory(); // team -> MatchResults

            foreach (var matches in history.Values)
                {
                foreach (MatchResult matchResult in matches)
                    {
                    (string, long) key = (matchResult.EventCode, matchResult.MatchNumber);
                    LeagueHistoryMatch match = MatchesByEventAndMatchNumber[key];
                    match.AddMatchResult(matchResult);
                    }
                }

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
                    (string, long) key = (matchResult.EventCode, matchResult.MatchNumber);
                    usedMatches.Add(MatchesByEventAndMatchNumber[key]);
                    }

                if (db.TeamsByNumber.TryGetValue(tx, out Team team))
                    {
                    team.LeagueHistoryMatchesThatCount = usedMatches;
                    }
                }
            }

        protected void MakeLeagueHistoryMatches()
            {
            // (Team, Event Code, Match) is the logical primary key. We clump four teams together into a match

            MatchesByEventAndMatchNumber.Clear();
            foreach (var row in db.Tables.LeagueHistory.Rows)
                {
                // Find the right LeagueHistoryMatch that goes with this row
                (string, long) key = (row.EventCode.NonNullValue, row.Match.NonNullValue);
                if (!MatchesByEventAndMatchNumber.TryGetValue(key, out LeagueHistoryMatch historicalMatch))
                    {
                    historicalMatch = new LeagueHistoryMatch(db, row.EventCode.NonNullValue, row.Match.NonNullValue);
                    MatchesByEventAndMatchNumber[key] = historicalMatch;

                    // Correlate LeagueHistoryMatch's with their event if they are, in fact, *historical*.
                    // Using only historical matches avoids double-counting as matches accumulate in *this* event.
                    // Matches for *this* event are culled from other tables
                    if (historicalMatch.Event != db.ThisEvent)
                        {
                        historicalMatch.Event.AddMatch(historicalMatch);
                        }
                    }

                historicalMatch.TeamNumbers.Add((int)row.TeamNumber.NonNullValue);
                }
            }

        // Returns map from team number to league history matches involving that team
        protected IDictionary<long, ISet<MatchResult>> GetLeagueHistory() // see SQLiteLeagueDAO.getLeagueHistory()
            {
            IDictionary<long, ISet<MatchResult>> result = new Dictionary<long, ISet<MatchResult>>();
            foreach (var row in db.Tables.LeagueHistory.Rows)
                {
                MatchResult matchResult = new MatchResult(
                    row.TeamNumber.NonNullValue,
                    row.EventCode.NonNullValue,
                    row.Match.NonNullValue,
                    row.RankingPoints.NonNullValue,
                    row.TieBreakingPoints.NonNullValue,
                    row.Score.NonNullValue,
                    row.DQorNoShow.NonNullValue,
                    EnumUtil.From<TMatchOutcome>(row.MatchOutcome.NonNullValue)
                    );

                if (!result.TryGetValue(matchResult.TeamNumber, out ISet<MatchResult> set))
                    {
                    set = new HashSet<MatchResult>();
                    result[matchResult.TeamNumber] = set;
                    }

                set.Add(matchResult);
                }

            // Make sure that all the teams of the this event are in the history
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
