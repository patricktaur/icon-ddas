using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models.Repository.Domain.SiteData;
using Norm;

namespace DDAS.Data.Mongo.Repositories.SiteData
{
    internal class FDADebarPageRepository : Repository<FDADebarPageSiteData>, 
        IFDADebarPageRepository
    {
        internal FDADebarPageRepository(IMongoDatabase db)
            : base(db)
        {
            
        }
    }
}
