using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEMC.DAL
    {
    abstract class Match : DBObject
        {
        //----------------------------------------------------------------------------------------
        // Types
        //----------------------------------------------------------------------------------------

        public enum TAlliance
            {
            Red = 1,
            Blue = 2,
            }

        public enum TStation
            {
            Station1 = 1,
            Station2 = 2,
            }

        public enum TTournamentLevel // org.usfirst.ftc.event.management.SQLiteManagementDAO.saveFMSSchedule
            {
            Unknown = -1,
            Other = 0,
            Qualification = 2,
            Eliminations = 3,
            }

        public enum TMatchType // org.usfirst.ftc.event.MatchType
            {
            PRACTICE,
            QUALS,
            ELIMS,
            TEST,
            }

        public enum TFieldType
            {
            Unknown = -1,
            Usual = 1,      // don't actually know what this means
            }

        public enum TStatus
            {
            DefaultValue = 0,   // semantic being 'unplayed' is just a guess
            }

        public enum TRandomization
            {
            DefaultValue = -1,
            }

        public enum TMatchScheduleType
            {
            Match = 0,              // MAY be only qual match; not sure
            AdminBreak = 1,         // admin-created break, such as lunch
            InterMatchBreak = 2,    // scoring-system-created break, such as 5 minute break between matches
            }

        public enum TMatchOutcome // org.usfirst.ftc.event.MatchOutcome, names must be in upper case as the string values are in DB (!)
            {
            WIN,
            LOSS,
            TIE,
            UNKNOWN
            }

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
        
        public abstract bool Plays(Team team);

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
