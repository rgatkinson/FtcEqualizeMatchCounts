namespace FEMC.DBTables
    {
    class QualsResults : PhaseResults
        {
        public QualsResults(Database database) : base(database)
            {
            }

        public override string TableName => "qualsResults";
        }
    }