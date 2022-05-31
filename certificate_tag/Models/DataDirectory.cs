using EmbedTenantName.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmbedTenantName.Models
{
    public class DataDirectory
    {
        public UInt32 VirtualAddress { get; set; }
        public UInt32 Size { get; set; }

        public static DataDirectory Create(byte[] byteContents, bool isLittleEndian = false)
        {
            if (byteContents.Count() != 8)
            {
                // invalid byte
                return null;
            }
            
            if (isLittleEndian == false)
            {
                throw new Exception("Unsupported operation");
            }

            return new DataDirectory()
            {
                VirtualAddress = ByteHelper.CalculateLittleEndianToUInt32(byteContents.Skip(0).Take(4).ToArray()),
                Size = ByteHelper.CalculateLittleEndianToUInt32(byteContents.Skip(4).Take(4).ToArray()),
            };
        }
    }
}
