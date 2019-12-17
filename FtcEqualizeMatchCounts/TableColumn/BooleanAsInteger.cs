namespace FEMC
    {
    class BooleanAsInteger : TableColumn // boolean stored as 'integer' in schema instead of 'boolean'
        {
        public bool? Value;

        public bool NonNullValue => Value.Value;

        public override string ToString()
            {
            return Value?.ToString() ?? "null";
            }

        public void SetValue(bool? value)
            {
            Value = value;
            }

        public void SetValue(long? value)
            {
            SetValue(value==null ? (bool?)null : value.Value != 0);
            }

        public override void SetValue(object value)
            {
            SetValue((long?)value);
            }
        }
    }