#pragma warning disable 649
using System;

namespace FEMC.DBTables
    {
    // Note: league history does not contain rows for a team's participation as a surrogate
    class LeagueHistory: Table<LeagueHistory.Row, Tuple<NullableLong, StringColumn, NullableLong>>
        {
        public class Row : TableRow<Tuple<NullableLong, StringColumn, NullableLong>>
            {
            public NullableLong Team;
            public StringColumn EventCode;
            public NullableLong Match;
            public NullableLong RankingPoints;
            public NullableLong TieBreakingPoints;
            public NullableLong Score;
            public BooleanAsInteger DQ;
            public StringColumn MatchOutcome; // "WIN", "LOSS", "TIE"

            public bool MatchIsCounted => true; // WRONG

            public override Tuple<NullableLong, StringColumn, NullableLong> PrimaryKey => new Tuple<NullableLong, StringColumn, NullableLong>(Team, EventCode, Match);
            }

        public LeagueHistory(Database database) : base(database)
            {
            }

        public override string TableName => "leagueHistory";
        }
    }
