using System;
using System.Collections.Generic;
using System.Linq;
using FEMC.Enums;

namespace FEMC.DAL
    {
    class HistoricalMatch : Match, IPlayedMatch
        {
        //----------------------------------------------------------------------------------------
        // Accessing
        //----------------------------------------------------------------------------------------

        private string eventCode;
        private long matchNumber;
        protected List<MatchResult> matchResults = new List<MatchResult>();
        public ICollection<MatchResult> MatchResults => matchResults;
        public ISet<int> TeamNumbers = new HashSet<int>();

        public override string EventCode => eventCode;
        public override long MatchNumber => matchNumber;

        public override TMatchType MatchType => EventCode == Database.ThisEventCode
            ? Database.ScheduledMatchesByNumber[MatchNumber].MatchType
            : TMatchType.QUALS; // by definition: historical matches are in meets, which only quals

        public override ICollection<int> PlayedTeams => TeamNumbers;

        //----------------------------------------------------------------------------------------
        // Construction
        //----------------------------------------------------------------------------------------

        public HistoricalMatch(Database db, string eventCode, long matchNumber) : base(db)
            {
            this.eventCode = eventCode;
            this.matchNumber = matchNumber;
            }

        //----------------------------------------------------------------------------------------
        // Accessing
        //----------------------------------------------------------------------------------------

        public void AddTeam(int teamNumber)
            {
            TeamNumbers.Add(teamNumber);
            }

        public void AddMatchResult(MatchResult matchResult)
            {
            matchResults.Add(matchResult);
            }
        }
    }
