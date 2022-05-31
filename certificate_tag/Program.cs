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

        private static void GetWorkingFlow()
        {
            string filename = "Resource/sw";
            string infilename = filename + ".exe";
            string outfilename = filename + "_tag.exe";
            string tagContents = "&tenantname=portaluat.pia.ai&username=tien.dang";

            bool removeAppendedTag = false;
            bool setAppendedTag = false;
            bool getAppendedTag = true;

            byte[] byteContent = File.ReadAllBytes(infilename);
            PE32Binary bin = PE32Binary.Create(byteContent);

            //if (removeAppendedTag)
            //{
            //    bin.RemoveAppendedTag();
            //}

            if (setAppendedTag)
            {
                bin.SetAppendedTag(tagContents);
            }

            if (getAppendedTag)
            {
                System.Console.WriteLine("Appended tag included: " + bin.GetAppendedTag());
            }
        }
    }
}
