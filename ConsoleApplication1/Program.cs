
using DDAS.Data.Mongo;
using DDAS.Data.Mongo.Maps;
using DDAS.Data.Mongo.Repositories;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Entities.Domain.SiteData;
using System;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
           // var test = new TestOne();
           //test.AddTest();
           //test.ReadTest();


            // getRecord(22);
            //Add();
           // Read();
            //RecId = { 2b8fc573 - 5b04 - 4e04 - a77b - af2d9be4b1c1}
            //+		item.RecId	{444a7e54-c8da-426c-9a09-3f0cb630e4b4}	System.Guid
            //+		item.RecId	{51050049-8a23-44f6-bc40-937bb0d482a7}	System.Guid


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
