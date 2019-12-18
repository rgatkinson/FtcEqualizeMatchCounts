using System;
using System.Diagnostics;

namespace FEMC
    {
    abstract class GuidColumn : TableColumn
        {
        public Guid? Value = null;

        public Guid NonNullValue => Value ?? throw MustBeNonNull(GetType().Name);

        // Conversion from sting to blob or visa versa is main use
        public static S CreateFrom<S>(GuidColumn t) where S : GuidColumn, new()
            {
            S result = new S();
            result.Value = t.Value;
            return result;
            }

        public override string ToString()
            {
            return $"{GetType().Name}: { Value?.ToString() ?? "null" }";
            }

        public void SetValue(Guid? guid)
            {
            Value = guid;
            }

        // It seems that the ScoreKeeper database uses big-endian Guids, whereas .Net uses little-endian
        public void LoadDatabaseValue(string value)
            {
            SetValue(new Guid(value));
            }

        public override bool Equals(object obj)
            {
            if (GetType() == obj?.GetType())
                {
                GuidColumn them = (GuidColumn)obj;
                return Equals(Value, them.Value);
                }
            return false;
            }

        public override int GetHashCode()
            {
            return HashCode.Combine(GetType(), Value, 0x309903);
            }
        }
    }