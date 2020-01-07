using FEMC.DBTables;
using FEMC.Enums;
using System;
using System.Linq;

namespace FEMC.DAL.Support
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

        public static byte[] EqualizationScoreDetails() // Gzip'd bson: carefully crafted for SetRedEqualizationMatch and SetBlueEqualizationMatch
            {
            return new byte[]
                {
                0x1f, 0x8b, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xed, 0x53, 0x4b, 0x4e, 0xc3, 0x30,
                0x10, 0x9d, 0xb6, 0x20, 0xca, 0x3d, 0x38, 0x00, 0x2d, 0xb0, 0x06, 0xfa, 0x01, 0x21, 0x48, 0x09,
                0x4d, 0x37, 0x2c, 0x4d, 0x32, 0xaa, 0x4c, 0x5d, 0x4f, 0xe5, 0x38, 0x91, 0x7a, 0x06, 0x16, 0x1c, 
                0x89, 0x2b, 0x71, 0x04, 0xec, 0x90, 0xb4, 0x89, 0x13, 0x84, 0x90, 0xba, 0x41, 0x90, 0x4d, 0x26,
                0x6f, 0x66, 0x3c, 0x6f, 0x9e, 0xf3, 0x46, 0x07, 0x00, 0x9d, 0x81, 0x48, 0xf0, 0x52, 0x08, 0xce, 
                0x64, 0x88, 0x41, 0x48, 0x0a, 0xe1, 0xb5, 0x03, 0xb0, 0x77, 0x99, 0x68, 0x0a, 0x34, 0x49, 0x8c, 
                0xe1, 0x01, 0x00, 0xda, 0xc7, 0x70, 0x68, 0x5e, 0xc1, 0xed, 0x63, 0x30, 0xbb, 0x9f, 0x8c, 0xa1, 
                0xdd, 0x83, 0x7d, 0xf3, 0x3d, 0xc9, 0xe2, 0x7e, 0x29, 0x3e, 0x29, 0xc5, 0xa7, 0xa5, 0xf8, 0x6c, 
                0x1b, 0x43, 0xcb, 0x1e, 0x3e, 0x42, 0xc1, 0x53, 0x54, 0x18, 0xc1, 0xe7, 0xf3, 0x7e, 0x9e, 0xc1,
                0x53, 0xd4, 0x89, 0x92, 0x1b, 0x14, 0xa0, 0x7b, 0xc5, 0x55, 0xac, 0x0b, 0xf8, 0x26, 0x0e, 0x16,
                0xeb, 0xd8, 0xf2, 0xca, 0x8f, 0xf1, 0x05, 0x0b, 0x2b, 0xd5, 0x94, 0xc8, 0x88, 0x69, 0x4e, 0x72,
                0x8a, 0x2b, 0x8a, 0xb9, 0x8d, 0x6c, 0xbe, 0x35, 0x52, 0x76, 0xdc, 0x90, 0xa4, 0x56, 0x24, 0x04,
                0x46, 0xee, 0xfc, 0x86, 0x92, 0x1a, 0x97, 0x5a, 0x85, 0x33, 0xbd, 0x35, 0x63, 0x06, 0x8d, 0xb5,
                0xe5, 0x18, 0x2a, 0xb6, 0x42, 0xb5, 0x49, 0x75, 0xa6, 0xf4, 0x44, 0xba, 0x07, 0x3d, 0x4b, 0x72,
                0xc2, 0x52, 0x3e, 0x67, 0xda, 0x76, 0x76, 0x7d, 0xa6, 0x16, 0x19, 0xc1, 0x21, 0x5b, 0x65, 0x8b,
                0xdd, 0x61, 0x8a, 0xa2, 0x10, 0xe5, 0x2d, 0x6f, 0xec, 0xff, 0xbc, 0xb1, 0x24, 0x85, 0x47, 0x69,
                0x56, 0xe9, 0x71, 0x49, 0xca, 0x47, 0xc9, 0x84, 0xe6, 0xe6, 0x62, 0x37, 0xb4, 0x3d, 0xf6, 0xdc,
                0x88, 0x97, 0xee, 0x69, 0xed, 0x13, 0x97, 0xba, 0xc8, 0x1d, 0x5d, 0x94, 0xc4, 0xaf, 0x64, 0x4c,
                0x57, 0x59, 0x78, 0x37, 0x97, 0x2f, 0x60, 0x72, 0x6e, 0xe6, 0x8b, 0xfb, 0x59, 0x7f, 0x57, 0xd7,
                0xcc, 0x61, 0x7b, 0x03, 0x03, 0x92, 0x49, 0xec, 0xa6, 0x0b, 0xc9, 0x5c, 0xdc, 0x6a, 0xca, 0xe5,
                0xdc, 0x85, 0xed, 0xae, 0x92, 0x96, 0xe4, 0x1c, 0x64, 0x54, 0xa8, 0xb1, 0x71, 0x3a, 0xc7, 0x32,
                0xba, 0x66, 0xcb, 0xfa, 0x9c, 0x4c, 0xeb, 0xda, 0x6e, 0x33, 0xd2, 0x4c, 0x38, 0x23, 0xcc, 0x0f,
                0x80, 0x51, 0xd5, 0xa0, 0x2f, 0x8e, 0x41, 0x3d, 0xc8, 0x0c, 0xba, 0x35, 0xdb, 0xce, 0xcd, 0x99,
                0xc3, 0xff, 0xe6, 0xfc, 0x0d, 0xe6, 0xac, 0x88, 0xff, 0x67, 0xcd, 0xd9, 0xc4, 0x66, 0xc7, 0xe6,
                0xb4, 0xcf, 0x07, 0x38, 0x55, 0x5e, 0x0a, 0x44, 0x07, 0x00, 0x00,
                };
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

        public void Save(PhaseGameSpecificHistory.Row row) // See SQLineMatchDAO.commitMatch
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
