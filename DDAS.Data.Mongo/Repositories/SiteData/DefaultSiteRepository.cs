using System;
using System.Collections.Generic;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Repository.Domain.SiteData;
using MongoDB.Driver;

namespace DDAS.Data.Mongo.Repositories.SiteData
{

    internal class DefaultSiteRepository : Repository<DefaultSite>,
        IDefaultSiteRepository
    {
        private IMongoDatabase _db;
        internal DefaultSiteRepository(IMongoDatabase db) : base(db)
        {
            _db = db;
        }

        public bool UpdateDefaultSite(DefaultSite defaultSite)
        {
            _db.GetCollection<DefaultSite>(typeof(DefaultSite).Name).
                ReplaceOne(x => x.RecId == defaultSite.RecId, defaultSite);
            return true;
        }
    }
}
