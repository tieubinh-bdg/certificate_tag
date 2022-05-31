using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace EmbedTenantName.Helpers
{
    public class ByteHelper
    {
        public const UInt16 Pe32Magic = 0x10b;
        public const UInt16 Pe32PlusMagic = 0x20b;
        
        public const int OffsetOfPEHeaderOffset = 0x3c;
        public const int CertificateTableIndex = 4;
        public const ushort coffCharacteristicExecutableImage = 2;
        public const int coffCharacteristicDLL = 0x2000;

        public static byte[] Slice(byte[] arr, int x, int y)
        {
            return arr.Skip(x).Take(y - x).ToArray();
        }

        public static uint CalculateLittleEndianToUInt32(byte[] byteContents)
        {
            return BitConverter.ToUInt32(byteContents, 0);
        }

        public static ushort CalculateLittleEndianToUInt16(byte[] byteContents)
        {
            return BitConverter.ToUInt16(byteContents, 0);
        }

        /// <summary>
        /// https://www.geeksforgeeks.org/how-to-convert-ascii-char-to-byte-in-c-sharp/#:~:text=)%5B0%5D%3B-,Step%201%3A%20Get%20the%20character.,the%20operation%20on%20the%20byte.
        /// Non Unicode char
        /// </summary>
        public static byte ConvertCharToByte(char c)
        {
            string str = c.ToString();
            return Encoding.ASCII.GetBytes(str)[0];
        }

        public static byte[] appendByteArray(byte[] x, byte[] y)
        {
            List<byte> list = new List<byte>();
            list.AddRange(x);
            list.AddRange(y);
            byte[] z = list.ToArray();
            return z;
        }

        public static bool Compare(ReadOnlySpan<byte> a1, ReadOnlySpan<byte> a2)
        {
            return a1.SequenceEqual(a2);
        }


        public static string ConvertByteToString(byte[] byteContents)
        {
            string content = "";
            using (StreamReader reader = new StreamReader(new MemoryStream(byteContents), Encoding.UTF8))
            {
                var x = string.Join("", reader.ReadToEnd().ToCharArray());
                content = string.Join("", x.ToArray());
            }

            return content;
        }
    }
}
