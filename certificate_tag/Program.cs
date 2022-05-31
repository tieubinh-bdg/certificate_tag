using EmbedTenantName.Helpers;
using EmbedTenantName.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmbedTenantName
{
    class Program
    {
        static void Main(string[] args)
        {
            GetWorkingFlow();
        }

        static void GetWorkingFlow()
        {
            string filename = "Resource/sw";
            string infilename = filename + ".exe";
            string outfilename = filename + "_tag.exe";
            string tagContents = "tenantname=portaluat.pia.ai&username=tien.dang..........";

            bool removeAppendedTag = false;
            bool setAppendedTag = false;
            bool getAppendedTag = true;

            // Get infile
            byte[] byteContent = File.ReadAllBytes(infilename);
            //var binContent = NewBinary(contents);
            //string base64Encoded = Convert.ToBase64String(buffer);
            //buffer = Convert.FromBase64String(base64Encoded);

            if (removeAppendedTag)
            {
                byteContent = RemoveAppendedTag(byteContent);
            }

            if (setAppendedTag)
            {
                byteContent = SetAppendedTag(byteContent, tagContents);
            }

            if (getAppendedTag)
            {
                // write byte content to file for output

                byte[] outputByteContent = File.ReadAllBytes(infilename);
                System.Console.WriteLine("Appended tag included: " + GetAppendedTag(outputByteContent));
            }
        }

        static byte[] BuildPE32File(byte[] byteContents, string tagContents)
        {
            // todo
            return byteContents;
        }


        static byte[] RemoveAppendedTag(byte[] byteContents)
        {
            // todo
            return BuildPE32File(byteContents, null);
        }

        static byte[] SetAppendedTag(byte[] byteContents, string tagContents)
        {
            // todo
            return BuildPE32File(byteContents, tagContents);
        }


        static (int offset, int size, int sizeOffset) GetAttributeCertificates(byte[] byteContents)
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

            //if (fileHeader.Characteristics & coffCharacteristicExecutableImage == 0)
            //{
            //    throw new Exception("file is not an executable image");
            //}

            //if (fileHeader.Characteristics & coffCharacteristicDLL != 0)
            //{
            //    throw new Exception("file is a DLL");
            //}

            OptionalHeader optionalHeader = OptionalHeader.Create(pe.Take(fileHeader.SizeOfOptionalHeader)
                                                                    .ToArray(), true);
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

        static (byte[] Asn1Data, string appendedTag) ProcessAttributeCertificates(byte[] attributeCertificates)
        {
            if (attributeCertificates.Count() < 8)
            {
                throw new Exception("attribute certificate truncated");
            }

            // This reads a WIN_CERTIFICATE structure from
            // http://msdn.microsoft.com/en-us/library/ms920091.aspx.
            certLen:= binary.LittleEndian.Uint32(certs[:4])

                revision:= binary.LittleEndian.Uint16(certs[4:6])

                certType:= binary.LittleEndian.Uint16(certs[6:8])


                if int(certLen) != len(certs) {
                err = errors.New("multiple attribute certificates found")

                    return

                }

            if revision != attributeCertificateRevision {
                err = fmt.Errorf("unknown attribute certificate revision: %x", revision)

                    return

                }

            if certType != attributeCertificateTypePKCS7SignedData {
                err = fmt.Errorf("unknown attribute certificate type: %d", certType)

                    return

                }

            asn1 = certs[8:]


                if len(asn1) < 2 {
                err = errors.New("ASN.1 structure truncated")

                    return

                }

            asn1Length, err:= lengthAsn1(asn1)

                if err != nil {
                return

                }
            appendedTag = asn1[asn1Length:]

                asn1 = asn1[:asn1Length]


                return
        }

        static string GetAppendedTag(byte[] byteContents)
        {
            (int offset, int size, int certSizeOffset) certAttr = GetAttributeCertificates(byteContents);
            var attributeCertificates = ByteHelper.Slice(byteContents, certAttr.offset, certAttr.offset + certAttr.size);

            (byte[] asn1Data, string appendedTag) = ProcessAttributeCertificates(attributeCertificates);
            //ByteHelper.ConvertByteToString(attributeCertificates);
            return "";
        }
    }
}
