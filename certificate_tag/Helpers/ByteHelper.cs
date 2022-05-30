using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmbedTenantName.Helpers
{
    public class ByteHelper
    {
        public static byte[] Slice(byte[] arr, int x, int y)
        {
            return arr.Skip(x).Take(y-x).ToArray();
        }

        public static UInt32 CalculateLittleEndian(byte[] byteContents, int offset)
        {
            return BitConverter.ToUInt32(byteContents, offset);
        }

        /// <summary>
        /// https://www.geeksforgeeks.org/how-to-convert-ascii-char-to-byte-in-c-sharp/#:~:text=)%5B0%5D%3B-,Step%201%3A%20Get%20the%20character.,the%20operation%20on%20the%20byte.
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
    }
}
