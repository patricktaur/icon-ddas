using DDAS.Models.Entities.Domain.SiteData;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Data.Mongo.Indexes
{
    public class Indexes
    {
        private IMongoDatabase _db;
        public Indexes()
        {
            var mongo = new MongoClient("mongodb://127.0.0.1");
            _db = mongo.GetDatabase("DDAS");
        }
       

        public void CreateTestIndex()
        {
            var collection = _db.GetCollection<FDADebarPageSiteData>("FDADebarPageSiteData");
            //collection.Indexes.CreateOne()
        }

        public async Task CreateIndex()
        {

            var collection = _db.GetCollection<FDADebarPageSiteData>("FDADebarPageSiteData");
          
            
            //await collection.Indexes.CreateOneAsync(Builders<FDADebarPageSiteData>.IndexKeys.Ascending("DebarredPersons.NameOfPerson"));

            await collection.Indexes.CreateOneAsync(Builders<FDADebarPageSiteData>.IndexKeys.Text("DebarredPersons.NameOfPerson"));
           

        }
    }
}
