namespace FEMC.DBTables
    {
    //--------------------------------------------------------------------------------------------------------------------------
    // Quals
    //--------------------------------------------------------------------------------------------------------------------------

    class Quals : Table<Quals.Row, NullableLong>
        {
        public class Row : TableRow<NullableLong>
            {
            public NullableLong Match;  // small integer match number: 1, 2, 3, 4, 5, ...; see ScheduleDetail.MatchNumber
            public NullableLong Red1;
            public BooleanAsInteger Red1Surrogate;
            public NullableLong Red2;
            public BooleanAsInteger Red2Surrogate;
            public NullableLong Blue1;
            public BooleanAsInteger Blue1Surrogate;
            public NullableLong Blue2;
            public BooleanAsInteger Blue2Surrogate;

            public override NullableLong PrimaryKey => Match;
            }

        public Quals(Database database) : base(database)
            {
            }

        public override string TableName => "quals";
        }
    }