using FEMC.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using FEMC.DAL.Support;

namespace FEMC.DAL
    {
    class LeagueSubsystem
        {
        //---------------------------------------------------------------------------------------------------
        // State
        //---------------------------------------------------------------------------------------------------

        protected readonly Database Database;
        protected readonly IDictionary<(string, long), HistoricalMatch> HistoricalMatchesByEventAndMatchNumber = new Dictionary<(string, long), HistoricalMatch>(); // key: event code, match number
        protected IDictionary<long, ISet<MatchResult>> LeagueMatchHistory = new Dictionary<long, ISet<MatchResult>>();
        protected IDictionary<long, Ranking> CombinedLeagueRankings = new Dictionary<long, Ranking>();

        //---------------------------------------------------------------------------------------------------
        // Construction
        //---------------------------------------------------------------------------------------------------

        public LeagueSubsystem(Database database)
            {
            Database = database;
            }

        //---------------------------------------------------------------------------------------------------
        // Rankings
        //---------------------------------------------------------------------------------------------------

        public void CalculateAndSetAllLeagueRankings() // leagueSubsystem.java / calculateAndSetAllLeagueRankings()
            {
            CombinedLeagueRankings.Clear();
            CombinedLeagueRankings.AddAll(CalculateLeagueRankings());
            }

        public void CreateExportTemp() // see LeagueRoutesLogic.java / createExportTemp
            {
            // 'Download Archive' from the ScoreKeeper is handled by LeagueRoutsLogic.handleLeagueExport. This happens
            // irrespective of whether the current event is a league event or not. That function in turn calls createExportTemp
            // to create the returned file, which in turn calls LeagueDAO.addMeet to add all the COMMITTED qualification matches 
            // to the leagueHistory of the exported archive. We seek to emulate that here, just to be completely faithful
            // to the ScoreKeeper behavior
            //
            ISet<MatchResult> newResults = new HashSet<MatchResult>(MatchResult.Equalitor);
            foreach (var m in Database.ThisEvent.CommittedQualsMatchesByMatchNumber)
                {
                newResults.AddAll(m.MatchResults);
                }

            ThisEvent thisEvent = Database.ThisEvent;
            var meet = Database.Tables.LeagueMeets.NewRow();
            meet.EventCode.Value = thisEvent.EventCode;
            meet.Name.Value = thisEvent.Name;
            meet.Start.Value = thisEvent.Start;
            meet.End.Value = thisEvent.End;
            meet.InsertOrReplace();

            foreach (var lmr in newResults)
                {
                var ps = Database.Tables.LeagueHistory.NewRow();
                ps.TeamNumber.Value = lmr.TeamNumber;
                ps.EventCode.Value = lmr.EventCode;
                ps.MatchNumber.Value = lmr.MatchNumber;
                ps.RankingPoints.Value = lmr.RankingPoints;
                ps.TieBreakingPoints.Value = lmr.TieBreakingPoints;
                ps.Score.Value = lmr.Score;
                ps.DQorNoShow.Value = lmr.DQorNoShow;
                ps.MatchOutcome.Value = lmr.Outcome.GetStringValue();
                ps.InsertOrReplace();
                }
            }

        //---------------------------------------------------------------------------------------------------
        // Loading
        //---------------------------------------------------------------------------------------------------

        public void Clear()
            {
            HistoricalMatchesByEventAndMatchNumber.Clear();
            LeagueMatchHistory.Clear();
            CombinedLeagueRankings.Clear();
            }

        public void Load()
            {
            MakeHistoricalMatches();
            LeagueMatchHistory = GetLeagueMatchHistory; // team -> MatchResults
            CalculateLeagueRankings(); // for its side-effects
            }

        // Modelled after LeagueSubsystem.java / CalculateLeagueRankings
        // But we only for the current league. Side effect of setting LeagueMatchHistoryUsed in teams
        public IDictionary<long, Ranking> CalculateLeagueRankings()
            {
            IDictionary<long, Ranking> rankings = new Dictionary<long, Ranking>();

            int matchesToConsider = Database.ProgramOptions.LeagueMatchesToConsider;

            foreach (var matches in LeagueMatchHistory.Values)
                {
                foreach (MatchResult matchResult in matches)
                    {
                    (string, long) key = (matchResult.EventCode, matchResult.MatchNumber);
                    HistoricalMatch match = HistoricalMatchesByEventAndMatchNumber[key];
                    match.AddMatchResult(matchResult);
                    }
                }

            // Determine teams to consider
            var teamsToConsider = new HashSet<long>(Database.TeamsByNumber.Keys);
            if (Database.ThisEvent.Type == TEventType.LEAGUE_MEET) // ScoreKeeper had != LEAGUE_TOURNAMENT, but that is more fragile
                {
                teamsToConsider.AddAll(LeagueMatchHistory.Keys);
                }
            foreach (var tx in teamsToConsider)
                {
                rankings[tx] = new Ranking(TRankingType.DEFAULT, new SimpleTeam(tx));
                }

            foreach (var tx in teamsToConsider)
                {
                ISet<MatchResult> teamHistorySet = LeagueMatchHistory.ContainsKey(tx) ? LeagueMatchHistory[tx] : null;
                List<MatchResult> teamHistory = new List<MatchResult>(teamHistorySet ?? new HashSet<MatchResult>());
                List<MatchResult> historyFromLeagueTournament = new List<MatchResult>();

                // Remove matches for *this* event: we want history only
                if (Database.ThisEvent.Type == TEventType.LEAGUE_TOURNAMENT || true) // w/o the "|| true", this tool could only be run pre-match: this event's matches are in other tables
                    {
                    for (int i = 0; i < teamHistory.Count; ++i)
                        {
                        if (teamHistory[i].EventCode == Database.ThisEventCode)
                            {
                            historyFromLeagueTournament.Add(teamHistory[i]);
                            teamHistory.RemoveAt(i);
                            i--;
                            }
                        }
                    }

                teamHistory.Sort(); // MatchResult has a semantically significant sort-order

                // Determine historical MatchResults that count
                List<MatchResult> usedMatchResultsThisTeam = new List<MatchResult>(teamHistory.Take(Math.Min(matchesToConsider, teamHistory.Count)));
                foreach (var res in usedMatchResultsThisTeam)
                    {
                    rankings[res.TeamNumber].AddMatch((int)res.RankingPoints, (int)res.Score, (int)res.TieBreakingPoints, res.DQorNoShow, res.Outcome);
                    }

                // Non-Scorekeeper: determine LeagueMatchHistoryUsed for team: map MatchResults to the actual historical matches
                List<HistoricalMatch> usedHistoricalMatchesThisTeam = new List<HistoricalMatch>();
                foreach (var matchResult in usedMatchResultsThisTeam)
                    {
                    (string, long) key = (matchResult.EventCode, matchResult.MatchNumber);
                    usedHistoricalMatchesThisTeam.Add(HistoricalMatchesByEventAndMatchNumber[key]);
                    }
                if (Database.TeamsByNumber.TryGetValue(tx, out Team team))
                    {
                    team.HistoricalMatchesUsed = usedHistoricalMatchesThisTeam; // must NEVER include matches from this event
                    }
                }

            Database.ThisEvent.AddThisEventRankings(rankings);
            Ranking.SortRankings(rankings.Values, Environment.TickCount);
            return rankings;
            }

        public void MakeHistoricalMatches()
            {
            // (Team, Event Code, Match) is the logical primary key. We clump four teams together into a match

            HistoricalMatchesByEventAndMatchNumber.Clear();
            foreach (var row in Database.Tables.LeagueHistory.Rows)
                {
                // Find the right LeagueHistoryMatch that goes with this row
                (string, long) key = (row.EventCode.NonNullValue, row.MatchNumber.NonNullValue);
                if (!HistoricalMatchesByEventAndMatchNumber.TryGetValue(key, out HistoricalMatch historicalMatch))
                    {
                    historicalMatch = new HistoricalMatch(Database, row.EventCode.NonNullValue, row.MatchNumber.NonNullValue);
                    HistoricalMatchesByEventAndMatchNumber[key] = historicalMatch;

                    // Correlate LeagueHistoryMatch's with their event if they are, in fact, *historical*.
                    // Using only historical matches avoids double-counting as matches accumulate in *this* event.
                    // Matches for *this* event are culled from other tables
                    if (historicalMatch.Event != Database.ThisEvent)
                        {
                        historicalMatch.Event.AddMatch(historicalMatch);
                        }
                    }

                historicalMatch.TeamNumbers.Add((int)row.TeamNumber.NonNullValue);
                }
            }

        protected ISet<LeagueData> GetLeagues // see SQLiteLeagueDAO / getLeagues()
            {
            get {
                ISet<LeagueData> result = new HashSet<LeagueData>();
                foreach (var row in Database.Tables.LeagueInfo.Rows)
                    {
                    result.Add(new LeagueData(row.LeagueCode.NonNullValue, row.Name.Value, row.Country.Value, row.State.Value, row.City.Value));
                    }
                return result;
                }
            }

        // Returns map from team number to league history matches involving that team
        protected IDictionary<long, ISet<MatchResult>> GetLeagueMatchHistory // see SQLiteLeagueDAO.getLeagueHistory()
            {
            get { 
                IDictionary<long, ISet<MatchResult>> result = new Dictionary<long, ISet<MatchResult>>();
                foreach (var row in Database.Tables.LeagueHistory.Rows)
                    {
                    MatchResult matchResult = new MatchResult(
                        row.TeamNumber.NonNullValue,
                        row.EventCode.NonNullValue,
                        row.MatchNumber.NonNullValue,
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
                foreach (var teamNumber in Database.TeamsByNumber.Keys)
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
    }
