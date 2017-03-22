using DDAS.Models.Entities.Domain;
using DDAS.Models.Repository.Domain.SiteData;
using MongoDB.Driver;

namespace DDAS.Data.Mongo.Repositories.SiteData
{
    internal class SiteSourceRepository : Repository<SearchQuerySite>,
        ISiteSourceRepository
    {
        private IMongoDatabase _db;
        internal SiteSourceRepository(IMongoDatabase db) : base(db)
        {
            _db = db;
        }

        public bool UpdateSiteSource(SearchQuerySite SiteSource)
        {
            _db.GetCollection<SearchQuerySite>(typeof(SearchQuerySite).Name).
                ReplaceOne(x => x.RecId == SiteSource.RecId, SiteSource);
            return true;
        }
    }
}
