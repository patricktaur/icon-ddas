﻿using DDAS.Models.Entities.Domain.SiteData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Repository.Domain.MatchedSiteData
{
    public interface IFDADebarMatchedRecordsRepository : 
        IRepository<FDADebarPageMatchRecords>
    {
    }
}
