﻿using DDAS.Models.Entities.Domain;
using DDAS.Models.Entities.Domain.SiteData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Repository.Domain.SiteData
{
    public interface IFDADebarPageRepository : IRepository<FDADebarPageSiteData>
    {
        FDADebarPageSiteData GetLatestDocument();
    }
}
