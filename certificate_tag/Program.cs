using EmbedTenantName.Helpers;
using System;
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
            string tagContents = "tenantname=portaluat.pia.ai&username=tien.dang";


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

                byte[] outputByteContent = File.ReadAllBytes(outfilename);
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


        static (int offset, int size, int sizeOffset, string err) GetAttributeCertificates(byte[] byteContents)
        {
            int offsetOfPEHeaderOffset = 0x3c; //60

            int certificateTableIndex = 4;

            if (byteContents.Count() < offsetOfPEHeaderOffset + 4)
            {
                throw new Exception("binary truncated");
            }

            uint peOffset = ByteHelper.CalculateLittleEndian(byteContents, offsetOfPEHeaderOffset);
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

            string content;
            using (StreamReader reader = new StreamReader(new MemoryStream(ByteHelper.Slice(pe, 4, pe.Count()).Reverse().ToArray())))
            {
                var x = reader.ReadToEnd().ToCharArray();
                Array.Reverse(x);
                content = string.Join("", x.Skip(x.Count() - 50).ToArray());
            }

            //BinaryReader reader = new BinaryReader();
            //reader.Read()

            //var r = io.Reader(bytes.NewReader(pe[4:]));
            //var fileHeader fileHeader;
            //binary.Read(r, binary.LittleEndian, &fileHeader);

            //FileHeader x = new FileHeader();


            //if (fileHeader.Characteristics & coffCharacteristicExecutableImage == 0)
            //{
            //    throw new Exception("file is not an executable image");
            //}

            //if (fileHeader.Characteristics & coffCharacteristicDLL != 0)
            //{
            //    throw new Exception("file is a DLL");
            //}

            //r = io.LimitReader(r, int64(fileHeader.SizeOfOptionalHeader));
            //var optionalHeader optionalHeader;
            //binary.Read(r, binary.LittleEndian, &optionalHeader);

            //// addressSize is the size of various fields in the Windows-specific
            //// header to follow.
            //int addressSize;
            //switch (optionalHeader.Magic)
            //{
            //    case pe32PlusMagic:
            //        addressSize = 8;

            //    case pe32Magic:
            //        addressSize = 4;

            //        // PE32 contains an additional field in the optional header.
            //        UInt32 baseOfData;
            //        binary.Read(r, binary.LittleEndian, &baseOfData); // check if operation is valid

            //    default:
            //        throw new Exception("unknown magic in optional header: %x", optionalHeader.Magic);
            //}

            //// Skip the Windows-specific header section up to the number of data
            //// directory entries.
            //var toSkip = addressSize + 40 + addressSize * 4 + 4;

            //var skipBuf = make([]byte, toSkip);
            //r.Read(skipBuf); // check if operation is valid

            //// Read the number of directory entries, which is also the last value
            //// in the Windows-specific header.
            //UInt32 numDirectoryEntries;
            //binary.Read(r, binary.LittleEndian, &numDirectoryEntries);

            //if (numDirectoryEntries > 4096)
            //{
            //    throw new Exception("invalid number of directory entries: %d", numDirectoryEntries);
            //}

            //var dataDirectory = make([]dataDirectory, numDirectoryEntries);
            //binary.Read(r, binary.LittleEndian, dataDirectory);

            //if (numDirectoryEntries <= certificateTableIndex)
            //{
            //    throw new Exception("file does not have enough data directory entries for a certificate");

            //}

            //var certEntry = dataDirectory[certificateTableIndex];

            //if (certEntry.VirtualAddress == 0)
            //{
            //    throw new Exception("file does not have certificate data");
            //}


            //var certEntryEnd = certEntry.VirtualAddress + certEntry.Size;

            //if (certEntryEnd < certEntry.VirtualAddress)
            //{
            //    throw new Exception("overflow while calculating end of certificate entry");
            //}

            //if (int(certEntryEnd) != len(byteContents))
            //{
            //    throw new Exception("certificate entry is not at end of file: %d vs %d", int(certEntryEnd), len(byteContents));
            //}

            //byte dummyByte[1];

            //var readErr = r.Read(dummyByte[:]);
            //if (readErr == nil || readErr != io.EOF)
            //{
            //    throw new Exception("optional header contains extra data after data directory");
            //}

            //var offset = int(certEntry.VirtualAddress);
            //var size = int(certEntry.Size);
            //var sizeOffset = int(peOffset) + 4 + fileHeaderSize + int(fileHeader.SizeOfOptionalHeader) - 8 * (int(numDirectoryEntries) - certificateTableIndex) + 4;


            //if (ByteHelper.CalculateLittleEndian(ByteHelper.Slice(byteContents, sizeOffset, byteContents.Count())) != certEntry.Size)
            //{
            //    throw new Exception("internal error when calculating certificate data size offset");
            //}

            return (0, 0, 0, "");
        }

        static string GetAppendedTag(byte[] byteContents)
        {
            // todo: Tien Dang
            (int offset, int size, int certSizeOffset, string err) attr = GetAttributeCertificates(byteContents);
            //if err != nil {
            //    return nil, errors.New("authenticodetag: error parsing headers: " + err.Error())


            //            }

            //attributeCertificates:= contents[offset: offset + size]


            //            asn1Data, appendedTag, err:= processAttributeCertificates(attributeCertificates)


            //            if err != nil {
            //        return nil, errors.New("authenticodetag: error parsing attribute certificate section: " + err.Error())

            //            }

            //    signedData, err:= parseSignedData(asn1Data)


            //            if err != nil {
            //        return nil, err

            return "";
        }
    }
}
