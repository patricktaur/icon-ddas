

using DDAS.Models.Entities.Domain.SiteData;
using Norm.Configuration;

namespace DDAS.Data.Mongo.Maps
{
    class ERRProposalToDebarPageSiteDataMap :MongoConfigurationMap
    {
        public ERRProposalToDebarPageSiteDataMap()
        {
            For<ERRProposalToDebarPageSiteData>(config =>
            {
                config.IdIs(u => u.RecId);

            });
        }
    }
}
