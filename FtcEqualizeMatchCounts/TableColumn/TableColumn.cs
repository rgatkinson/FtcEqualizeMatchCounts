using System;

namespace FEMC
    {
    abstract class TableColumn
        {
        public virtual void LoadDatabaseValue(object value)
            {
            throw new NotImplementedException($"{GetType()}.LoadDatabaseValue()");
            }
        }
    }