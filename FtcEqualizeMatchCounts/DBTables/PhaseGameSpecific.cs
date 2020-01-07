using System;

#pragma warning disable 649
namespace FEMC.DBTables
    {
    abstract class PhaseGameSpecific : Table<PhaseGameSpecific.Row, (long, long)>
        {
        public class Row : TableRow<Row, (long, long)>
            {
            public NullableLong MatchNumber;
            public NullableLong Alliance;      // 0==Red or 1==Blue

            // Autonomous
            public NullableLong FirstReturnedSkyStone;
            public NullableLong SecondBrick;   // ?? historical artifact, always zero ??; see SkystoneScores.java
            public AutoStones   AutoDelivered; // always 6 bytes long, by 'Type': 0==None 1==Stone 2==Skystone. Note: sometimes erroneously stored as 'Text' instead of 'Blob'
            public NullableLong AutoReturned;
            public NullableLong AutoPlaced;
            public NullableLong Repositioned;
            public NullableLong Navigated1;
            public NullableLong Navigated2;

            // Bricks (Driver-Controlled)
            public NullableLong TeleopDelivered;
            public NullableLong TeleopReturned;
            public NullableLong TeleopPlaced;
            public NullableLong TallestTower;

            // End-Game
            public NullableLong Capstone1;      // -1 if no capstone; >=0 is yes capstone, and value gives 'Level'; -2 is 'reqRefInteraction'
            public NullableLong Capstone2;      // ditto
            public NullableLong FoundationMoved;
            public NullableLong Parked1;
            public NullableLong Parked2;

            public override (long, long) PrimaryKey => (MatchNumber.NonNullValue, Alliance.NonNullValue);
            }

        protected PhaseGameSpecific(Database database) : base(database)
            {
            }
        }
    }