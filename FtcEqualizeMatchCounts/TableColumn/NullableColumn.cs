﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEMC
    {
    class NullableColumn<T> : TableColumn where T : struct, IEquatable<T>
        {
        public T? Value;

        public override string ToString()
            {
            return Value?.ToString() ?? "null";
            }

        public override bool Equals(object obj)
            {
            if (GetType() == obj?.GetType())
                {
                NullableColumn<T> them = (NullableColumn<T>)obj;
                return Value.HasValue && them.Value.HasValue && Value.Value.Equals(them.Value.Value) || !Value.HasValue && !them.Value.HasValue;
                }
            return false;
            }

        public override int GetHashCode()
            {
            return HashCode.Combine(GetType(), Value, 0x83791);
            }

        public void SetValue(T? value)
            {
            Value = value;
            }

        public override void SetValue(object value)
            {
            SetValue((T?)value);
            }

        }
    }