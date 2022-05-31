using EmbedTenantName.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EmbedTenantName.Models
{
    public class FileHeader
    {
        public const int Size = 20; // bytes
        public UInt16 Machine { get; set; }
        public UInt16 NumberOfSections { get; set; }
        public UInt32 TimeDateStamp { get; set; }
        public UInt32 PointerForSymbolTable { get; set; }
        public UInt32 NumberOfSymbols { get; set; }
        public UInt16 SizeOfOptionalHeader { get; set; }
        public UInt16 Characteristics { get; set; }

        public static FileHeader Create(byte[] byteContents, bool isLittleEndian = false)
        {
            if (byteContents.Count() != 20)
            {
                // invalid byte
                return null;
            }

            if (isLittleEndian == false)
            {
                throw new Exception("Unsupported operation");
            }

            return new FileHeader()
            {
                Machine = ByteHelper.CalculateLittleEndianToUInt16(byteContents.Skip(0).Take(2).ToArray()),
                NumberOfSections = ByteHelper.CalculateLittleEndianToUInt16(byteContents.Skip(2).Take(2).ToArray()),
                TimeDateStamp = ByteHelper.CalculateLittleEndianToUInt32(byteContents.Skip(4).Take(4).ToArray()),
                PointerForSymbolTable = ByteHelper.CalculateLittleEndianToUInt32(byteContents.Skip(8).Take(4).ToArray()),
                NumberOfSymbols = ByteHelper.CalculateLittleEndianToUInt32(byteContents.Skip(12).Take(4).ToArray()),
                SizeOfOptionalHeader = ByteHelper.CalculateLittleEndianToUInt16(byteContents.Skip(16).Take(2).ToArray()),
                Characteristics = ByteHelper.CalculateLittleEndianToUInt16(byteContents.Skip(18).Take(2).ToArray()),
            };
        }
    }
}
