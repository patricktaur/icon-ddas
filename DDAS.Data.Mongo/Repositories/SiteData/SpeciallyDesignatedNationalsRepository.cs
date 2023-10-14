using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models.Repository.Domain.SiteData;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDAS.Models.Entities.Domain;

namespace DDAS.Data.Mongo.Repositories.SiteData
{
    internal class SpeciallyDesignatedNationalsRepository : 
        Repository<SpeciallyDesignatedNationalsListSiteData>,
        ISpeciallyDesignatedNationalsRepository
    {
        private IMongoDatabase _db;
        public SpeciallyDesignatedNationalsRepository(IMongoDatabase db) : base(db)
        {
            _db = db;
        }

        public SpeciallyDesignatedNationalsListSiteData GetLatestDocument()
        {
            var collection = _db.GetCollection<SpeciallyDesignatedNationalsListSiteData>(typeof(SpeciallyDesignatedNationalsListSiteData).Name);
            var entity = collection.Find(x => x.DataExtractionSucceeded == true).SortByDescending(y => y.CreatedOn).FirstOrDefault();
            return entity;
        }
    }
}
