using System;
using System.Data;
using System.Diagnostics;
using Microsoft.Data.Sqlite;

namespace FEMC
    {
    class GuidColumnAsBlob : GuidColumn
        {
        public override object GetDatabaseValue()
            {
            if (Value.HasValue)
                {
                Guid guid = Value.Value;
                if (BitConverter.IsLittleEndian)
                    {
                    guid = MiscUtil.ByteSwap(guid);
                    }
                return guid.ToByteArray();
                }
            else
                {
                return null;
                }
            }
        }
    }