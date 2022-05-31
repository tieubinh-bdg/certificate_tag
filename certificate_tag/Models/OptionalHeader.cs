using EmbedTenantName.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmbedTenantName.Models
{
    class OptionalHeader
    {
        public const int Size = 24; // bytes
        public UInt16 Magic { get; set; }
        public byte MajorLinkerVersion { get; set; }
        public byte MinorLinkerVersion { get; set; }
        public UInt32 SizeOfCode { get; set; }
        public UInt32 SizeOfInitializedData { get; set; }
        public UInt32 SizeOfUninitializedData { get; set; }
        public UInt32 AddressOfEntryPoint { get; set; }
        public UInt32 BaseOfCode { get; set; }

        public static OptionalHeader Create(byte[] byteContents, bool isLittleEndian = false)
        {
            if (byteContents.Count() < 24)
            {
                // invalid byte
                return null;
            }

            if (isLittleEndian == false)
            {
                throw new Exception("Unsupported operation");
            }

            return new OptionalHeader()
            {
                Magic = ByteHelper.CalculateLittleEndianToUInt16(byteContents.Skip(0).Take(2).ToArray()),
                MajorLinkerVersion = byteContents[2],
                MinorLinkerVersion = byteContents[3],
                SizeOfCode = ByteHelper.CalculateLittleEndianToUInt32(byteContents.Skip(4).Take(4).ToArray()),
                SizeOfInitializedData = ByteHelper.CalculateLittleEndianToUInt32(byteContents.Skip(8).Take(4).ToArray()),
                SizeOfUninitializedData = ByteHelper.CalculateLittleEndianToUInt32(byteContents.Skip(12).Take(4).ToArray()),
                AddressOfEntryPoint = ByteHelper.CalculateLittleEndianToUInt32(byteContents.Skip(16).Take(4).ToArray()),
                BaseOfCode = ByteHelper.CalculateLittleEndianToUInt32(byteContents.Skip(20).Take(4).ToArray())
            };
        }
    }
}