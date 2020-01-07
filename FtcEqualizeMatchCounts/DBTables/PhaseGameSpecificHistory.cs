using System;

#pragma warning disable 649
namespace FEMC.DBTables
    {
    abstract class PhaseGameSpecificHistory : Table<PhaseGameSpecificHistory.Row, Tuple<NullableLong, DateTimeAsInteger, NullableLong>>
        {
        public class Row : TableRow<Row, Tuple<NullableLong, DateTimeAsInteger, NullableLong>>
            {
            public NullableLong MatchNumber;
            public DateTimeAsInteger Ts;
            public NullableLong Alliance;      // 0==Red or 1==Blue

            // Autonomous
            public NullableLong FirstReturnedSkyStone;
            public NullableLong SecondBrick;   
            public AutoStones AutoDelivered; 
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
            public NullableLong Capstone1;     
            public NullableLong Capstone2;     
            public NullableLong FoundationMoved;
            public NullableLong Parked1;
            public NullableLong Parked2;

            public override Tuple<NullableLong, DateTimeAsInteger, NullableLong> PrimaryKey => new Tuple<NullableLong, DateTimeAsInteger, NullableLong>(MatchNumber, Ts, Alliance);
            }

        protected PhaseGameSpecificHistory(Database database) : base(database)
            {
            }
        }
    }