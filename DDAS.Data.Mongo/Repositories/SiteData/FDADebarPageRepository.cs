using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models.Repository.Domain.SiteData;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DDAS.Data.Mongo.Repositories.SiteData
{
    internal class FDADebarPageRepository : Repository<FDADebarPageSiteData>,
        IFDADebarPageRepository
    {
        private IMongoDatabase _db;
        internal FDADebarPageRepository(IMongoDatabase db)
            : base(db)
        { _db = db; }

    }
}
