using DDAS.Data.Mongo;
using DDAS.Data.Mongo.Maps;
using DDAS.Models;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using DDAS.Models.Interfaces;
using DDAS.Services.Search;
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
            //test t = new test();
            //t.TestConnection();
            Program p = new Program();

            p.TestClinicalInvestigator();

            IUnitOfWork uow = new UnitOfWork("DefaultConnection");
            ILog log = new LogText("", true);
            ISearchEngine searchEngine = new SearchEngine(log, uow);

            var test = new Search(uow, searchEngine);

            MongoMaps.Initialize();

            var FDASiteData = 
                uow.FDADebarPageRepository.GetAll().
                OrderByDescending(t => t.CreatedOn).FirstOrDefault();

            var result = test.GetFDADebarPageMatch("Cullinane Andrew R", FDASiteData.RecId);
            Console.WriteLine("Records matching single word:");
            Console.WriteLine(result.DebarredPersons.Where(x => x.Matched == 1).Count());
            Console.WriteLine("Records matching two word:");
            Console.WriteLine(result.DebarredPersons.Where(x => x.Matched == 2).Count());
            Console.WriteLine("Records matching three words:");
            Console.WriteLine(result.DebarredPersons.Where(x => x.Matched == 3).Count());
            Console.WriteLine("All Records:");
            Console.WriteLine(result.DebarredPersons.ToList());
            Console.ReadLine();
        }

        public void TestClinicalInvestigator()
        {

        }
    }
}
