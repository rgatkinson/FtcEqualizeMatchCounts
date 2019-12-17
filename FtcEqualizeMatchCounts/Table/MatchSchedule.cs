namespace FEMC.DBTables
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
            public LongColumn        Type;      // 0 == Qualification, 2 == 5 minute break for consecutive match 
            public StringColumn      Label;
            }

        public MatchSchedule(Database database) : base(database)
            {
            }

        public override string TableName => "matchSchedule";
        }
    }