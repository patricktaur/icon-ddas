using DDAS.Data.Mongo;
using DDAS.Data.Mongo.Maps;
using DDAS.Models;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using DDAS.Models.Interfaces;
using DDAS.Services.Search;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.Linq;
using Utilities;
using WebScraping.Selenium.SearchEngine;

namespace WebScraping
{
    class Program
    {
        static void Main(string[] args)
        {
            IUnitOfWork uow = new UnitOfWork("DefaultConnection");
            ILog log = new LogText(
                @"C:\Development\p926-ddas\DDAS.API\Logs\DataExtraction.log", true);

            log.LogStart();

            ISearchEngine searchEngine = new SearchEngine(uow);

            //var test = new SearchService(uow, searchEngine);

            MongoMaps.Initialize();

            var service = new ComplianceFormService(uow);

            var searchSummary = new SearchService(uow, searchEngine);

            var forms = searchSummary.ReadUploadedFileData(
                @"C:\Development\p926-ddas\DDAS.API\App_Data\DDAS_Upload.xlsx", log);

            var complianceForms = new List<ComplianceForm>();

            foreach (ComplianceForm form in forms)
            {
                complianceForms.Add(searchSummary.ScanUpdateComplianceForm(
                    form, log));
            }

            //SLDocument doc = new SLDocument(@"C:\Development\p926-ddas\DDAS.API\App_Data\Test-DDAS.xlsx");

            //doc.GetCellValueAsString("A1");
            log.LogEnd();
            
            Console.ReadLine();
        }
    }
}
