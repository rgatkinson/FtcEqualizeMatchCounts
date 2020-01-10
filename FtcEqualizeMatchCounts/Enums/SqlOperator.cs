using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq.Expressions;

namespace FEMC.Enums
    {
    enum SqlOperator
        {
        [StringValue("=")] EQUAL,
        [StringValue("<")] LT,
        [StringValue("<=")] LE,
        [StringValue(">")] GT,
        [StringValue(">=")] GE,
        }

    static class SqlOperatorUtil
        {
        // TODO: this is *basically* correct, but edge cases could use some polish, perhaps
        public static bool Test(this SqlOperator op, object v1, object v2)
            {
            switch (op)
                {
                case SqlOperator.EQUAL:
                    return Equals(v1, v2);

                case SqlOperator.GE:
                    if (Test(SqlOperator.EQUAL, v1, v2))
                        {
                        return true;
                        }
                    return Test(SqlOperator.LT, v2, v1);
                
                case SqlOperator.GT:
                    return Test(SqlOperator.LT, v2, v1);
                
                case SqlOperator.LE:
                    if (Test(SqlOperator.EQUAL, v1, v2))
                        {
                        return true;
                        }
                    return Test(SqlOperator.LT, v1, v2);
                
                default:
                    if (v1 == null || v2 == null)
                        return false;

                    Type t1 = v1.GetType();
                    Type t2 = v2.GetType();

                    switch (PromotionType(Type.GetTypeCode(t1), Type.GetTypeCode(t2)))
                        {
                        case TypeCode.Single: return (float)v1 < (float)v2;
                        case TypeCode.Double: return (double)v1 < (double)v2;
                        case TypeCode.UInt64: return (UInt64)v1 < (UInt64)v2;
                        case TypeCode.Int64: return (Int64)v1 < (Int64)v2;
                        case TypeCode.Char: return (char)v1 < (char)v2;
                        case TypeCode.DateTime: return (DateTime)v1 < (DateTime)v2;
                        }

                    return false;
                }
            }

        private static TypeCode PromotionType(TypeCode t1, TypeCode t2)
            {
            if (t1==TypeCode.Double || t2==TypeCode.Double)
                return TypeCode.Double;
            if (t1 == TypeCode.Single || t2 == TypeCode.Single)
                return TypeCode.Single;
            if (t1 == TypeCode.Decimal|| t2 == TypeCode.Decimal)
                return TypeCode.Decimal;

            if (t1 == TypeCode.Int64 || t2 == TypeCode.Int64
                || t1 == TypeCode.Int32 || t2 == TypeCode.Int32
                || t1 == TypeCode.Int16 || t2 == TypeCode.Int16
                || t1 == TypeCode.SByte || t2 == TypeCode.SByte)
                return TypeCode.Int64;

            if (t1 == TypeCode.UInt64 || t2 == TypeCode.UInt64
                || t1 == TypeCode.UInt32 || t2 == TypeCode.UInt32
                || t1 == TypeCode.UInt16 || t2 == TypeCode.UInt16
                || t1 == TypeCode.Byte || t2 == TypeCode.Byte)
                return TypeCode.UInt64;

            if (t1 == TypeCode.Char || t2 == TypeCode.Char)
                return TypeCode.Char;

            return TypeCode.Empty;
            }
        }
    }
