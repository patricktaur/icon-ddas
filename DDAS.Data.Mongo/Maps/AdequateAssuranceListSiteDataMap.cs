using DDAS.Models.Entities.Domain.SiteData;
using Norm.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Data.Mongo.Maps
{
    class AdequateAssuranceListSiteDataMap : MongoConfigurationMap
    {
        public AdequateAssuranceListSiteDataMap()
        {
            For<AdequateAssuranceListSiteData>(config =>
            {
                config.IdIs(u => u.RecId);

            });
        }
    }
}
