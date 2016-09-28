using DDAS.Models.Entities;
using DDAS.Models.Entities.Domain.SiteData;
using MongoDB.Bson.Serialization;
using Norm.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Data.Mongo.Maps
{
    public class FDADebarPageSiteDataMap: MongoConfigurationMap
    {

        public FDADebarPageSiteDataMap()
        {
            For<FDADebarPageSiteData>(config =>
            {
                config.IdIs(u => u.RecId);
  
            });
        }
    }
}

