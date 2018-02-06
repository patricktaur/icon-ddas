

using DDAS.Models.Entities.Domain;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Configuration;
using System.Linq;



using MongoDB.Driver.Builders;
using System.Collections.Generic;

namespace DDAS.EF.No_SQL_DB
{
    public class TestOne
    {
        private IMongoDatabase _db;

        public TestOne()
        {
            string conn = ConfigurationManager.AppSettings["DBConnection"];
            MongoClient client = new MongoClient(conn);
            _db = client.GetDatabase("DDAS");
           // _db.RunCommandAsync((Command<BsonDocument>)"{ping:1}").Wait();


        }

        public void AddTest()
        {
  
            var coll = _db.GetCollection<SearchQuery>("search");
            var document = new SearchQuery { NameToSearch ="Two" };
            coll.InsertOne(document);
           

        }

        public  void ReadTest()
        {
            var collection = _db.GetCollection<SearchQuery>("search");
            //var items = collection.FindAsync(x => x.NameToSearch.StartsWith("X"));
            //items.Wait();


            var obj =  collection.Find(x => x.NameToSearch.StartsWith("One")).FirstOrDefault();
            
            //var collection = _db.GetCollection<SearchQuery>("search");
            
        }

       
    }

    public class SearchQuery
    {
        public ObjectId Id { get; set; }
        public string NameToSearch { get; set; }
       
    }
}
