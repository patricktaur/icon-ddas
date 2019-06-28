using System;
using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models.Repository.Domain.SiteData;
using MongoDB.Driver;

namespace DDAS.Data.Mongo.Repositories.SiteData
{
    
    internal class AdequateAssuranceListRepository : 
        Repository<AdequateAssuranceListSiteData>, 
        IAdequateAssuranceListRepository
    {
        private IMongoDatabase _db;
        
        internal AdequateAssuranceListRepository(IMongoDatabase db)
            : base(db)
        {
            _db = db;
            
        }

        public AdequateAssuranceListSiteData GetLatestDocument()
        {
            var collection = _db.GetCollection<AdequateAssuranceListSiteData>(typeof(AdequateAssuranceListSiteData).Name);
            var entity = collection.Find(x => true).SortByDescending(y => y.CreatedOn).FirstOrDefault();
            return entity;
        }

        
    }



}
