//using System.Security.Cryptography.X509Certificates;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using EmbedTenantName.Helpers;

//namespace EmbedTenantName
//{
//    struct fileHeader
//    {
//        public UInt16 Machine;
//        public UInt16 NumberOfSections;
//        public UInt32 TimeDateStamp;
//        public UInt32 PointerForSymbolTable;
//        public UInt32 NumberOfSymbols;
//        public UInt16 SizeOfOptionalHeader;
//        public UInt16 Characteristics;
//    };
//    struct optionalHeader
//    {
//        public UInt16 Magic;
//        public UInt64 MajorLinkerVersion;
//        public UInt64 MinorLinkerVersion;
//        public UInt32 SizeOfCode;
//        public UInt32 SizeOfInitializedData;
//        public UInt32 SizeOfUninitializedData;
//        public UInt32 AddressOfEntryPoint;
//        public UInt32 BaseOfCode;
//    };
//    struct dataDirectory
//    {
//        public UInt32 VirtualAddress;
//        public UInt32 Size;

//    };
//    // signedData represents a PKCS#7, SignedData strucure.
//    struct signedData {

//        public asn1.ObjectIdentifier Type;

//        struct PKCS7 {

//            public int Version;

//            public asn1.Rawvalue Digests;
//            public asn1.RawValue ContentInfo;
//            //The following tags on struct fields have special meaning to Unmarshal: https://docs.studygolang.com/pkg/encoding/asn1/
//            public [] asn1.RawValue `asn1: "tag:0,optional,set"` Certs;
//            public asn1.RawValue SignerInfos;

//        } `asn1: "explicit,tag:0"`
//            // signedData represents a PKCS#7, SignedData strucure.
//            //type signedData struct {

//            //    Type asn1.ObjectIdentifier

//            //    PKCS7 struct {

//            //        Version int
//            //    Digests     asn1.RawValue
//            //    ContentInfo asn1.RawValue
//            //    Certs[]asn1.RawValue `asn1:"tag:0,optional,set"`
//            //		SignerInfos asn1.RawValue

//            //    } `asn1:"explicit,tag:0"`
//            //}
//}

//    class certificate_tag
//    {

//        public const string bytes = "bytes";
//        public const string crypto_rand = "crypto/rand";
//        public const string crypto_rsa = "crypto/rsa";
//        public const string crypto_x509 = "crypto/x509";
//        public const string crypto_x509_pkix = "crypto/x509/pkix";
//        public const string encoding_asn1 = "encoding/asn1";
//        public const string encoding_binary = "encoding/binary";
//        public const string encoding_hex = "encoding/hex";
//        public const string errors = "errors";
//        public const string flag = "flag";
//        public const string fmt = "fmt";
//        public const string io = "io";
//        public const string io_ioutil = "io/ioutil";
//        public const string math_big = "math/big";
//        public const string os = "os";
//        public const string strings = "strings";
//        public const string time = "time";
//        //
//        public const string rsaKeyBits = "2048";
//        public const string notBeforeTime = "Mon Jan 1 10:00:00 UTC 2013";
//        public const string notAfterTime = "Mon Apr 1 10:00:00 UTC 2013";

//        public const int fileHeaderSize = 20;

//        public const int coffCharacteristicExecutableImage = 2;
//        public const int coffCharacteristicDLL = 0x2000;

//        public const int pe32Magic = 0x10b;
//        public const int pe32PlusMagic = 0x20b;

