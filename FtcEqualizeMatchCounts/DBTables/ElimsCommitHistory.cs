namespace FEMC.DBTables
    {
    class ElimsCommitHistory : PhaseCommitHistory
        {
        public ElimsCommitHistory(Database database) : base(database)
            {
            }

        public override string TableName => "elimsCommitHistory";
        }
    }