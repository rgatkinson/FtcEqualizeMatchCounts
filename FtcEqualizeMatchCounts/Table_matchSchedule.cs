using System;

namespace FtcEqualizeMatchCounts
    {
    //--------------------------------------------------------------------------------------------------------------------------
    // Match Schedule
    //--------------------------------------------------------------------------------------------------------------------------

    class Table_matchSchedule : Table
        {
        public class Row : TableRow
            {
            public DateTimeAsInteger Start;
            public DateTimeAsInteger End;
            public long              Type;      // 0 == Qualification, 2 == 5 minute break for consecutive match 
            public string            Label;
            }

        public Table_matchSchedule(Database database) : base(database)
            {
            }

        public override string TableName => "matchSchedule";

        public void Load()
            {
            Load<Row>();
            }
        }
    }