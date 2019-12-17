namespace FEMC
    {
    //--------------------------------------------------------------------------------------------------------------------------
    // Quals
    //--------------------------------------------------------------------------------------------------------------------------

    class Table_Quals : Table<Table_Quals.Row>
        {
        public class Row : TableRow
            {
            public long Match;  // small integer match number: 1, 2, 3, 4, 5, ...; see ScheduleDetail.MatchNumber
            public long Red1;
            public BooleanAsInteger Red1Surrogate;
            public long Red2;
            public BooleanAsInteger Red2Surrogate;
            public long Blue1;
            public BooleanAsInteger Blue1Surrogate;
            public long Blue2;
            public BooleanAsInteger Blue2Surrogate;
            }

        public Table_Quals(Database database) : base(database)
            {
            }

        public override string TableName => "quals";
        }
    }