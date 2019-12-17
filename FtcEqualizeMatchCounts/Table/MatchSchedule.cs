using System;

namespace FEMC
    {
    //--------------------------------------------------------------------------------------------------------------------------
    // Match Schedule
    //--------------------------------------------------------------------------------------------------------------------------

    class MatchSchedule : Table<MatchSchedule.Row>
        {
        public class Row : TableRow
            {
            public DateTimeAsInteger Start;
            public DateTimeAsInteger End;
            public long              Type;      // 0 == Qualification, 2 == 5 minute break for consecutive match 
            public string            Label;
            }

        public MatchSchedule(Database database) : base(database)
            {
            }

        public override string TableName => "matchSchedule";
        }
    }