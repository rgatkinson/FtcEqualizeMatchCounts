namespace FtcEqualizeMatchCounts
    {
    //--------------------------------------------------------------------------------------------------------------------------
    // Quals
    //--------------------------------------------------------------------------------------------------------------------------

    class Table_quals : Table
        {
        public class Row : TableRow
            {
            public long Match;  // small integer match number: 1, 2, 3, 4, 5, ...; see ScheduleDetail.MatchNumber
            public long Red1;
            public bool Red1Surrogate;
            public long Red2;
            public bool Red2Surrogate;
            public long Blue1;
            public bool Blue1Surrogate;
            public long Blue2;
            public bool Blue2Surrogate;
            }

        public Table_quals(Database database) : base(database)
            {
            }

        public override string TableName => "quals";
        }
    }