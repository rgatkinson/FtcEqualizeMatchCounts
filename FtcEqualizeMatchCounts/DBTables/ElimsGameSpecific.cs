namespace FEMC.DBTables
    {
    class ElimsGameSpecific : PhaseGameSpecific
        {
        public ElimsGameSpecific(Database database) : base(database)
            {
            }

        public override string TableName => "elimsGameSpecific";
        }
    }