using EmbedTenantName.Helpers;
using EmbedTenantName.Models;
using System.IO;

namespace EmbedTenantName
{
    class Program
    {
        static void Main(string[] args)
        {
            GetWorkingFlow();
        }

        private static void GetWorkingFlow()
        {
            string filename = "Resource/OrchestratorAgentService.1.0.20220320.2";
            string infilename = filename + ".exe";
            string outfilename = filename + "_tag.exe";
            string tagContents = "&tenantname=portaluat.pia.ai&username=tien.dang";

            bool removeAppendedTag = false;
            bool setAppendedTag = false;
            bool getAppendedTag = true;

            byte[] byteContent = File.ReadAllBytes(outfilename);
            PE32Binary bin = PE32Binary.Create(byteContent);
            if (getAppendedTag)
            {
                System.Console.WriteLine("Appended tag included: " + bin.GetAppendedTag());
                //System.Console.WriteLine("Contents: " + bin.GetContent());
            }
            if (removeAppendedTag)
            {
                bin.RemoveAppendedTag();
            }

            if (setAppendedTag)
            {
                bin.SetAppendedTag(tagContents);
            }

            if (getAppendedTag)
            {
                //File.WriteAllBytes("binh.exe", bin.Contents);
                //System.Console.WriteLine(ByteHelper.ByteArrayToFile(outfilename, bin.Contents));
                System.Console.WriteLine("Appended tag included: " + bin.GetAppendedTag());
                //System.Console.WriteLine("Contents: " + bin.GetContent());
            }
        }
    }
}
