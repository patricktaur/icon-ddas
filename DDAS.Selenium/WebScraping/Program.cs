using OpenXmlEmbedObjectNew;
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
            var EmbeddingObjectPath = "C:\\Development\\test.pdf";
            var WordDocumentPath = "C:\\Development\\p926-ddas\\DDAS.API\\App_Data\\Templates\\ComplianceFormTemplate.docx";
            Console.WriteLine("Calling DLL to embed file");
            Start.EmbedObjectIntoDocument(EmbeddingObjectPath, WordDocumentPath);
            Console.WriteLine("file is embedded successfully");
            //string Name = "FullName: Pradeep Chavhan~ FirstName: Pradeep~LastName: Chavhan~Company: Clarity~WorkLocation: Bangalore";

            //string res = Regex.Replace(Name, "[A-Z]", " $0").Trim();

            //Console.WriteLine(res);

            //IUnitOfWork uow = new UnitOfWork("DefaultConnection");
            //ILog log = new LogText(
            //    @"C:\Development\p926-ddas\DDAS.API\Logs\DataExtraction.log", true);

            //log.LogStart();

            //ISearchEngine searchEngine = new SearchEngine(uow);

            ////var test = new SearchService(uow, searchEngine);

            //MongoMaps.Initialize();

            //var service = new ComplianceFormService(uow);

            //var searchSummary = new SearchService(uow, searchEngine);

            //var forms = searchSummary.ReadUploadedFileData(
            //    @"C:\Development\p926-ddas\DDAS.API\App_Data\DDAS_Upload.xlsx", log);

            //var complianceForms = new List<ComplianceForm>();

            //foreach (ComplianceForm form in forms)
            //{
            //    complianceForms.Add(searchSummary.ScanUpdateComplianceForm(
            //        form, log));
            //}

            ////SLDocument doc = new SLDocument(@"C:\Development\p926-ddas\DDAS.API\App_Data\Test-DDAS.xlsx");

            ////doc.GetCellValueAsString("A1");
            //log.LogEnd();

            //Console.ReadLine();
        }
    }
}
