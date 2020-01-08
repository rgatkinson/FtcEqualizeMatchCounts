using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FEMC.Enums;

namespace FEMC.DAL.Support
    {
    class Commit
        {
        public DateTimeOffset Ts;
        public TCommitType CommitType;

        public Commit()
            {
            }

        public Commit(DateTimeOffset ts, TCommitType commitType)
            {
            Ts = ts;
            CommitType = commitType;
            }
        }
    }
