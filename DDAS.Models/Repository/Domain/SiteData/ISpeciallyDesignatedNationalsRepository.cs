﻿using DDAS.Models.Entities.Domain;
using DDAS.Models.Entities.Domain.SiteData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Repository.Domain.SiteData
{
    public interface ISpeciallyDesignatedNationalsRepository : 
        IRepository<SpeciallyDesignatedNationalsListSiteData>
    {
       SpeciallyDesignatedNationalsListSiteData GetLatestDocument();
    }
}
