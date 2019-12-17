using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEMC.DBTables
    {
    class LeagueHistory: Table<LeagueHistory.Row, Tuple<LongColumn, StringColumn, LongColumn>>
        {
        public class Row : TableRow<Tuple<LongColumn, StringColumn, LongColumn>>
            {
            public LongColumn Team;
            public StringColumn EventCode;
            public LongColumn Match;
            public LongColumn RankingPoints;
            public LongColumn TieBreakingPoints;
            public LongColumn Score;
            public BooleanAsInteger DQ;
            public StringColumn MatchOutcome; // "WIN", "LOSS", "TIE"

            public bool MatchIsCounted => true; // WRONG

            public override Tuple<LongColumn, StringColumn, LongColumn> PrimaryKey => new Tuple<LongColumn, StringColumn, LongColumn>(Team, EventCode, Match);
            }

        public LeagueHistory(Database database) : base(database)
            {
            }

        public override string TableName => "leagueHistory";
        }
    }
