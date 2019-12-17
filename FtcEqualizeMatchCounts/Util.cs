using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace FEMC
    {
    static class MiscUtil
        {
        // https://stackoverflow.com/questions/1600962/displaying-the-build-date
        // http://msdn.microsoft.com/en-us/library/ms680313

        struct _IMAGE_FILE_HEADER
            {
            public ushort Machine;
            public ushort NumberOfSections;
            public uint TimeDateStamp;
            public uint PointerToSymbolTable;
            public uint NumberOfSymbols;
            public ushort SizeOfOptionalHeader;
            public ushort Characteristics;
            };

        // Not actually useful with -deterministic:
        // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-options/deterministic-compiler-option
        public static DateTime GetBuildDateTimeFromCoff(Assembly assembly)
            {
            var path = assembly.Location;
            if (File.Exists(path))
                {
                var buffer = new byte[Math.Max(Marshal.SizeOf(typeof(_IMAGE_FILE_HEADER)), 4)];
                using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
                    {
                    fileStream.Position = 0x3C;
                    fileStream.Read(buffer, 0, 4);
                    fileStream.Position = BitConverter.ToUInt32(buffer, 0); // COFF header offset
                    fileStream.Read(buffer, 0, 4); // "PE\0\0"
                    fileStream.Read(buffer, 0, buffer.Length);
                    }
                var pinnedBuffer = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                try
                    {
                    var coffHeader = (_IMAGE_FILE_HEADER)Marshal.PtrToStructure(pinnedBuffer.AddrOfPinnedObject(), typeof(_IMAGE_FILE_HEADER));
                    uint secondsSince1970 = coffHeader.TimeDateStamp;
                    DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds((long)secondsSince1970);
                    return dateTimeOffset.LocalDateTime;
                    }
                finally
                    {
                    pinnedBuffer.Free();
                    }
                }
            return new DateTime();
            }
        }
    }
