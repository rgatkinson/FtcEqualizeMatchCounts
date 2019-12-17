namespace FEMC.DBTables
    {
    //--------------------------------------------------------------------------------------------------------------------------
    // Match Schedule
    //--------------------------------------------------------------------------------------------------------------------------

    class MatchSchedule : Table<MatchSchedule.Row, DateTimeColumn>
        {
        public class Row : TableRow<DateTimeColumn>
            {
            public DateTimeAsInteger Start; // primary
            public DateTimeAsInteger End;
            public NullableLong        Type;      // 0 == Qualification, 2 == 5 minute break for consecutive match 
            public StringColumn      Label;

            public override DateTimeColumn PrimaryKey => Start;
            }

        public MatchSchedule(Database database) : base(database)
            {
            }

        public override string TableName => "matchSchedule";
        }
    }