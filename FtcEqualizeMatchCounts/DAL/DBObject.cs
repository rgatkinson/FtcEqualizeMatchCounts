namespace FEMC.DAL
    {
    abstract class DBObject
        {
        protected Database Database;

        protected DBObject(Database database)
            {
            this.Database = database;
            }
        }
    }