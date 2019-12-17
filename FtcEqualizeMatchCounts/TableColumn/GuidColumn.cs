﻿using System;
using System.Diagnostics;

namespace FEMC
    {
    abstract class GuidColumn : TableColumn
        {
        public System.Guid? Value = null;

        public override string ToString()
            {
            return $"{GetType().Name}: { Value?.ToString() ?? "null" }";
            }

        public void SetValue(System.Guid? guid)
            {
            Value = guid;
            }

        public void SetValue(byte[] bytes)
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
                SetValue(new Guid(bytes));
                }
            }

        public void SetValue(string value)
            {
            SetValue(new Guid(value));
            }

        public override bool Equals(object obj)
            {
            if (GetType() == obj?.GetType())
                {
                GuidColumn them = (GuidColumn)obj;
                return Value == them.Value;
                }
            return false;
            }

        public override int GetHashCode()
            {
            return HashCode.Combine(GetType(), Value, 0x309903);
            }
        }
    }