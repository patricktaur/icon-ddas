using DDAS.Models.Entities.Domain.SiteData;
using Norm;
using Norm.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Data.Mongo.Maps
{
    class PHSAdministrativeActionListingSiteDataMap : MongoConfigurationMap
    {
        public PHSAdministrativeActionListingSiteDataMap()
        {
            For<PHSAdministrativeActionListingSiteData>(config =>
            {
                config.IdIs(u => u.RecId);

            });
        }
    }
}