//        public const int certificateTableIndex = 4;

        
//        public (int offset, int size, int sizeOffset, string err) getAttributeCertificates(byte[] bin)
//        {
//            // offsetOfPEHeaderOffset is the offset into the binary where the
//            // offset of the PE header is found.
//            int offset = 0;
//            int size = 0;
//            int sizeOffset = 0;
//            string err = null;
//            //Error err;
//            int offsetOfPEHeaderOffset = 0x3c;
//            if (bin.Length < offsetOfPEHeaderOffset + 4)
//            {
//                //err = errors.New("binary truncated")
//                return (offset, size, sizeOffset, err);
//            }
//            byte[] temp = ByteArrayHelper.slicesArray(bin, offsetOfPEHeaderOffset, bin.Length);
//            int peOffset = temp[0] | temp[1] << 8 | temp[2] << 16 | temp[3] << 24; //binary.LittleEndian.Uint32

//            if (peOffset < 0 || peOffset + 4 < peOffset)
//            {
//                //err = errors.New("overflow finding PE signature");
//                return (offset, size, sizeOffset, err);

//            }
//            if (bin.Length < peOffset + 4)
//            {
//                //err = errors.New("binary truncated");
//                return (offset, size, sizeOffset, err);
//            }

//            byte[] pe = ByteArrayHelper.slicesArray(bin, peOffset, bin.Length);

//            //lib
//            byte[] PEtemp =  { (byte )'P', (byte)'E', 0, 0 };
//            if(!ByteArrayHelper.EqualByteArr(ByteArrayHelper.slicesArray(pe,0,4), PEtemp)){
//                err = "PE header not found at expected offset";
//                return  (offset, size, sizeOffset, err);
//            }
           
//            //r:= io.Reader(bytes.NewReader(pe[4:])) //cast
//            //Reader r = new Reader(bytes.newReader(slicesArray(pe, 4, pe.Length));
//            Reader r = new Reader(ByteArrayHelper.slicesArray(pe, 4, pe.Length));
//            //ByteOrder kind: littleEndian 
//            ByteOrder order = new ByteOrder();
//            fileHeader fileHeader = new fileHeader() ;
//            //TODO: need to check Reader + LimitedReader
//            err = binary.Read(r, order, fileHeader);
//            if (err != null)
//            {
//                return (offset, size, sizeOffset, err);
//            }

//            if ((fileHeader.Characteristics & coffCharacteristicExecutableImage) == 0)
//            {
//                //err = errors.New("file is not an executable image");

//                return (offset, size, sizeOffset, err);
//            }

//            if ((fileHeader.Characteristics & coffCharacteristicDLL) != 0) {
//                //err = errors.New("file is a DLL");

//                return (offset, size, sizeOffset, err);

//            }
//            LimitedReader lr = new LimitedReader(r, (Int64)fileHeader.SizeOfOptionalHeader);
//            //r = io.LimitReader(r, int64(fileHeader.SizeOfOptionalHeader))
//            //Limit
//            optionalHeader optionalHeader = new optionalHeader();
//            err = binary.ReadLimit(lr, order, optionalHeader);
//            if (err != null)
//            {
//                return (offset, size, sizeOffset, err);
//            }

//            // addressSize is the size of various fields in the Windows-specific
//            // header to follow.
//            int addressSize;


//            switch (optionalHeader.Magic) {
//                case pe32PlusMagic: //0x20b
//                    addressSize = 8;

//                case pe32Magic: //0x10b
//                    addressSize = 4;

//                    // PE32 contains an additional field in the optional header.
//                    UInt32 baseOfData;
//                    err = binary.ReadLimit(lr, order, baseOfData);
//                    if (err != null)
//                    {
//                        return (offset, size, sizeOffset, err);
//                    }
//                //if (err = binary.Read(r, binary.LittleEndian, &baseOfData); err != nil) {
//                //    return (offset, size, sizeOffset);

//                //}
//                default:
//                    //err = fmt.Errorf("unknown magic in optional header: %x", optionalHeader.Magic);
//                    err = "unknown magic in optional header: ";
//                    return (offset, size, sizeOffset, err);

//            }

//            // Skip the Windows-specific header section up to the number of data
//            // directory entries.
//            int toSkip = addressSize + 40 + addressSize * 4 + 4;

