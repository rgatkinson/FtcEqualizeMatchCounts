namespace FEMC.DBTables
    {
    class QualsCommitHistory : PhaseCommitHistory
        {
        public QualsCommitHistory(Database database) : base(database)
            {
            }

        public override string TableName => "qualsCommitHistory";
        }
    }