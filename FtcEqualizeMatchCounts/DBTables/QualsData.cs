namespace FEMC.DBTables
    {
    class QualsData : PhaseData
        {
        public QualsData(Database database) : base(database)
            {
            }

        public override string TableName => "qualsData";
        }
    }