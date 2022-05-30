using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmbedTenantName.Helpers
{
    public class ByteArrayHelper
    {
        public static byte[] slicesArray(byte[] arr, int x, int y)
        {
            List<byte> slice = new List<byte>();
            for (int i = x; i < y; i++)
            {
                slice.Add(arr[i]);
            }
            return slice.ToArray();
        }
        
        public static byte[] appendByteArray(byte[] x, byte[] y)
        {
            List<byte> list = new List<byte>();
            list.AddRange(x);
            list.AddRange(y);
            byte[] z = list.ToArray();
            return z;
        }
        public static bool EqualByteArr(byte[] a , byte[] b)
        {
            string byteA = Convert.ToBase64String(a);
            string byteB = Convert.ToBase64String(b);
            return a.Equals(b);
        }
 
}
}
