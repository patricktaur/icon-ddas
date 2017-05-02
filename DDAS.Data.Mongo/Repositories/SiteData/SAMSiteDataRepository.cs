using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models.Repository.Domain.SiteData;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Data.Mongo.Repositories.SiteData
{
    internal class SAMSiteDataRepository : Repository<SystemForAwardManagement>,
        ISAMSiteDataRepository
    {
        private IMongoDatabase _db;
        public SAMSiteDataRepository(IMongoDatabase db) : base(db)
        {
            _db = db;
        }

        public bool DropRecord(object RecId)
        {
            var filter = Builders<SystemForAwardManagement>.Filter.Eq("_id", RecId);
            var collection = _db.GetCollection<SystemForAwardManagement>(typeof(SystemForAwardManagement).Name);
            var entity = collection.DeleteOne(filter);
            return true;
        }

        public bool DropAll()
        {
            
            var collection = _db.GetCollection<SystemForAwardManagement>(typeof(SystemForAwardManagement).Name);
            var entity = collection.DeleteMany("{ }");
            return true;
        }
    }
}
