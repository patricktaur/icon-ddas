using OpenXmlEmbedObjectNew;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WebScraping.Tests;

namespace WebScraping
{
    class Program
    {
        static void Main(string[] args)
        {
            //var FilePath = @"C:\Development\p926-ddas\DDAS.API\DataFiles\Templates\Output_File.xlsx";

            //var EmbeddingObjectPath = "C:\\Development\\test.pdf";
            //var WordDocumentPath = "C:\\Development\\p926-ddas\\DDAS.API\\App_Data\\Templates\\ComplianceFormTemplate.docx";
            //Console.WriteLine("Calling DLL to embed file");
            //var start = new Start();
            //start.EmbedObjectIntoDocument(EmbeddingObjectPath, WordDocumentPath);
            //Console.WriteLine("file is embedded successfully");

            ////string Name = "FullName: Pradeep Chavhan~ FirstName: Pradeep~LastName: Chavhan~Company: Clarity~WorkLocation: Bangalore";
            ////string res = Regex.Replace(Name, "[A-Z]", " $0").Trim();
            ////Console.WriteLine(res);
            ////Console.ReadLine();

            //Test cases
            //1. Login
            var Tests = new TestSites();
            Tests.TestLogin();
        }
    }
}
