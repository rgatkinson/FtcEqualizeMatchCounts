using FEMC.DBTables;

namespace FEMC.DAL.Support
    {
    class Scores
        {
        public Match Match;

        #pragma warning disable 649
        public bool reqRefInteraction;
        #pragma warning restore 649
        public long major;
        public long minor;
        public long adjust;
        public long card1;
        public long card2;
        #pragma warning disable 649
        public long oldCard1;
        public long oldCard2;
        #pragma warning restore 649
        public bool dq1;
        public bool dq2;
        public bool noshow1;
        public bool noshow2;
        public bool noshow3;

        protected long penaltyPoints;
        protected long scoredPoints;
        protected long autonomousPoints;
        protected long teleopPoints;
        protected long endGamePoints;
        protected long teleopToSubtractFromAuto;

        public long PenaltyPoints => penaltyPoints;
        public long ScoredPoints => scoredPoints;
        public long AutonomousPoints => autonomousPoints;
        public long TeleopPoints => teleopPoints;
        public long EndGamePoints => endGamePoints;

        public Scores(Match match)
            {
            Match = match;
            }

        public void SetDqFromCard()
            {
            if (card1 >= 2)
                {
                dq1 = true;
                }
            if (card2 >= 2)
                {
                dq2 = true;
                }
            }

        protected virtual void SetEqualizationMatch()
            {
            major = 0;
            minor = 0;
            adjust = 0;
            card1 = 0;
            card2 = 0;
            dq1 = false;
            dq2 = false;
            noshow1 = false;
            noshow2 = false;
            noshow3 = false;
            }

        public void Save(QualsScores.Row row)
            {
            row.Card1.Value = card1;
            row.Card2.Value = card2;
            row.DQ1.Value = dq1;
            row.DQ2.Value = dq2;
            row.NoShow1.Value = noshow1;
            row.NoShow2.Value = noshow2;
            row.Major.Value = major;
            row.Minor.Value = minor;
            row.Adjust.Value = adjust;
            }

        public void Save(QualsScoresHistory.Row row)
            {
            row.Card1.Value = card1;
            row.Card2.Value = card2;
            row.DQ1.Value = dq1;
            row.DQ2.Value = dq2;
            row.NoShow1.Value = noshow1;
            row.NoShow2.Value = noshow2;
            row.Major.Value = major;
            row.Minor.Value = minor;
            row.Adjust.Value = adjust;
            }

        public void Load(QualsScores.Row row)
            {
            card1 = row.Card1.NonNullValue;
            card2 = row.Card2.NonNullValue;
            dq1 = row.DQ1.NonNullValue;
            dq2 = row.DQ2.NonNullValue;
            noshow1 = row.NoShow1.NonNullValue;
            noshow2 = row.NoShow2.NonNullValue;
            noshow3 = false;
            major = row.Major.NonNullValue;
            minor = row.Minor.NonNullValue;
            adjust = row.Adjust.NonNullValue;
            }

        public void Load(ElimsScores.Row row)
            {
            card1 = row.Card.NonNullValue;
            card2 = card1;
            dq1 = row.DQ.NonNullValue;
            dq2 = dq1;
            noshow1 = row.NoShow1.NonNullValue;
            noshow2 = row.NoShow2.NonNullValue;
            noshow3 = row.NoShow3.NonNullValue;
            major = row.Major.NonNullValue;
            minor = row.Minor.NonNullValue;
            adjust = row.Adjust.NonNullValue;
            }

        public void Save(ElimsScores.Row row)
            {
            row.Card.Value = card1;
            row.DQ.Value = dq1;
            row.NoShow1.Value = noshow1;
            row.NoShow2.Value = noshow2;
            row.NoShow3.Value = noshow3;
            row.Major.Value = major;
            row.Minor.Value = minor;
            row.Adjust.Value = adjust;
            }
        
        public void Save(ElimsScoresHistory.Row row)
            {
            row.Card.Value = card1;
            row.DQ.Value = dq1;
            row.NoShow1.Value = noshow1;
            row.NoShow2.Value = noshow2;
            row.NoShow3.Value = noshow3;
            row.Major.Value = major;
            row.Minor.Value = minor;
            row.Adjust.Value = adjust;
            }
        }
    }
