using DDAS.Models.Entities.Domain;
using DDAS.Models.Repository.Domain.SiteData;
using MongoDB.Driver;

namespace DDAS.Data.Mongo.Repositories.SiteData
{
    internal class SiteSourceRepository : Repository<SitesToSearch>,
        ISiteSourceRepository
    {
        private IMongoDatabase _db;
        internal SiteSourceRepository(IMongoDatabase db) : base(db)
        {
            _db = db;
        }

        public bool UpdateSiteSource(SitesToSearch SiteSource)
        {
            _db.GetCollection<SitesToSearch>(typeof(SitesToSearch).Name).
                ReplaceOne(x => x.RecId == SiteSource.RecId, SiteSource);
            return true;
        }
    }
}
