using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Data.Sqlite;

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

        public static MemoryStream ReadFully(Stream input)
            {
            byte[] buffer = new byte[1024];
            MemoryStream ms = new MemoryStream();
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                ms.Write(buffer, 0, read);
                }
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
            }

        public static void DumpStackTrance(String name, IndentedTextWriter writer, Exception e)
            {
            writer.WriteLine($"{name}");
            writer.WriteLine($"Exception Thrown: {e.Message}");
            writer.WriteLine($"Stack Trace:");
            writer.Indent++;
            foreach (var frame in e.StackTrace.Split('\n'))
                {
                writer.WriteLine(frame.Trim());
                }
            writer.Indent--;
            }
        }

    public class StringValue : Attribute
        {
        public string Value { get; private set; }

        public StringValue(string value)
            {
            Value = value;
            }
        }

    public class ColumnName : Attribute
        {
        public string Value { get; private set; }

        public ColumnName(string value)
            {
            Value = value;
            }
        }

    public static class FieldUtil
        {
        public static string GetColumnName(this FieldInfo fieldInfo)
            {
            string stringValue = null;
            ColumnName[] attrs = fieldInfo.GetCustomAttributes(typeof(ColumnName), false) as ColumnName[];
            if (attrs.Length > 0)
                {
                stringValue = attrs[0].Value;
                }
            return stringValue;
            }
        }

    public static class EnumUtil
        {
        public static IEnumerable<T> GetValues<T>()
            {
            return Enum.GetValues(typeof(T)).Cast<T>();
            }

        public static T From<T>(string value) where T: Enum
            {
            foreach (T t in GetValues<T>())
                {
                if (t.ToString() == value)
                    {
                    return t;
                    }
                }
            return default(T);
            }

        public static T From<T>(int value) where T : Enum
            {
            if (Enum.IsDefined(typeof(T), value))
                {
                return (T)Enum.ToObject(typeof(T), value);
                }
            return default(T);
            }

        public static T From<T>(long value) where T : Enum
            {
            return From<T>((int) value);
            }

        public static string GetStringValue(this Enum value)
            {
            string stringValue = value.ToString();
            Type type = value.GetType();
            FieldInfo fieldInfo = type.GetField(value.ToString());
            StringValue[] attrs = fieldInfo.GetCustomAttributes(typeof(StringValue), false) as StringValue[];
            if (attrs.Length > 0)
                {
                stringValue = attrs[0].Value;
                }
            return stringValue;
            }
        }

    public static class SqlUtil
        {
        public static int ExecuteNonQuery(this SqliteConnection connection, string commandText, params SqliteParameter[] parameters)
            {
            using (SqliteCommand command = connection.CreateCommand())
                {
                command.CommandText = commandText;
                command.Parameters.AddRange((IEnumerable<SqliteParameter>)parameters);
                return command.ExecuteNonQuery();
                }
            }

        public static T ExecuteScalar<T>(this SqliteConnection connection, string commandText, params SqliteParameter[] parameters)
            {
            return (T)connection.ExecuteScalar(commandText, parameters);
            }

        private static object ExecuteScalar(this SqliteConnection connection, string commandText, params SqliteParameter[] parameters)
            {
            using (SqliteCommand command = connection.CreateCommand())
                {
                command.CommandText = commandText;
                command.Parameters.AddRange((IEnumerable<SqliteParameter>)parameters);
                return command.ExecuteScalar();
                }
            }
        }

    }