//            //skipBuf:= make([]byte, toSkip)
//            byte[] skipBuf = new byte[toSkip];
//            err = LimitedReader.Read(skipBuf).err;
//            if (err != null)
//            {
//                return (offset, size, sizeOffset, err);
//            }
//            //if (_, err = r.Read(skipBuf); err != nil) {
//            //    return (offset, size, sizeOffset);

//            //}

//            // Read the number of directory entries, which is also the last value
//            // in the Windows-specific header.
//            UInt32 numDirectoryEntries;

//            err = binary.ReadLimit(lr, order, numDirectoryEntries);
//            if (err != null)
//            {
//                return (offset, size, sizeOffset, err);
//            }
//            //if (err = binary.Read(r, binary.LittleEndian, &numDirectoryEntries); err != nil) {
//            //    return (offset, size, sizeOffset);

//            //}

//            if (numDirectoryEntries > 4096)
//            {
//                //err = fmt.Errorf("invalid number of directory entries: %d", numDirectoryEntries);
//                err = "invalid number of directory entries";
//                return (offset, size, sizeOffset, err);

//            }
//            dataDirectory[] dataDirectory = new dataDirectory[numDirectoryEntries];
//            //dataDirectory:= make([]dataDirectory, numDirectoryEntries);
//            err = binary.ReadLimit(lr, order, dataDirectory);
//            if (err != null)
//            {
//                return (offset, size, sizeOffset, err);
//            }
//            //if (err = binary.Read(r, binary.LittleEndian, dataDirectory); err != nil) {
//            //    return (offset, size, sizeOffset);

//            //}

//            if (numDirectoryEntries <= certificateTableIndex)
//            {
//                //err = errors.New("file does not have enough data directory entries for a certificate");
//                err = "file does not have enough data directory entries for certificate";
//                return (offset, size, sizeOffset, err);

//            }
//            //check
//            dataDirectory certEntry = dataDirectory[certificateTableIndex];
//            //certEntry = dataDirectory[certificateTableIndex];


//            if (certEntry.VirtualAddress == 0)
//            {
//                //err = errors.New("file does not have certificate data");
//                err = "file does not have ceritficate data";
//                return (offset, size, sizeOffset, err);

//            }

//            UInt32 certEntryEnd = certEntry.VirtualAddress + certEntry.Size;

//            if (certEntryEnd < certEntry.VirtualAddress)
//            {
//                //err = errors.New("overflow while calculating end of certificate entry");
//                err = "overflow whil calculating end of ceritficate entry";
//                return (offset, size, sizeOffset, err);

//            }

//            if ((int)certEntryEnd != bin.Length)
//            {
//                //err = fmt.Errorf("certificate entry is not at end of file: %d vs %d", (int)certEntryEnd, bin.Length);
//                err = "certificate entry is not at end of file";
//                return (offset, size, sizeOffset, err);

//            }

//            byte[] dummyByte = new byte[1];
//            string readErr = LimitedReader.Read(dummyByte).err;
//            if (readErr != null || readErr != "EOF")
//            {
//                err = "optional header contains extra data after data directory";
//                return (offset, size, sizeOffset, err);
//            }
//            //if (_, readErr:= r.Read(dummyByte[:]); readErr == nil || readErr != io.EOF) {
//            //    err = errors.New("optional header contains extra data after data directory");
//            //    return (offset, size, sizeOffset);
//            //}

//            offset = (int)(certEntry.VirtualAddress);

//            size = (int)(certEntry.Size);

//            sizeOffset = (int)(peOffset) + 4 + fileHeaderSize + (int)(fileHeader.SizeOfOptionalHeader) - 8 * ((int)(numDirectoryEntries) - certificateTableIndex) + 4;

