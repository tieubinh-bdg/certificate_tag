using System;
using System.IO;
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

        static string GetAppendedTag(byte[] byteContents)
        {
            // todo: Tien Dang
            return "";
        }
    }
}
