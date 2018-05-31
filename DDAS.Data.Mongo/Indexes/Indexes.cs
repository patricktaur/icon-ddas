using DDAS.Models.Entities.Domain;
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
            //var ConnStr =
            //    System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            //var DBName =
            //    System.Configuration.ConfigurationManager.AppSettings["DBName"];

            //var uow = new UnitOfWork(ConnStr, DBName);

            //var mongo = new MongoClient(ConnectionString);

            //_db = mongo.GetDatabase(DBName);


            //var mongo = new MongoClient("mongodb://127.0.0.1");
            //_db = mongo.GetDatabase("DDAS");
        }
       

        public void CreateTestIndex()
        {
            var collection = _db.GetCollection<FDADebarPageSiteData>("FDADebarPageSiteData");
            //collection.Indexes.CreateOne()
        }

        public async Task CreateIndex()
        {

            var collection = _db.GetCollection<ComplianceForm>("ComplianceForm");
          
            
            //await collection.Indexes.CreateOneAsync(Builders<FDADebarPageSiteData>.IndexKeys.Ascending("DebarredPersons.NameOfPerson"));

            await collection.Indexes.CreateOneAsync(Builders<ComplianceForm>.IndexKeys.Ascending("SearchStartedOn"));
           

        }
    }
}
