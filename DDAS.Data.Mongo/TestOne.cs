using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDAS.Models.Entities.Domain;

namespace DDAS.Data.Mongo
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

            var coll = _db.GetCollection<Sites>("FDADebarSite");
            //var searchQuerySite = _db.GetCollection<SearchQueryAtSite>("QuerySite");
            var document = new Sites {
                NameToSearch = "John",
                SiteName = "FDA Debar page",
                SiteShortName = "FDA Debar page",
                SiteUrl = "xxxxx",
                SearchTimeTakenInMs = "None",
                HasErrors = false,
                ErrorDescription = "None"};

            coll.InsertOne(document);

        }

        public void ReadTest()
        {
            var collection = _db.GetCollection<Sites>("FDADebarSite");
            //var items = collection.FindAsync(x => x.NameToSearch.StartsWith("X"));
            //items.Wait();


            var obj = collection.Find(x => x.NameToSearch.StartsWith("John")).FirstOrDefault();

            //var collection = _db.GetCollection<SearchQuery>("search");

        }


    }

    public class Sites
    {
        public ObjectId Id { get; set; }
        public string NameToSearch { get; set; }
        public string SiteName { get; set; }
        public string SiteShortName { get; set; }
        public bool Selected { get; set; }
        public string SiteUrl { get; set; }
        public string SearchTimeTakenInMs { get; set; }
        public bool HasErrors { get; set; } = false;
        public string ErrorDescription { get; set; }
    }

}
