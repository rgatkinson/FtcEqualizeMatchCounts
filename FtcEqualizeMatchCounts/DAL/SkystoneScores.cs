using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FEMC.DBTables;
using FEMC.Enums;

namespace FEMC.DAL
    {
    class SkystoneScores : Scores // See SkytoneScores.Java
        {
        // public long firstBrick; // not used
        public long secondBrick;
        public long autoDelivered;
        public long autoReturned;
        public long firstBrickReturned;
        public long autoPlaced;
        public long repositioned;
        public long navigated1;
        public long navigated2;
        public byte[] autoStones = new byte[6];

        public long teleopDelivered;
        public long teleopReturned;
        public long teleopPlaced;
        public long tallestTower;

        public long capstone1 = -1;
        public long capstone2 = -1;
        public long foundationMoved;
        public long parked1;
        public long parked2;

        private const long AUTO_DELIVERED_VALUE = 2;
        private const long AUTO_PLACED_VALUE = 4;
        private const long NAVIGATED_VALUE = 5;
        private const long REPOSITIONED_VALUE = 10;
        private const long TARGET_BRICK_VALUE = 8;
        private const long TELEOP_DELIVERED_VALUE = 1;
        private const long TELEOP_PLACED_VALUE = 1;
        private const long TOWER_BONUS = 2;
        private const long CAPSTONE_VALUE = 5;
        private const long CAPSTONE_BONUS = 1;
        private const long FOUNDATION_MOVED_VALUE = 15;
        private const long PARKED_VALUE = 5;
        private const long MINOR_PENALTY_VALUE = 5;
        private const long MAJOR_PENALTY_VALUE = 20;

        public SkystoneScores(Match match) : base(match)
            {
            }

        public SkystoneScoreDetail CalculateBreakdown() // see SkystoneScores.java
            {
            scoredPoints = 0;
            penaltyPoints = 5 * minor + 20 * major;
            autoDelivered = 0;

            long i;
            for (i = 0; i < autoStones.Length; ++i)
                {
                if (autoStones[i] > 0)
                    {
                    ++autoDelivered;
                    }
                }

            i = autoDelivered - Math.Max(0, autoReturned);
            long targetReturned = autoReturned > 0 && firstBrickReturned > 0 ? 1 : 0;
            long targets = (autoStones[0] == 2 ? 1 : 0) + (autoStones[1] == 2 ? 1 : 0);
            long navigated = Math.Max(0, navigated1) + Math.Max(0, navigated2);
            long teleopDeliveredNet = Math.Max(0, teleopDelivered) - Math.Max(0, teleopReturned);
            long parked = Math.Max(0, parked1) + Math.Max(0, parked2);
            long capstones = (capstone1 > -1 ? 1 : 0) + (capstone2 > -1 ? 1 : 0);
            long capstoneHeight = (capstone1 > -1 ? capstone1 : 0) + (capstone2 > -1 ? capstone2 : 0);
            SkystoneScoreDetail scoreDetail = new SkystoneScoreDetail();
            scoreDetail.totalPenaltyPoints = penaltyPoints;
            scoreDetail.minorPenalties = minor;
            scoreDetail.majorPenalties = major;
            scoredPoints += scoreDetail.autoTransport = 2 * i - targetReturned * 8 + 8 * targets;
            scoredPoints += scoreDetail.autoPlaced = 4 * Math.Max(0, autoPlaced);
            scoredPoints += scoreDetail.navigated = 5 * navigated;
            scoredPoints += scoreDetail.repositioned = 10 * Math.Max(0, repositioned);
            autonomousPoints = scoredPoints;
            scoredPoints += scoreDetail.teleopTransport = 1 * teleopDeliveredNet;
            scoredPoints += scoreDetail.teleopPlaced = 1 * Math.Max(0, teleopPlaced);
            scoredPoints += scoreDetail.towerBonus = 2 * Math.Max(0, tallestTower);
            teleopPoints = scoredPoints - autonomousPoints;
            teleopToSubtractFromAuto = scoreDetail.teleopPlaced;
            scoredPoints += scoreDetail.capstone = 5 * capstones + 1 * capstoneHeight;
            scoredPoints += scoreDetail.foundationMoved = 15 * Math.Max(0, foundationMoved);
            scoredPoints += scoreDetail.parked = 5 * parked;
            endGamePoints = scoredPoints - autonomousPoints - teleopPoints;
            if (Match.MatchType == TMatchType.ELIMS && card1 >= 2)
                {
                scoredPoints = adjust;
                }
            scoreDetail.totalScore = scoredPoints;
            return scoreDetail;
            }

        protected override void SetEqualizationMatch()
            {
            base.SetEqualizationMatch();
            firstBrickReturned = 0;
            secondBrick = 0;
            autoStones = new byte[6]; // all zero
            autoReturned = 0;
            autoPlaced = 0;
            repositioned = 0;
            navigated1 = 0;
            navigated2 = 0;
            teleopDelivered = 0;
            teleopReturned = 0;
            teleopPlaced = 0;
            tallestTower = 0;
            capstone1 = -1;
            capstone2 = -1;
            foundationMoved = 0;
            parked1 = 0;
            parked2 = 0;
            }

        public SkystoneScoreDetail SetRedEqualizationMatch()
            {
            SetEqualizationMatch();

            return CalculateBreakdown();
            }

        public SkystoneScoreDetail SetBlueEqualizationMatch()
            {
            SetEqualizationMatch();
            autoStones[0] = 2;

            return CalculateBreakdown();
            }

        public void Save(PhaseGameSpecific.Row row) // See SQLineMatchDAO.commitMatch
            {
            row.FirstReturnedSkyStone.Value = firstBrickReturned;
            row.SecondBrick.Value = secondBrick;
            row.AutoDelivered.Value = (byte[])autoStones.Clone();
            row.AutoReturned.Value = autoReturned;
            row.AutoPlaced.Value = autoPlaced;
            row.Repositioned.Value = repositioned;
            row.Navigated1.Value = navigated1;
            row.Navigated2.Value = navigated2;

            row.TeleopDelivered.Value = teleopDelivered;
            row.TeleopReturned.Value = teleopReturned;
            row.TeleopPlaced.Value = teleopPlaced;
            row.TallestTower.Value = tallestTower;

            row.Capstone1.Value = capstone1;
            row.Capstone2.Value = capstone2;
            row.FoundationMoved.Value = foundationMoved;
            row.Parked1.Value = parked1;
            row.Parked2.Value = parked2;
            }

        public void Load(PhaseGameSpecific.Row row) // See SQLineMatchDAO.getQualsMatches()
            {
            firstBrickReturned = row.FirstReturnedSkyStone.NonNullValue;
            secondBrick = row.SecondBrick.NonNullValue;
            autoStones = row.AutoDelivered.Value.Take(6).ToArray();
            autoReturned = row.AutoReturned.NonNullValue;
            autoPlaced = row.AutoPlaced.NonNullValue;
            repositioned = row.Repositioned.NonNullValue;
            navigated1 = row.Navigated1.NonNullValue;
            navigated2 = row.Navigated2.NonNullValue;

            teleopDelivered = row.TeleopDelivered.NonNullValue;
            teleopReturned = row.TeleopReturned.NonNullValue;
            teleopPlaced = row.TeleopPlaced.NonNullValue;
            tallestTower = row.TallestTower.NonNullValue;

            capstone1 = row.Capstone1.NonNullValue;
            capstone2 = row.Capstone2.NonNullValue;
            foundationMoved = row.FoundationMoved.NonNullValue;
            parked1 = row.Parked1.NonNullValue;
            parked2 = row.Parked2.NonNullValue;
            }
        }
    }
