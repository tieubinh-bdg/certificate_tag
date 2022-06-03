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
            string filename = "Resource/OrchestratorAgentService.2";
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
                System.Console.WriteLine("-----------------Set appended tag process-----------");
                bin.SetAppendedTag(tagContents);
                System.Console.ReadLine();
            }

            if (getAppendedTag)
            {
                System.Console.WriteLine("-----------------Get appended tag process-----------");
                System.Console.WriteLine("Appended tag included: " + bin.GetAppendedTag());
                System.Console.WriteLine("Create OrchestratorAgentService_tag.exe that has appendedTag : " + ByteHelper.ByteArrayToFile(outfilename, bin.Contents));
                
            }
        }
    }
}
