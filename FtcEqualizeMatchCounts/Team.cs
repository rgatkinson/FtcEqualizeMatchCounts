using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEMC
    {
    abstract class DBObject
        {
        protected Tables Tables;

        protected DBObject(Tables tables)
            {
            Tables = tables;
            }
        }

    class Team : DBObject
        {
        public FMSTeamId TeamId;
        public long TeamNumber;
        public string Name;

        public override string ToString()
            {
            return $"Team {TeamNumber}: {Name}";
            }

        public Team(Tables tables, DBTables.Team.Row row) : base(tables)
            {
            TeamId = row.FMSTeamId;
            TeamNumber = row.TeamNumber.Value.Value;
            Name = row.TeamNameShort.Value;
            }
        }
    }
