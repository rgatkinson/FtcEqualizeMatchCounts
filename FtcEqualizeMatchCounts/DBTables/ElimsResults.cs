namespace FEMC.DBTables
    {
    class ElimsResults : PhaseResults
        {
        public ElimsResults(Database database) : base(database)
            {
            }

        public override string TableName => "elimsResults";
        }
    }