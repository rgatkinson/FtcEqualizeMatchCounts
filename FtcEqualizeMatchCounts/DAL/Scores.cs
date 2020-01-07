﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FEMC.DBTables;

namespace FEMC.DAL
    {
    class Scores
        {
        public Match Match;

        public bool reqRefInteraction;
        public long major;
        public long minor;
        public long adjust;
        public long card1;
        public long card2;
        public long oldCard1;
        public long oldCard2;
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

        public Scores(Match match)
            {
            Match = match;
            }

        protected virtual void SetEqualizationMatch()
            {
            // TODO: WRONG: NOT IMPLEMENTED
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
        }
    }
