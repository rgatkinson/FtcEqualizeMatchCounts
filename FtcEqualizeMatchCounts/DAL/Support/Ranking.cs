using System;
using FEMC.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FEMC.DAL.Support
    {
    class Ranking : IComparable<Ranking>
        {
        //------------------------------------------------------------------------------------
        // State
        //------------------------------------------------------------------------------------

        public TRankingType RankingType;
        public int RankingPoints;
        public int TieBreakingPoints;
        public int TBPMatchesPlayed;
        public int NumDQedOrNoShow;
        public int RankingValue;
        public int MatchesPlayed;
        public SimpleTeam Team;
        public int Random;
        public DateTimeOffset Timestamp;
        public int Wins = 0;
        public int Losses = 0;
        public int Ties = 0;
        public List<int> HighestMatches = new List<int>();
        public List<int> HighestTBP = new List<int>();

        public override string ToString()
            {
            return $"{GetType().Name}: {Team.TeamNumber}: RP={RankingPoints}, TBP={TieBreakingPoints} Played={MatchesPlayed} Ranking={RankingValue}";
            }

        public int TotalScore => HighestMatches.Sum();
        public int AverageScore => MatchesPlayed==0 ? 0 : TotalScore / MatchesPlayed; // TODO: rounding? ScoreKeeper has bug (they use int division; we mirror)?
        public int Highest => HighestMatches.Count==0 ? 0 : HighestMatches[0];

        public static readonly string RANKING_FORMAT = DecimalFormat("0.00");
        public static readonly string TBP_RANKING_FORMAT = DecimalFormat("##0.0");

        // DisplayedRankingPoints and DisplayedTieBreakingPoints reflect what is *actually* shown on the pit display screen
        public string DisplayedRankingPoints => MatchesPlayed==0 ? "--" : string.Format(RANKING_FORMAT, (double)RankingPoints / (double)MatchesPlayed);
        public string DisplayedTieBreakingPoints => MatchesPlayed == 0 ? "--" : string.Format(TBP_RANKING_FORMAT, (double)TieBreakingPoints / (double)TBPMatchesPlayed);

        protected static string DecimalFormat(string format)
            {
            StringBuilder result = new StringBuilder();
            result.Append("{0:");
            result.Append(format);
            result.Append("}");
            return result.ToString();
            }

        public static TRankingType GetType(int matchCount)
            {
            return matchCount > 6 ? TRankingType.MORE_THAN_6 : TRankingType.LESS_THAN_7;
            }

        //------------------------------------------------------------------------------------
        // Construction
        //------------------------------------------------------------------------------------

        public Ranking(TRankingType rankingType, SimpleTeam team)
            {
            RankingType = rankingType;
            Team = team;
            }

        public Ranking Copy()
            {
            Ranking result = (Ranking)MemberwiseClone();
            result.HighestMatches = new List<int>(result.HighestMatches);
            result.HighestTBP = new List<int>(result.HighestTBP);
            return result;
            }

        public void AddMatch(int rankingPoints, int total, int TBP, bool DQorNoShow, TMatchOutcome outcome)
            {
            RankingPoints += rankingPoints;
            ++MatchesPlayed;
            if (DQorNoShow)
                {
                ++NumDQedOrNoShow;
                if (rankingPoints > 0 || TBP > 0)
                    {
                    throw new InternalErrorException($"WARNING: IMPROPER DQ/NO SHOW PROCESSED. RP:{rankingPoints} TBP:{TBP}");
                    }
                }
            else
                {
                HighestTBP.Add(TBP);
                HighestTBP.Sort((m1, m2) => m2 - m1); // decreasing order
                }

            TBPMatchesPlayed = GetTBPMatchesPlayed(RankingType, MatchesPlayed, NumDQedOrNoShow);
            HighestMatches.Add(total);
            HighestMatches.Sort((m1, m2) => m2 - m1);

            TieBreakingPoints = HighestTBP.Take(Math.Max(TBPMatchesPlayed - NumDQedOrNoShow, 0)).Sum();
            switch (outcome)
                {
                case TMatchOutcome.WIN:
                    ++Wins;
                    break;
                case TMatchOutcome.LOSS:
                    ++Losses;
                    break;
                case TMatchOutcome.TIE:
                    ++Ties;
                    break;
                case TMatchOutcome.UNKNOWN:
                    break;
                }
            }

        protected static int GetTBPMatchesPlayed(TRankingType type, int matchesPlayed, int numDQedOrNoShow)
            {
            int eligibleToDrop = matchesPlayed - numDQedOrNoShow;
            int TBPMatchesPlayed;
            if (type != TRankingType.MORE_THAN_6 && (type != TRankingType.DEFAULT || matchesPlayed <= 6))
                {
                TBPMatchesPlayed = matchesPlayed - Math.Min(eligibleToDrop, 1);
                }
            else
                {
                TBPMatchesPlayed = matchesPlayed - Math.Min(eligibleToDrop, 2);
                }

            TBPMatchesPlayed = Math.Max(1, TBPMatchesPlayed);
            return TBPMatchesPlayed;
            }

        //------------------------------------------------------------------------------------
        // Accessing
        //------------------------------------------------------------------------------------

        public void SetRanking(int ranking, DateTimeOffset timestamp)
            {
            RankingValue = ranking;
            Timestamp = timestamp;
            }

        public int CompareTo(Ranking them)
            {
            if (MatchesPlayed == 0 && them.MatchesPlayed == 0)
                {
                return them.Team.TeamNumber - Team.TeamNumber;
                }
            else if (MatchesPlayed == 0)
                {
                return -1;
                }
            else if (them.MatchesPlayed == 0)
                {
                return 1;
                }
            else
                {
                int score = RankingPoints * them.MatchesPlayed - them.RankingPoints * MatchesPlayed;
                if (score != 0)
                    {
                    return score;
                    }
                else
                    {
                    score = TieBreakingPoints * them.TBPMatchesPlayed - them.TieBreakingPoints * TBPMatchesPlayed;
                    if (score == 0 && HighestMatches.Count > 0 && them.HighestMatches.Count > 0)
                        {
                        score = HighestMatches[0] - them.HighestMatches[0];
                        if (score == 0)
                            {
                            score = Random - them.Random; // TODO: Shouldn't we use random even if there are no highest matches?
                            }
                        }

                    return score;
                    }
                }
            }

        // Sorts by setting properties in each ranking
        public static void SortRankings(IEnumerable<Ranking> rankings, int randomSeed)
            {
            List<Ranking> sortedRankings = new List<Ranking>(rankings);

            int i;
            sortedRankings.Sort((ranking1, ranking2) => ranking2.Team.TeamNumber - ranking1.Team.TeamNumber); // decreasing, don't know why

            List<int> randomNumbers = new List<int>();
            for (i = 0; i < sortedRankings.Count; i++)
                {
                randomNumbers.Add(i);
                }
            randomNumbers.Shuffle(randomSeed);

            for (i = 0; i < sortedRankings.Count; ++i)
                {
                sortedRankings[i].Random = randomNumbers[i];
                }

            sortedRankings.Sort();
            DateTimeOffset timestamp = DateTimeOffset.UtcNow;

            i = sortedRankings.Count - 1;
            for (int rank = 1; i >= 0; ++rank)
                {
                if (sortedRankings[i].MatchesPlayed == 0)
                    {
                    sortedRankings[i].SetRanking(-rank, timestamp);
                    }
                else
                    {
                    sortedRankings[i].SetRanking(rank, timestamp);
                    }
                --i;
                }
            }

        }
    }