#pragma warning disable 649
using System;

namespace FEMC.DBTables
    {
    // Note: league history does not contain rows for a team's participation as a surrogate
    class LeagueHistory: Table<LeagueHistory.Row, (long, string, long)>
        {
        public class Row : TableRow<Row, (long, string, long)>
            {
            public NullableLong TeamNumber;
            public StringColumn EventCode;
            public NullableLong MatchNumber;
            public NullableLong RankingPoints;
            public NullableLong TieBreakingPoints;
            public NullableLong Score;
            public BooleanAsInteger DQorNoShow;
            public StringColumn MatchOutcome; // "WIN", "LOSS", "TIE"; see TMatchOutcome

            public override (long, string, long) PrimaryKey => (TeamNumber.NonNullValue, EventCode.NonNullValue, MatchNumber.NonNullValue);
            }

        public LeagueHistory(Database database) : base(database)
            {
            }

        public override string TableName => "leagueHistory";
        }
    }
