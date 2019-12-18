using System;
using System.Data;
using System.Diagnostics;
using Microsoft.Data.Sqlite;

namespace FEMC
    {
    class GuidColumnAsBlob : GuidColumn
        {
        public override void LoadDatabaseValue(object value)
            {
            LoadDatabaseValue((byte[])value);
            }

        public void LoadDatabaseValue(byte[] bytes)
            {
            if (bytes == null)
                SetValue((Guid?)null);
            else
                {
                if (bytes.Length == 0)
                    {
                    bytes = new byte[16]; // account for quirky RowVersion seen in practice (are they *really* guids?)
                    }
                Trace.Assert(bytes.Length == 16);
                Guid guid = new Guid(bytes);
                if (BitConverter.IsLittleEndian)
                    {
                    guid = MiscUtil.ByteSwap(guid);
                    }
                SetValue(guid);
                }
            }
        
        public override void SaveDatabaseValue(SqliteParameter parameter)
            {
            if (Value.HasValue)
                {
                Guid guid = Value.Value;
                if (BitConverter.IsLittleEndian)
                    {
                    MiscUtil.ByteSwap(guid);
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