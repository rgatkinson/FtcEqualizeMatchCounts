using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;

namespace FEMC
    {
    static class MiscUtil
        {
        public static long ByteSwap(long value) => BitConverter.ToInt64(ByteSwap(BitConverter.GetBytes(value)), 0);
        public static ulong ByteSwap(ulong value) => BitConverter.ToUInt64(ByteSwap(BitConverter.GetBytes(value)), 0);
        public static int ByteSwap(int value) => BitConverter.ToInt32(ByteSwap(BitConverter.GetBytes(value)), 0);
        public static uint ByteSwap(uint value) => BitConverter.ToUInt32(ByteSwap(BitConverter.GetBytes(value)), 0);
        public static short ByteSwap(short value) => BitConverter.ToInt16(ByteSwap(BitConverter.GetBytes(value)), 0);
        public static ushort ByteSwap(ushort value) => BitConverter.ToUInt16(ByteSwap(BitConverter.GetBytes(value)), 0);

        public static Guid ByteSwap(Guid guid)
            {
            byte[] bytes = guid.ToByteArray();
            UInt32 data1 = BitConverter.ToUInt32(bytes, 0);
            UInt16 data2 = BitConverter.ToUInt16(bytes, 4);
            UInt16 data3 = BitConverter.ToUInt16(bytes, 6);

            data1 = ByteSwap(data1);
            data2 = ByteSwap(data2);
            data3 = ByteSwap(data3);

            Array.Copy(BitConverter.GetBytes(data1), 0, bytes, 0, 4);
            Array.Copy(BitConverter.GetBytes(data2), 0, bytes, 4, 2);
            Array.Copy(BitConverter.GetBytes(data3), 0, bytes, 6, 2);

            return new Guid(bytes);
            }
            

        public static byte[] ByteSwap(byte[] bytes)
            {
            byte[] result = new byte[bytes.Length];
            for (int i = 0; i < bytes.Length; i++)
                {
                result[i] = bytes[bytes.Length-1-i];
                }
            return result;
            }

        public static void DumpStackTrance(IndentedTextWriter writer, Exception e)
            {
            writer.WriteLine($"Exception thrown: {e.Message}");
            writer.WriteLine($"Stack Trace:");
            writer.Indent++;
            foreach (var frame in e.StackTrace.Split('\n'))
                {
                writer.WriteLine(frame.Trim());
                }
            writer.Indent--;
            }
        }

    public static class EnumUtil
        {
        public static IEnumerable<T> GetValues<T>()
            {
            return Enum.GetValues(typeof(T)).Cast<T>();
            }
        }
    }
