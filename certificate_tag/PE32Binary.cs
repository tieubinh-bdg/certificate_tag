using EmbedTenantName.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmbedTenantName
{
    class PE32Binary
    {
        // PE32Binary represents a PE binary.

        public byte[] contents;  // the full file
        public int attrCertOffset; // the offset to the attribute certificates table
        public int certSizeOffset; // the offset to the size of the attribute certificates table
        public byte[] asn1Bytes;      // the PKCS#7, SignedData in DER form.
        public byte[] appendedTag;    // the appended tag, if any.

        //public signedData signedData; // the parsed SignedData structure.
        UInt32 attributeCertificateRevision = 0x200;
        int attributeCertificateTypePKCS7SignedData = 2;

        public (byte[] data, bool ok) AppendedTag()
        {
            bool isAllZero = true;

            for (int i = 0; i < this.appendedTag.Length; i++)
            {
                if (this.appendedTag[i] != 0)
                {
                    isAllZero = false;

                    break;
                }
            }
            if (isAllZero && (this.appendedTag.Length < 8))
            {
                return (null, false);
            }
            return (this.appendedTag, true);
        }

        public byte[] asn1Data()
        {
            return this.asn1Bytes;
        }

        public Int64 certificateOffset()
        {
            return (Int64)this.attrCertOffset;
        }

        
        public byte[] slicesArray(byte[] arr, int x, int y)
        {
            List<byte> slice = new List<byte>();
            for (int i = x; i < y; i++)
            {
                slice.Add(arr[i]);
            }
            return slice.ToArray();
        }
        
        public void PutUint32(byte[] arr, UInt32 x)
        {
            arr[0] = (byte)(x);
            arr[1] = (byte)(x >> 8);
            arr[2] = (byte)(x >> 16);
            arr[3] = (byte)(x >> 24);
        }
        public (byte[], string) buildBinary(byte[] asn1Data, byte[] tag)
        {
            byte[] contentsBB = slicesArray(this.contents, 0, this.certSizeOffset);
            string error;

            while (((asn1Data.Length + tag.Length) & 7) > 0)
            {
                var temp = tag.ToList();
                temp.Add(0);
                tag = temp.ToArray();
            }
            UInt32 attrCertSectionLen = (UInt32)(8 + asn1Data.Length + tag.Length);
            byte[] lengthBytes = new byte[4];
            PutUint32(lengthBytes, attrCertSectionLen);

            contentsBB = ByteArrayHelper.appendByteArray(contentsBB, slicesArray(lengthBytes, 0, 4));
            contentsBB = ByteArrayHelper.appendByteArray(contentsBB, slicesArray(this.contents, this.certSizeOffset + 4, this.attrCertOffset));

            byte[] header = new byte[8];
            
            PutUint32(header, attrCertSectionLen);

            header[4] = (byte)(attributeCertificateRevision);
            header[5] = (byte)(attributeCertificateRevision >> 8);

            header[6] = (byte)(attributeCertificateTypePKCS7SignedData);
            header[7] = (byte)(attributeCertificateTypePKCS7SignedData >> 8);


            contentsBB = ByteArrayHelper.appendByteArray(contentsBB, header);
            contentsBB = ByteArrayHelper.appendByteArray(contentsBB, asn1Data);
            
            return (ByteArrayHelper.appendByteArray(contentsBB, tag), null);
        }

        public (byte[], string) RemoveAppendedTag()
        {
            string error;
            bool ok = this.AppendedTag().ok;
            if (!ok)
            {
                return (null, "authenticodetag: no appended tag found");
            }
            return this.buildBinary(this.asn1Data(), null);
        }
        
        public (byte[], string) SetAppendedTag(byte[] tagContents)
        {
            return this.buildBinary(this.asn1Data(), tagContents);
        }

        // oidChromeTag is an OID that we use for the extension in the superfluous
        // certificate. It's in the Google arc, but not officially assigned.

        /// <summary>
        /// library asn1
        /// </summary>
        //var oidChromeTag = asn1.ObjectIdentifier([]int{1, 3, 6, 1, 4, 1, 11129, 2, 1, 9999})

        // oidChromeTagSearchBytes is used to find the final location of the tag buffer.
        // This is followed by the 2-byte length of the buffer, and then the buffer itself.
        // x060b - OID and length; 11 bytes of OID; x0482 - Octet string, 2-byte length prefix.
        // (In practice our tags are 8206 bytes, so the size fits in two bytes.)

        byte[] oidChromeTagSearchBytes = { 0x06, 0x0b, 0x2b, 0x06, 0x01, 0x04, 0x01, 0xd6, 0x79, 0x02, 0x01, 0xce, 0x0f, 0x04, 0x82 };

        //GetSuperfluousCert()(cert* x509.Certificate, index int, err error)
        //            func(bin* PE32Binary) getSuperfluousCert() (cert* x509.Certificate, index int, err error) {
        //	return getSuperfluousCert(bin.signedData)
        //}


        //        func parseUnixTimeOrDie(unixTime string) time.Time {
        //	t, err := time.Parse(time.UnixDate, unixTime)
        //	if err != nil {
        //		panic(err)

        //    }
        //	return t
        //}

        // SetSuperfluousCertTag returns a PE binary based on bin, but where the
        // superfluous certificate contains the given tag data.
        // The (parsed) bin.signedData is modified; but bin.asn1Bytes, which contains
        // the raw original bytes, is not.

        public (byte[] contents, string err) SetSuperfluousCertTagBinary(byte[] tag)
        {
            //byte[] asn1Bytes = SetSuperfluousCertTag(this.signedData, tag);
            //string err = SetSuperfluousCertTag(this.signedData, tag);
            //if (err != nil) {
            //    return nil, err;

            //}
            return this.buildBinary(this.asn1Bytes, this.appendedTag);

        }
    }
}
