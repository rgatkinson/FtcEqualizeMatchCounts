using FEMC.Enums;
using System;

namespace FEMC.DAL
    {
    abstract class Match : DBObject
        {
        //----------------------------------------------------------------------------------------
        // Types
        //----------------------------------------------------------------------------------------
            
        public class MatchResult : IComparable<MatchResult> // org.usfirst.ftc.event.MatchResult
            {
            public long    TeamNumber;
            public String  EventCode;
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

            public int CompareTo(MatchResult other) // cloned from org.usfirst.ftc.event.MatchResult.compareTo
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
            }


        //----------------------------------------------------------------------------------------
        // Accessing
        //----------------------------------------------------------------------------------------

        public abstract string EventCode { get; }
        public abstract long MatchNumber { get; }
        public virtual bool IsEqualizationMatch => false;

        public abstract TMatchType MatchType { get; }
        
        public abstract bool Plays(Team team); // Does this match play this team?

        public Event Event => Database.EventsByCode[EventCode];

        public override string ToString() => $"{GetType().Name}: Event={EventCode} MatchNumber={MatchNumber}";

        //----------------------------------------------------------------------------------------
        // Construction
        //----------------------------------------------------------------------------------------
        
        protected Match(Database db) : base(db)
            {
            }
        }
    }
