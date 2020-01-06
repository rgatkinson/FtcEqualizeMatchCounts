using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FEMC.DBTables;

namespace FEMC.DAL
    {
    class SkystoneScores : Scores // See SkytoneScores.Java
        {
        // public long firstBrick; // not used
        public long secondBrick;
        // public long autoDelivered; // not used
        public long autoReturned;
        public long firstBrickReturned;
        public long autoPlaced;
        public long repositioned;
        public long navigated1;
        public long navigated2;
        public byte[] autoDelivered = new byte[6]; // was autoStones

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

        public void Save(PhaseGameSpecific.Row row) // See SQLineMatchDAO.commitMatch
            {
            row.FirstReturnedSkyStone.Value = firstBrickReturned;
            row.SecondBrick.Value = secondBrick;
            row.AutoDelivered.Value = (byte[])autoDelivered.Clone();
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
            autoDelivered = row.AutoDelivered.Value.Take(6).ToArray();
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
