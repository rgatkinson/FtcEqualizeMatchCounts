using System;

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
                SetValue(new Guid(bytes));
                }
            }

        public void SetValue(string value)
            {
            SetValue(new Guid(value));
            }
        }
    }