//            //lib
//            byte[] sliceBin = ByteArrayHelper.slicesArray(bin, sizeOffset, bin.Length);
//            UInt32 tempCESize = (UInt32)sliceBin[0] | (UInt32)sliceBin[1] << 8 | (UInt32)sliceBin[2] << 16 | (UInt32)sliceBin[3] << 24;
//            //if (binary.LittleEndian.UInt32(bin[sizeOffset:]) != certEntry.Size)
//            if (tempCESize != certEntry.Size)
//            {
//                //err = errors.New("internal error when calculating certificate data size offset");
//                err = "internal error when calculating certificate data size offset";
//                return (offset, size, sizeOffset, err);

//            }

//            return (offset, size, sizeOffset, err);
//        }

//        public (int asn1Length, string err) lengthAsn1(byte[] asn1)
//        {
//            // Read the ASN.1 length of the object.
//            int asn1Length = 0;
//            string err = "";
//            if ((asn1[1] & 0x80) == 0)
//            {
//                // Short form length.
//                asn1Length = (asn1[1]) + 2;
//            }
//            else
//            {
//                int numBytes = (asn1[1] & 0x7f);
//                if (numBytes == 0 || numBytes > 2)
//                {
//                    //err = fmt.Errorf("bad number of bytes in ASN.1 length: %d", numBytes);
//                    return (asn1Length, err);
//                }
//                if (asn1.Length < numBytes + 2)
//                {
//                    //err = errors.New("ASN.1 structure truncated");

//                    return (asn1Length, err);

//                }
//                asn1Length = (asn1[2]);

//                if (numBytes == 2)
//                {
//                    asn1Length <<= 8;

//                    asn1Length |= (asn1[3]);

//                }
//                asn1Length += 2 + numBytes;
//            }
//            return (asn1Length, err);
//        }

//        public (signedData signedData, string err) parseSignedData(byte[] asn1Data)
//        {
//            //(*signedData, error)
//            signedData signedData;
//            string err = "";
//            //if (_, err:= asn1.Unmarshal(asn1Data, &signedData); err != nil) {
//            //    return nil, errors.New("authenticodetag: error while parsing SignedData structure: " + err.Error());

//            //}

//            //der, err:= asn1.Marshal(signedData);

//            //if (err != nil)
//            //{
//            //    return nil, errors.New("authenticodetag: error while marshaling SignedData structure: " + err.Error());

//            //}

//            //if (!bytes.Equal(der, asn1Data))
//            //{
//            //    return nil, errors.New("authenticodetag: ASN.1 parse/unparse test failed");

//            //}
//            return (signedData, err);
           
//        }

//        //        public (X509Certificate cert, int index, string err) getSuperfluousCert(signedData signedData)  
//        //        {
//        //            int n = (signedData.PK)
//        //             n:= len(signedData.PKCS7.Certs);
//        //            int n = (signedData.PKCS7.Certs).Length;
//        //             if (n == 0) {
//        //                return null, -1, null;

//        //            }

//        //            for (index, certASN1 := range signedData.PKCS7.Certs)
//        //            {
//        //                if (cert, err = x509.ParseCertificate(certASN1.FullBytes); err != nil) {
//        //                    return nil, -1, err;

//        //                }

//        //                for (_, ext := range cert.Extensions)
//        //                {
//        //                    if (!ext.Critical && ext.Id.Equal(oidChromeTag))
//        //                    {
//        //                        return cert, index, nil;

//        //                    }
//        //                }
//        //            }

//        //                return nil, -1, nil;
//        //            }

//        //	    }

//        ////DAY2---------------------------------------------------------------------------------------------------------------------------------

//        //// SetSuperfluousCertTag modifies signedData, adding the superfluous cert with the given tag.
//        //// It returns the asn1 serialization of the modified signedData.
//        //public byte[] SetSuperfluousCertTag(signedData signedData, byte[] tag)  {
//        //    //([]byte, error)
//        //    cert = getSuperfluousCert(signedData);
//        //    index = getSuperfluousCert(signedData);
//        //    err = getSuperfluousCert(signedData);

