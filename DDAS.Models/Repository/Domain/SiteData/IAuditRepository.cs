﻿using DDAS.Models.Entities.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Repository.Domain.SiteData
{
    public interface IAuditRepository : IRepository<Audit>
    {
        Task UpdateAudit(Audit audit);
    }
}