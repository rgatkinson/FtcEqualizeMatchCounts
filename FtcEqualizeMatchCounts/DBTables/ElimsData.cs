namespace FEMC.DBTables
    {
    class ElimsData : PhaseData
        {
        public ElimsData(Database database) : base(database)
            {
            }

        public override string TableName => "elimsData";
        }
    }