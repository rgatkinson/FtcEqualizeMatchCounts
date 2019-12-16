using System.Collections.Generic;

namespace FtcEqualizeMatchCounts
    {
    // Notes:
    //      FMSMatchId is a UUID
    //          in qualsData, stored as text
    //
    //      FMSScheduleDetailId is a UUID
    //          in qualsData, stored as text
    //          in ScheduleDetail, stored as blob
    //              so is FMSEventId: is this *also* a UUID?
    //          ditto in ScheduleStation
    //              ditto FMSEventId, FMSTeamId therein

    abstract class Table
        {
        protected Database database;

        protected Table(Database database)
            {
            this.database = database;
            }

        public virtual string TableName { get; }

        public List<List<object>> SelectAll()
            {
            string query = $"SELECT * FROM { TableName }";
            List<List<object>> table = database.ExecuteQuery(query);
            return table;
            }
        }

    class Table_Match : Table
        {
        public Table_Match(Database database) : base(database)
            {
            }

        public override string TableName => "Match";
        }

    class Table_matchSchedule : Table
        {
        public Table_matchSchedule(Database database) : base(database)
            {
            }

        public override string TableName => "matchSchedule";
        }

    class Table_quals : Table
        {
        public Table_quals(Database database) : base(database)
            {
            }

        public override string TableName => "quals";
        }
    }