//        //    if (err != nil) {
//        //        //return nil, fmt.Errorf("couldn't identity if any existing certificates are superfluous because of parse error: %w", err);

//        //    }

//        //    if (cert != nil) {
//        //        pkcs7 = &signedData.PKCS7;

//        //        certs = pkcs7.Certs;


//        //        var newCerts[]asn1.RawValue
//        //       newCerts = append(newCerts, certs[:index]...)
//        //		newCerts = append(newCerts, certs[index + 1:]...)

//        //        pkcs7.Certs = newCerts

//        //    }

//        //    notBefore = parseUnixTimeOrDie(notBeforeTime);

//        //notAfter = parseUnixTimeOrDie(notAfterTime);


//        //    priv, err:= rsa.GenerateKey(rand.Reader, rsaKeyBits)

//        //    if (err != nil) {
//        //        return nil, err;

//        //    }

//        //issuerTemplate:= x509.Certificate{
//        //    SerialNumber: new(big.Int).SetInt64(1),
//        //		Subject: pkix.Name{
//        //        CommonName: "Unknown issuer",
//        //		},
//        //		NotBefore: notBefore,
//        //		NotAfter: notAfter,
//        //		KeyUsage: x509.KeyUsageCertSign,
//        //		ExtKeyUsage:[]x509.ExtKeyUsage{ x509.ExtKeyUsageAny},
//        //		SignatureAlgorithm: x509.SHA1WithRSA,
//        //		BasicConstraintsValid: true,
//        //		IsCA: true,
//        //	}

//        //template:= x509.Certificate{
//        //    SerialNumber: new(big.Int).SetInt64(1),
//        //		Subject: pkix.Name{
//        //        CommonName: "Dummy certificate",
//        //		},
//        //		Issuer: pkix.Name{
//        //        CommonName: "Unknown issuer",
//        //		},
//        //		NotBefore: notBefore,
//        //		NotAfter: notAfter,
//        //		KeyUsage: x509.KeyUsageCertSign,
//        //		ExtKeyUsage:[]x509.ExtKeyUsage{ x509.ExtKeyUsageAny},
//        //		SignatureAlgorithm: x509.SHA1WithRSA,
//        //		BasicConstraintsValid: true,
//        //		IsCA: false,
//        //		ExtraExtensions:[]pkix.Extension{
//        //            {
//        //            // This includes the tag in an extension in the
//        //            // certificate.
//        //            Id: oidChromeTag,
//        //				Value: tag,
//        //			},
//        //		},
//        //	}

//        //    derBytes, err:= x509.CreateCertificate(rand.Reader, &template, &issuerTemplate, &priv.PublicKey, priv)

//        //    if (err != nil) {
//        //        return nil, err;

//        //    }

//        //    signedData.PKCS7.Certs = append(signedData.PKCS7.Certs, asn1.RawValue{
//        //    FullBytes: derBytes,
//        //	})

//        //	asn1Bytes, err:= asn1.Marshal(*signedData);

//        //    if (err != nil) {
//        //        return nil, err;

//        //    }
//        //    return asn1Bytes, nil;
//        //}

//        // Certificate constants. See
//        // http://msdn.microsoft.com/en-us/library/ms920091.aspx.
//        // Despite MSDN claiming that 0x100 is the only, current revision - in
//        // practice it's 0x200.

//        const UInt32 attributeCertificateRevision = 0x200;
//        const int attributeCertificateTypePKCS7SignedData = 2;


//        // processAttributeCertificates parses an attribute certificates section of a
//        // PE file and returns the ASN.1 data and trailing data of the sole attribute
//        // certificate included.
//        public (byte[] asn1, byte[] appendedTag, string err) ProcessAttributeCertificates(byte[] certs)
//        {
//            //return (asn1, appendedTag[]byte, err error)
//            byte[] asn1;
//            byte[] appendedTag;
//            string err = "";
//            if (certs.Length < 8)
//            {
//                //err = errors.New("attribute certificate truncated");

