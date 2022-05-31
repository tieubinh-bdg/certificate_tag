using EmbedTenantName.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmbedTenantName.Models
{
    public class PE32Binary
    {
        public byte[] Contents { get; set; }    // the full file
        public int AttrCertOffset { get; set; } // the offset to the attribute certificates table
        public int CertSizeOffset { get; set; } // the offset to the size of the attribute certificates table
        public byte[] Asn1Bytes { get; set; }   // the PKCS#7, SignedData in DER form.
        public byte[] AppendedTag { get; set; } // the appended tag, if any.
        //public signedData     *signedData // the parsed SignedData structure.
        
        public static PE32Binary Create(byte[] byteContents)
        {
            (int offset, int size, int certSizeOffset) certAttr = GetAttributeCertificates(byteContents);
            var attributeCertificates = ByteHelper.Slice(byteContents, certAttr.offset, certAttr.offset + certAttr.size);

            (byte[] asn1Data, byte[] appendedTag) = ProcessAttributeCertificates(attributeCertificates);

            return new PE32Binary()
            {
                Contents = byteContents,
                AttrCertOffset = certAttr.offset,
                CertSizeOffset = certAttr.certSizeOffset,
                AppendedTag = appendedTag,
                Asn1Bytes = asn1Data
            };
        }
        
        public string GetAppendedTag()
        {
            if (this.AppendedTag.Length == 0)
            {
                return "";
            }

            return ByteHelper.ConvertByteToString(this.AppendedTag);
        }

        public void RemoveAppendedTag()
        {
        }        

        public void SetAppendedTag(string tagString)
        {

        }

        private static (int offset, int size, int sizeOffset) GetAttributeCertificates(byte[] byteContents)
        {
            if (byteContents.Count() < ByteHelper.OffsetOfPEHeaderOffset + 4)
            {
                throw new Exception("binary truncated");
            }

            uint peOffset = ByteHelper.CalculateLittleEndianToUInt32(byteContents.Skip(ByteHelper.OffsetOfPEHeaderOffset).ToArray());
            if (peOffset < 0 || peOffset + 4 < peOffset)
            {
                throw new Exception("overflow finding PE signature");
            }

            if (byteContents.Count() < peOffset + 4)
            {
                throw new Exception("binary truncated");
            }

            byte[] pe = ByteHelper.Slice(byteContents, (int)peOffset, byteContents.Count());
            byte[] PEtemp = { ByteHelper.ConvertCharToByte('P'), ByteHelper.ConvertCharToByte('E'), 0, 0 };

            if (!ByteHelper.Compare(ByteHelper.Slice(pe, 0, 4), PEtemp))
            {
                throw new Exception("PE header not found at expected offset");
            }
            pe = ByteHelper.Slice(pe, 4, pe.Count());

            FileHeader fileHeader = FileHeader.Create(pe.Take(FileHeader.Size).ToArray(), true);
            pe = ByteHelper.Slice(pe, FileHeader.Size, pe.Count());

            if ((fileHeader.Characteristics & ByteHelper.CoffCharacteristicExecutableImage).Equals(0))
            {
                throw new Exception("file is not an executable image");
            }

            if ((fileHeader.Characteristics & ByteHelper.CoffCharacteristicDLL).CompareTo(0) != 0)
            {
                throw new Exception("file is a dll");
            }

            OptionalHeader optionalHeader = OptionalHeader.Create(pe.Take(fileHeader.SizeOfOptionalHeader).ToArray(), true);
            pe = ByteHelper.Slice(pe, OptionalHeader.Size, pe.Count());

            // addressSize is the size of various fields in the Windows-specific header to follow.
            int addressSize;
            if (optionalHeader.Magic == ByteHelper.Pe32PlusMagic)
            {
                addressSize = 8;
            }
            else if (optionalHeader.Magic == ByteHelper.Pe32Magic)
            {
                addressSize = 4;
                pe = ByteHelper.Slice(pe, 4, pe.Count());
            }
            else
            {
                throw new Exception("unknown magic in optional header: " + optionalHeader.Magic);
            }

            //// Skip the Windows-specific header section up to the number of data
            //// directory entries.
            int toSkip = addressSize + 40 + addressSize * 4 + 4;
            pe = ByteHelper.Slice(pe, toSkip, pe.Count());

            // Read the number of directory entries, which is also the last value
            // in the Windows-specific header.
            UInt32 numDirectoryEntries = ByteHelper.CalculateLittleEndianToUInt32(pe.Take(4).ToArray());
            pe = ByteHelper.Slice(pe, 4, pe.Count());

            if (numDirectoryEntries > 4096)
            {
                throw new Exception("invalid number of directory entries:" + numDirectoryEntries);
            }

            toSkip = 0;
            List<DataDirectory> dataDirectories = new List<DataDirectory>();
            for (int i = 0; i < numDirectoryEntries; i++)
            {
                DataDirectory dataDirectory = DataDirectory.Create(pe.Skip(toSkip).Take(8).ToArray(), true);
                dataDirectories.Add(dataDirectory);
                toSkip += 8;
            }
            pe = ByteHelper.Slice(pe, toSkip, pe.Count());

            if (numDirectoryEntries <= ByteHelper.CertificateTableIndex)
            {
                throw new Exception("file does not have enough data directory entries for a certificate");

            }

            var certEntry = dataDirectories[ByteHelper.CertificateTableIndex];

            if (certEntry.VirtualAddress == 0)
            {
                throw new Exception("file does not have certificate data");
            }

            var certEntryEnd = certEntry.VirtualAddress + certEntry.Size;

            if (certEntryEnd < certEntry.VirtualAddress)
            {
                throw new Exception("overflow while calculating end of certificate entry");
            }

            if (certEntryEnd != byteContents.Count())
            {
                throw new Exception($"certificate entry is not at end of file: {certEntryEnd} vs {byteContents.Length}");
            }

            //byte dummyByte[1];

            //var readErr = r.Read(dummyByte[:]);
            //if (readErr == nil || readErr != io.EOF)
            //{
            //    throw new Exception("optional header contains extra data after data directory");
            //}

            var offset = (int)certEntry.VirtualAddress;
            var size = (int)certEntry.Size;
            var sizeOffset = peOffset + 4 + FileHeader.Size + fileHeader.SizeOfOptionalHeader - 8 * (numDirectoryEntries - ByteHelper.CertificateTableIndex) + 4;

            if (ByteHelper.CalculateLittleEndianToUInt32(ByteHelper.Slice(byteContents, (int)sizeOffset, byteContents.Count())) != certEntry.Size)
            {
                throw new Exception("internal error when calculating certificate data size offset");
            }

            return (offset, size, (int)sizeOffset);
        }

        private static int GetLengthAsn1(byte[] asn1)
        {
            int asn1Length;
            // Read the ASN.1 length of the object.
            if ((asn1[1] & 0x80).Equals(0))
            {
                // Short form length.
                asn1Length = (int)asn1[1] + 2;
            }
            else
            {
                int numBytes = asn1[1] & 0x7f;
                if (numBytes == 0 || numBytes > 2)
                {
                    throw new Exception("bad number of bytes in ASN.1 length: " + numBytes);
                }

                if (asn1.Length < numBytes + 2)
                {
                    throw new Exception("ASN.1 structure truncated");
                }

                asn1Length = (int)asn1[2];
                if (numBytes == 2)
                {
                    asn1Length <<= 8;
                    asn1Length |= (int)asn1[3];
                }

                asn1Length += 2 + numBytes;
            }

            return asn1Length;
        }

        private static (byte[] asn1Data, byte[] appendedTag) ProcessAttributeCertificates(byte[] attributeCertificates)
        {
            if (attributeCertificates.Count() < 8)
            {
                throw new Exception("attribute certificate truncated");
            }

            // This reads a WIN_CERTIFICATE structure from
            // http://msdn.microsoft.com/en-us/library/ms920091.aspx.
            var certLen = ByteHelper.CalculateLittleEndianToUInt32(attributeCertificates.Take(4).ToArray());
            var revision = ByteHelper.CalculateLittleEndianToUInt16(ByteHelper.Slice(attributeCertificates, 4, 6));
            var certType = ByteHelper.CalculateLittleEndianToUInt16(ByteHelper.Slice(attributeCertificates, 6, 8));

            if (certLen != attributeCertificates.Length)
            {
                throw new Exception("multiple attribute certificates found");
            }

            if (revision != ByteHelper.AttributeCertificateRevision)
            {
                throw new Exception("unknown attribute certificate revision: " + revision);
            }

            if (certType != ByteHelper.AttributeCertificateTypePKCS7SignedData)
            {
                throw new Exception("unknown attribute certificate type: " + certType);
            }

            byte[] asn1 = ByteHelper.Slice(attributeCertificates, 8, attributeCertificates.Length);

            if (asn1.Length < 2)
            {
                throw new Exception("ASN.1 structure truncated");
            }

            var asn1Length = GetLengthAsn1(asn1);
            return (ByteHelper.Slice(asn1, 0, asn1Length), ByteHelper.Slice(asn1, asn1Length, asn1.Length));
        }

    }
}
