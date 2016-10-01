using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models.Repository.Domain.SiteData;
using MongoDB.Driver;
//using Norm;



namespace DDAS.Data.Mongo.Repositories.SiteData
{
    internal class AdequateAssuranceListRepository : 
        Repository<AdequateAssuranceListSiteData>, 
        IAdequateAssuranceListRepository
    {
        internal AdequateAssuranceListRepository(IMongoDatabase db)
            : base(db)
        {

        }
    }



}
