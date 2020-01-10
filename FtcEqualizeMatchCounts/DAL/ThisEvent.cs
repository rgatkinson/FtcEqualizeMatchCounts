using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing;
using System.Linq;
using FEMC.DAL.Support;
using FEMC.Enums;

namespace FEMC.DAL
    {
    // TODO: more info/state should be moved here from Database
    class ThisEvent : Event
        {
        //----------------------------------------------------------------------------------------
        // State
        //----------------------------------------------------------------------------------------

        protected readonly IDictionary<long, Ranking> calculatedRankings = new Dictionary<long, Ranking>();

        //----------------------------------------------------------------------------------------
        // Construction
        //----------------------------------------------------------------------------------------

        public ThisEvent(Database db, DBTables.LeagueMeets.Row row, TEventType type, TEventStatus status) : base(db, row, type, status)
            {
            }

        //----------------------------------------------------------------------------------------
        // Accessing
        //----------------------------------------------------------------------------------------

        public ICollection<Team> Teams => Database.Teams;

        public override ICollection<SimpleTeam> SimpleTeams
            {
            get {
                List<SimpleTeam> result = new List<SimpleTeam>();
                foreach (Team team in Teams)
                    {
                    result.Add(new SimpleTeam(team));
                    }
                return result;
                }
            }

        //----------------------------------------------------------------------------------------
        // Rankings
        //----------------------------------------------------------------------------------------

        public List<Ranking> RankingsList
            {
            get {
                List<Ranking> result = new List<Ranking>();
                foreach (var ranking in calculatedRankings.Values)
                    {
                    result.Add(ranking.Copy());
                    }
                result.Sort((r1, r2) => Math.Abs(r1.RankingValue) - Math.Abs(r2.RankingValue));
                return result;
                }
            }

        public void CalculateAndSetRankings()
            {
            calculatedRankings.Clear();
            foreach (var pair in CalculateRankings())
                {
                calculatedRankings.Add(pair);
                }
            SaveRankings(RankingsList);
            }

        protected void SaveRankings(List<Ranking> ranks)
            {
            FMSEventId FMSEventId = Database.ThisFMSEventId;

            foreach (var r in ranks)
                {
                var row = Database.Tables.TeamRanking.NewRow();
                row.FMSEventId = FMSEventId;
                row.FMSTeamId = Database.TeamsByNumber[r.Team.TeamNumber].FMSTeamId;
                row.Ranking.Value = r.RankingValue;
                row.RankChange.Value = 0;
                row.Wins.Value = r.Wins;
                row.Losses.Value = r.Losses;
                row.Ties.Value = r.Ties;
                row.QualifyingScore.Value = r.RankingPoints;
                row.PointsScoredTotal.Value = r.TotalScore;
                row.PointsScoredAverage.Value = r.AverageScore;
                row.PointsScoredAverageChange.Value = 0;
                row.MatchesPlayed.Value = r.MatchesPlayed;
                row.NumDisqualified.Value = r.NumDQedOrNoShow;
                row.SortOrder1.Value = r.RankingPoints;
                row.SortOrder2.Value = r.TieBreakingPoints;
                row.SortOrder3.Value = r.Highest;
                row.SortOrder4.Value = r.Random;
                row.SortOrder5.Value = 0;
                row.SortOrder6.Value = 0;
                row.ModifiedOn.Value = r.Timestamp;
                row.InsertOrReplace();
                }
            }

        protected IDictionary<long, Ranking> CalculateRankings() // MatchSubsystem.java / calculateRankings
            {
            IDictionary<long, Ranking> rankings = new Dictionary<long, Ranking>(); // key is team number

            TRankingType type = Ranking.GetType(Database.ThisEventMatchesPerTeam);
            foreach (var team in SimpleTeams)
                {
                rankings[team.TeamNumber] = new Ranking(type, team);
                }

            AddThisEventRankings(rankings);
            Ranking.SortRankings(rankings.Values, Environment.TickCount);
            return rankings;
            }

        public void AddThisEventRankings(IDictionary<long, Ranking> rankings)
            {
            IEnumerable<MatchPlayedThisEvent> playedMatches = CommittedQualsMatchesByMatchNumber;
            foreach (var m in playedMatches)
                {
                foreach (var result in m.MatchResults)
                    {
                    // Paranoia: be robust about missing teams
                    if (!rankings.TryGetValue(result.TeamNumber, out var ranking))
                        {
                        ranking = new Ranking(Ranking.GetType(Database.ThisEventMatchesPerTeam), new SimpleTeam(result.TeamNumber));
                        rankings[result.TeamNumber] = ranking;
                        }
                    ranking.AddMatch((int)result.RankingPoints, (int)result.Score, (int)result.TieBreakingPoints, result.DQorNoShow, result.Outcome);
                    }
                }
            }

        public IEnumerable<MatchPlayedThisEvent> CommittedQualsMatchesByMatchNumber
            {
            get
                {
                return CommittedQualsMatches.OrderBy(match => match.MatchNumber);
                }
            }

        public IEnumerable<MatchPlayedThisEvent> CommittedQualsMatches
            {
            get {
                foreach (var matches in Database.PlayedMatchesByNumber.Values) // sorted descending by play number
                    {
                    MatchPlayedThisEvent match = matches[0];
                    if (match.IsQual && match.MatchState == TMatchState.Committed)
                        {
                        yield return match;
                        }
                    }
                }
            }

        }
    }
