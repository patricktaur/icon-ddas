
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace WebScraping
{
    class Program
    {
        static void Main(string[] args)
        {
            var FilePath = @"C:\Development\p926-ddas\DDAS.API\DataFiles\Templates\Output_File.xlsx";

            SLDocument doc = new SLDocument(FilePath);

            doc.SetCellValue("A2", "Testing");
            doc.Save();
            //var EmbeddingObjectPath = "C:\\Development\\test.pdf";
            //var WordDocumentPath = "C:\\Development\\p926-ddas\\DDAS.API\\App_Data\\Templates\\ComplianceFormTemplate.docx";
            //Console.WriteLine("Calling DLL to embed file");
            //Start.EmbedObjectIntoDocument(EmbeddingObjectPath, WordDocumentPath);
            //Console.WriteLine("file is embedded successfully");
            
            ////string Name = "FullName: Pradeep Chavhan~ FirstName: Pradeep~LastName: Chavhan~Company: Clarity~WorkLocation: Bangalore";
            ////string res = Regex.Replace(Name, "[A-Z]", " $0").Trim();
            ////Console.WriteLine(res);
            ////Console.ReadLine();
        }
    }
}
