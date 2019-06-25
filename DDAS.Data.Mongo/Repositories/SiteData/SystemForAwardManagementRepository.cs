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
    internal class SystemForAwardManagementRepository : 
        Repository<SystemForAwardManagementPageSiteData>, 
        ISystemForAwardManagementRepository
    {
        private IMongoDatabase _db;
        public SystemForAwardManagementRepository(IMongoDatabase db) : base(db)
        {
            _db = db;
        }

        public SystemForAwardManagementPageSiteData GetLatestDocument()
        {
            var collection = _db.GetCollection<SystemForAwardManagementPageSiteData>(typeof(SystemForAwardManagementPageSiteData).Name);
            var entity = collection.Find(x => true).SortByDescending(y => y.CreatedOn).FirstOrDefault();
            return entity;
        }
    }
}
