using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FEMC.DAL.Support
    {
    internal class FMSSkystoneScoreDetail
        {
        public Stone[] autoStones;
        public long autoDelivered;
        public long autoReturned;
        public bool firstReturnedIsSkystone;
        public long autoPlaced;
        public bool foundationRepositioned;
        public long driverControlledDelivered;
        public long driverControlledReturned;
        public long driverControlledPlaced;
        public long tallestSkyscraper;
        public Robot robot1 = new Robot();
        public Robot robot2 = new Robot();
        public bool foundationMoved;
        public long minorPenalties;
        public long majorPenalties;
        public long autoDeliveryPoints;
        public long autoPlacedPoints;
        public long repositionedPoints;
        public long navigationPoints;
        public long driverControlledDeliveryPoints;
        public long driverControlledPlacedPoints;
        public long skyscraperBonusPoints;
        public long capstonePoints;
        public long parkingPoints;
        public long autonomousPoints;
        public long driverControlledPoints;
        public long endGamePoints;
        public long penaltyPoints;
        public long totalPoints;

        public FMSSkystoneScoreDetail() // for json
            {
            }

        public FMSSkystoneScoreDetail(SkystoneScores score, long penaltyPoints)
            {
            autoStones = new Stone[score.autoStones.Length];

            for (long i = 0; i < score.autoStones.Length; ++i)
                {
                autoStones[i] = score.autoStones[i] == 2 ? Stone.SKYSTONE : score.autoStones[i] == 1 ? Stone.STONE : Stone.NONE;
                }

            autoDelivered = score.autoDelivered;
            autoPlaced = score.autoPlaced;
            autoReturned = score.autoReturned;
            foundationRepositioned = score.repositioned == 1;
            robot1.navigated = score.navigated1 == 1;
            robot2.navigated = score.navigated2 == 1;
            firstReturnedIsSkystone = score.firstBrickReturned == 1;
            driverControlledDelivered = score.teleopDelivered;
            driverControlledReturned = score.teleopReturned;
            driverControlledPlaced = score.teleopPlaced;
            tallestSkyscraper = score.tallestTower;
            robot1.capstoneLevel = score.capstone1;
            robot2.capstoneLevel = score.capstone2;
            foundationMoved = score.foundationMoved == 1;
            robot1.parked = score.parked1 == 1;
            robot2.parked = score.parked2 == 1;
            minorPenalties = score.minor;
            majorPenalties = score.major;
            SkystoneScoreDetail detail = score.CalculateBreakdown();
            autoDeliveryPoints = detail.autoTransport;
            autoPlacedPoints = detail.autoPlaced;
            repositionedPoints = detail.repositioned;
            navigationPoints = detail.navigated;
            driverControlledDeliveryPoints = detail.teleopTransport;
            driverControlledPlacedPoints = detail.teleopPlaced;
            skyscraperBonusPoints = detail.towerBonus;
            capstonePoints = detail.capstone;
            parkingPoints = detail.parked;
            this.penaltyPoints = penaltyPoints;
            totalPoints = Math.Max(detail.totalScore + penaltyPoints, 0);
            endGamePoints = score.EndGamePoints;
            autonomousPoints = score.AutonomousPoints;
            driverControlledPoints = score.TeleopPoints;
            }

        public class Robot
            {
            public bool navigated;
            public bool parked;
            public long capstoneLevel;
            }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum Stone
            {
            NONE,
            STONE,
            SKYSTONE,
            }
        }
    }