//                return (null, null, err);
//            }

//            // This reads a WIN_CERTIFICATE structure from
//            // http://msdn.microsoft.com/en-us/library/ms920091.aspx.
//            //    certLen = binary.LittleEndian.Uint32(certs[:4]);
//            int certLen = certs[0] | certs[1] << 8 | certs[2] << 16 | certs[3] << 24;

//            //revision = binary.LittleEndian.Uint16(certs[4:6]);
//            int revision = certs[4] | certs[5] << 8;
//            //certType = binary.LittleEndian.Uint16(certs[6:8]);
//            int certType = certs[6] | certs[7] << 8;

//            if ((certLen) != certs.Length)
//            {
//                //err = errors.New("multiple attribute certificates found");

//                return (null, null, err);

//            }

//            if (revision != attributeCertificateRevision)
//            {
//                //err = fmt.Errorf("unknown attribute certificate revision: %x", revision);

//                return (null, null, err);

//            }

//            if (certType != attributeCertificateTypePKCS7SignedData)
//            {
//                //err = fmt.Errorf("unknown attribute certificate type: %d", certType);

//                return (null, null, err);

//            }

//            asn1 = ByteArrayHelper.slicesArray(certs, 8, certs.Length);
            
//            if (asn1.Length < 2)
//            {
//                //err = errors.New("ASN.1 structure truncated")

//                return (null, null, err);
//            }

//            int asn1Length = lengthAsn1(asn1).asn1Length;
//            err = lengthAsn1(asn1).err;

//            if (err != null)
//            {
//                return (null, null, err);
//            }

//            appendedTag = ByteArrayHelper.slicesArray(asn1, asn1Length, asn1.Length);
//            asn1 = ByteArrayHelper.slicesArray(asn1, 0, asn1Length);
           
//            return (asn1, appendedTag, err);
//        }

//        // NewBinary returns a Binary that contains details of the PE32 or MSI binary given in |contents|.
//        // |contents| is modified if it is an MSI file.

//        public (PE32Binary, string) NewBinary(byte[] contents)
//        {
//            PE32Binary pe = NewPE32Binary(contents).Item1;
//            string err = NewPE32Binary(contents).Item2;
//            if(err == null)
//            {
//                return (pe, err);
//            }
//            return (null, "Could not parse input as  PE32 ");
//        }

//        // NewPE32Binary returns a Binary that contains details of the PE32 binary given in contents.
//        public (PE32Binary, string) NewPE32Binary(byte[] contents)
//        {
//            int offset = getAttributeCertificates(contents).offset;
//            int size = getAttributeCertificates(contents).size;
//            int certSizeOffset = getAttributeCertificates(contents).sizeOffset;
//            string err = getAttributeCertificates(contents).err;
//            if (err != null)
//            {
//                return (null, err);
//            }
//            byte[] attributeCertificates = ByteArrayHelper.slicesArray(contents, offset, (offset + size));
//            //public (byte[] asn1, byte[] appendedTag, string err) ProcessAttributeCertificates(byte[] certs)
//            byte[] asn1Data = ProcessAttributeCertificates(attributeCertificates).asn1;
//            byte[] appendedTag = ProcessAttributeCertificates(attributeCertificates).appendedTag;
//            err = ProcessAttributeCertificates(attributeCertificates).err;
//            if (err != null)
//            {
//                return (null, err);
//            }

//            signedData signedData = parseSignedData(asn1Data).signedData;
//            PE32Binary result = new PE32Binary() { 
//                contents = contents,
//                attrCertOffset = offset,
//                certSizeOffset = certSizeOffset,
//                asn1Bytes = asn1Data,
//                appendedTag = appendedTag,
//                signedData = signedData
//            };
//            return (result, null);
//        }
        
//    } 
//}
 