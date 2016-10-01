using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using DDAS.Models.Entities.Domain.SiteData;
using MongoDB.Bson.Serialization;
using DDAS.Data.Mongo.Maps;

namespace DDAS.Data.Mongo
{
    public class test
    {
        public ObjectId Id { get; set; }

        public  void TestConnection()
        {

            Type type = typeof(FDADebarPageSiteData);
            Console.WriteLine(type.Name);
            //FDADebarPageSiteDataMap map = new FDADebarPageSiteDataMap();
            TestingMongoDb map = new TestingMongoDb();

            var client = new MongoClient("mongodb://127.0.0.1");
            var DB = client.GetDatabase("DDAS");



            var Collec = DB.GetCollection<TestMongo>("TestMongo");
            var documnt = new TestMongo
            {
                FirstName = "Pradeep",
                LastName = "Chavhan",
                Company = "Clarity",
                Location = "Bangalore"
            };

            Collec.InsertOne(documnt);

            //var collections = DB.GetCollection<FDADebarPageSiteData>("FDADebarPageSiteData");

            //var documents = await collections.Find(_ => true).ToListAsync();
            //var documents =  collections.Find(_ => true).ToList();

        }
    }

    public class TestMongo
    {
        public long RecId { get; set; }
        public string FirstName { get; set; }
        public string Company { get; set; }
        public string LastName { get; set; }
        public string Location { get; set; }
    }
}
