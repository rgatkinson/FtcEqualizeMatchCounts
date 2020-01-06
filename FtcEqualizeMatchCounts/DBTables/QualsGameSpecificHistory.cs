namespace FEMC.DBTables
    {
    class QualsGameSpecificHistory : PhaseGameSpecificHistory
        {
        public QualsGameSpecificHistory(Database database) : base(database)
            {
            }

        public override string TableName => "qualsGameSpecificHistory";
        }
    }