﻿#pragma warning disable 649
namespace FEMC.DBTables
    {
    abstract class PhaseResults : Table<PhaseResults.Row, NullableLong>
        {
        public class Row : TableRow<NullableLong>
            {
            public NullableLong Match;
            public NullableLong RedScore;
            public NullableLong BlueScore;
            public NullableLong RedPenaltyCommitted;
            public NullableLong BluePenaltyCommitted;

            public override NullableLong PrimaryKey => Match;
            }

        public PhaseResults(Database database) : base(database)
            {
            }
        }
    }