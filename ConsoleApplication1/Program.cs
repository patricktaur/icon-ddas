
using DDAS.Data.Mongo;
using DDAS.Data.Mongo.Maps;
using DDAS.Models;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Interfaces;
using DDAS.Services.Search;
using Utilities;
using WebScraping.Selenium.SearchEngine;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            GetSearchSummaryTest();


        }
        
        static void GetSearchSummaryTest()
        {
            //references Services
            //

            MongoMaps.Initialize();
            string DataExtractionLogFile = System.Configuration.ConfigurationManager.AppSettings["DataExtractionLogFile"];
           
            ILog log = new LogText(DataExtractionLogFile, true);
            IUnitOfWork uow = new UnitOfWork("DefaultConnection");

            ISearchEngine engine = new SearchEngine(log, uow );
            var sut = new SearchService(uow);


            var query = new NameToSearchQuery();
            query.NameToSearch = "Patrick";
            sut.GetSearchSummary(query);

        }

         /*

        static void getRecord(Int64? id)
        {
            //var id = "444a7e54c8da426c9a093f0cb630e4b4";
            var test = new Repository<FDADebarPageSiteData>();
            var ret = test.FindById(id);
        }

        static void Read()
        {
            var test = new Repository<FDADebarPageSiteData>();
            var obj = test.GetAll();
            foreach(FDADebarPageSiteData item in obj)
            {
                getRecord(item.RecId);
                Console.Write(item.RecId);
                Console.Write(item.CreatedBy);
            }
        }

        static void Add()
        {
            var test = new Repository<FDADebarPageSiteData>();
            var objToSave = new FDADebarPageSiteData
            {
                //RecId = Guid.NewGuid(),
                CreatedBy = "Ram",
                CreatedOn = DateTime.Now,
                SiteLastUpdatedOn = DateTime.Today,
                DebarredPersons = new DebarredPerson[]
                {
                    new DebarredPerson {NameOfPerson="abc", EffectiveDate="eff date - 4", VolumePage="abc" },
                     new DebarredPerson {NameOfPerson="bbc", EffectiveDate="eff date - 5", VolumePage="abc" },
                      new DebarredPerson {NameOfPerson="cbc", EffectiveDate="eff date - 6", VolumePage="abc" }
                }
            };
            test.Add(objToSave);
>>>>>>> ba9cf142902b14d35837af7f7c8b71c7ca6dbacc
        }
        */
    }
}
