namespace FEMC.DBTables
    {
    //--------------------------------------------------------------------------------------------------------------------------
    // Quals
    //--------------------------------------------------------------------------------------------------------------------------

    class Quals : Table<Quals.Row, LongColumn>
        {
        public class Row : TableRow<LongColumn>
            {
            public LongColumn Match;  // small integer match number: 1, 2, 3, 4, 5, ...; see ScheduleDetail.MatchNumber
            public LongColumn Red1;
            public BooleanAsInteger Red1Surrogate;
            public LongColumn Red2;
            public BooleanAsInteger Red2Surrogate;
            public LongColumn Blue1;
            public BooleanAsInteger Blue1Surrogate;
            public LongColumn Blue2;
            public BooleanAsInteger Blue2Surrogate;

            public override LongColumn PrimaryKey => Match;
            }

        public Quals(Database database) : base(database)
            {
            }

        public override string TableName => "quals";
        }
    }