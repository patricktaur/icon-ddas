﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models.Repository.Domain.SiteData;
using MongoDB.Driver;

namespace DDAS.Data.Mongo.Repositories.SiteData
{
    internal class SDNSiteDataRepository :
        Repository<SDNList>, ISDNSiteDataRepository
    {
        public SDNSiteDataRepository(IMongoDatabase db) : base(db)
        {
        }
    }
}
