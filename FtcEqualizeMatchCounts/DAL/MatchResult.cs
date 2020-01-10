using System;
using System.Collections.Generic;
using FEMC.Enums;

namespace FEMC.DAL
    {
    class MatchResult : IComparable<MatchResult> // org.usfirst.ftc.event.MatchResult
        {
        public long    TeamNumber;
        public string  EventCode;
        public long    MatchNumber;
        public long    RankingPoints;
        public long    TieBreakingPoints;
        public long    Score;
        public bool    DQorNoShow;
        public TMatchOutcome Outcome;

        public override string ToString()
            {
            return $"{GetType().Name}: {EventCode}, {MatchNumber}, {TeamNumber}";
            }

        public MatchResult(long teamNumber, string eventCode, long matchNumber, long rp, long tbp, long score, bool dqOrNoShow, TMatchOutcome outcome)
            {
            TeamNumber = teamNumber;
            EventCode = eventCode;
            MatchNumber = matchNumber;
            RankingPoints = rp;
            TieBreakingPoints = tbp;
            Score = score;
            DQorNoShow = dqOrNoShow;
            Outcome = outcome;
            }

        public int CompareTo(MatchResult other) // cloned from org.usfirst.ftc.event.MatchResult.compareTo: this is semantically important to selecting which league history matches count
            {
            long result = TeamNumber - other.TeamNumber;
            if (result == 0)
                {
                result = RankingPoints - other.RankingPoints;
                if (result == 0)
                    {
                    result = TieBreakingPoints - other.TieBreakingPoints;
                    if (result == 0)
                        {
                        result = Score - other.Score;
                        if (result == 0)
                            {
                            result = MatchNumber - other.MatchNumber;
                            if (result == 0)
                                {
                                // ReSharper disable once StringCompareToIsCultureSpecific
                                result = EventCode.CompareTo(other.EventCode);
                                }
                            }
                        }
                    }
                }

            return (int)result;
            }

        //--------------------------------------------------------------------------------
        // Support
        //--------------------------------------------------------------------------------

        // Separate class to keep our core equality logic on MatchResult the same as ScoreKeeper's
        public class TEqualitor : IEqualityComparer<MatchResult>
            {
            public bool Equals(MatchResult a, MatchResult b)
                {
                return object.Equals(a?.TeamNumber, b?.TeamNumber)
                    && object.Equals(a?.EventCode, b?.EventCode)
                    && object.Equals(a?.MatchNumber, b?.MatchNumber)
                    && object.Equals(a?.RankingPoints, b?.RankingPoints)
                    && object.Equals(a?.Score, b?.Score)
                    && object.Equals(a?.DQorNoShow, b?.DQorNoShow)
                    && object.Equals(a?.Outcome, b?.Outcome);
                }

            public int GetHashCode(MatchResult res)
                {
                return HashCode.Combine(GetType(), res, 0x9083141);
                }
            }

        public static TEqualitor Equalitor = new TEqualitor();

        }
    }