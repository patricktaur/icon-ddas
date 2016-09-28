using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models.Repository.Domain.SiteData;
using Norm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDAS.Models.Repository;
using System.Threading;

namespace DDAS.Data.Mongo.Repositories.SiteData
{
    internal class ERRProposalToDebarRepository : Repository<ERRProposalToDebarPageSiteData>,
        IERRProposalToDebarRepository
    {
        internal ERRProposalToDebarRepository(IMongoDatabase db) : base(db)
        {

        }
    }
}
