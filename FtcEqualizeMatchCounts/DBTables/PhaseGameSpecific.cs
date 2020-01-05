﻿using System;

#pragma warning disable 649
namespace FEMC.DBTables
    {
    abstract class PhaseGameSpecific : Table<PhaseGameSpecific.Row, Tuple<NullableLong, NullableLong>>
        {
        public class Row : TableRow<Tuple<NullableLong, NullableLong>>
            {
            public NullableLong Match;
            public NullableLong Alliance;      // 0==Red or 1==Blue

            // Autonomous
            public BooleanAsInteger FirstReturnedSkyStone;
            public NullableLong SecondBrick;   // ?? historical artifact, always zero ??; see SkystoneScores.java
            public AutoStones   AutoDelivered; // always 6 bytes long, by 'Type': 0==None 1==Stone 2==Skystone. Note: sometimes erroneously stored as 'Text' instead of 'Blob'
            public NullableLong AutoReturned;
            public NullableLong AutoPlaced;
            public BooleanAsInteger Repositioned;
            public BooleanAsInteger Navigated1;
            public BooleanAsInteger Navigated2;

            // Bricks (Driver-Controlled)
            public NullableLong TeleopDelivered;
            public NullableLong TeleopReturned;
            public NullableLong TeleopPlaced;
            public NullableLong TallestTower;

            // End-Game
            public NullableLong Capstone1;      // -1 if no capstone; >=0 is yes capstone, and value gives 'Level'
            public NullableLong Capstone2;      // ditto
            public BooleanAsInteger FoundationMoved;
            public BooleanAsInteger Parked1;
            public BooleanAsInteger Parked2;

            public override Tuple<NullableLong, NullableLong> PrimaryKey => new Tuple<NullableLong, NullableLong>(Match, Alliance);
            }

        public PhaseGameSpecific(Database database) : base(database)
            {
            }
        }
    }