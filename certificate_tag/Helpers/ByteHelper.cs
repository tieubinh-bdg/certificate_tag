using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public const ushort CoffCharacteristicExecutableImage = 2;
        public const int CoffCharacteristicDLL = 0x2000;

        // Despite MSDN claiming that 0x100 is the only, current revision - in
        // practice it's 0x200.
        public const UInt32 AttributeCertificateRevision = 0x200;
        public const UInt32 AttributeCertificateTypePKCS7SignedData = 2;

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

        public static bool Compare(ReadOnlySpan<byte> a1, ReadOnlySpan<byte> a2)
        {
            return a1.SequenceEqual(a2);
        }

        public static string ConvertByteToString(byte[] byteContents)
        {
            if (byteContents == null || byteContents.Length == 0)
            {
                return "";
            }

            using (StreamReader reader = new StreamReader(new MemoryStream(byteContents), Encoding.UTF8))
            {
                return string.Join("", reader.ReadToEnd().ToCharArray());
            }
        }
        public static void PutUint32(byte[] arr, UInt32 x)
        {
            arr[0] = (byte) x;
            arr[1] = (byte)(x >> 8);
            arr[2] = (byte)(x >> 16);
            arr[3] = (byte)(x >> 24);
        }
        public static void PutUint16(byte[] b, UInt16 v)
        {
            b[0] = (byte)v;
            b[1] = (byte)(v >> 8);
        }
        public static byte[] ConvertStringToByte(string text)
        {
            text = text ?? "";
            return Encoding.ASCII.GetBytes(text);
        }
        public static bool ByteArrayToFile(string fileName, byte[] byteArray)
        {
            try
            {
                using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(byteArray, 0, byteArray.Length);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in process: {0}", ex);
                return false;
            }
        }
        public static byte[] AppendByteArray(byte[] x, byte[] y)
        {
            var z = new byte[x.Length + y.Length];
            x.CopyTo(z, 0);
            y.CopyTo(z, x.Length);

            return z;
        }
    }
}
