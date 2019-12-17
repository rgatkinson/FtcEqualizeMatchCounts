using System;

namespace FEMC
    {
    abstract class TableColumn
        {
        public virtual void SetValue(object value)
            {
            throw new NotImplementedException($"{GetType()}.SetValue()");
            }
        }
    }