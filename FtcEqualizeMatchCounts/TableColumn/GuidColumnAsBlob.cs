using System;
using System.Data;
using System.Diagnostics;
using Microsoft.Data.Sqlite;

namespace FEMC
    {
    class GuidColumnAsBlob : GuidColumn
        {
        public override void SaveDatabaseValue(SqliteParameter parameter)
            {
            if (Value.HasValue)
                {
                Guid guid = Value.Value;
                if (BitConverter.IsLittleEndian)
                    {
                    guid = MiscUtil.ByteSwap(guid);
                    }
                SetParameterValue(parameter, guid.ToByteArray());
                }
            else
                {
                SetParameterValue(parameter, null);
                }
            }
        }
    }