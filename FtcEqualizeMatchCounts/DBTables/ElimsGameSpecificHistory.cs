namespace FEMC.DBTables
    {
    class ElimsGameSpecificHistory : PhaseGameSpecificHistory
        {
        public ElimsGameSpecificHistory(Database database) : base(database)
            {
            }

        public override string TableName => "elimsGameSpecificHistory";
        }
    }