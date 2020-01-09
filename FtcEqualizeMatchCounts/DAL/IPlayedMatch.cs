using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEMC.DAL
    {
    interface IPlayedMatch
        {
        ICollection<MatchResult> MatchResults { get; }

        long MatchNumber { get; }
        }
    }
