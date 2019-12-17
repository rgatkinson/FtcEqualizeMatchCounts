namespace FEMC
    {
    abstract class BlobColumn : TableColumn
        {
        public byte[] Value;

        public override string ToString()
            {
            return $"{GetType().Name}: { Value?.ToString() ?? "null" }";
            }

        public void SetValue(byte[] value)
            {
            Value = value;
            }

        public override void SetValue(object value)
            {
            SetValue((byte[])value);
            }
        }
    }