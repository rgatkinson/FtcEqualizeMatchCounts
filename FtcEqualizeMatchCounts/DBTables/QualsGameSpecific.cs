namespace FEMC.DBTables
    {
    class QualsGameSpecific : PhaseGameSpecific
        {
        public QualsGameSpecific(Database database) : base(database)
            {
            }

        public override string TableName => "qualsGameSpecific";
        }
    }