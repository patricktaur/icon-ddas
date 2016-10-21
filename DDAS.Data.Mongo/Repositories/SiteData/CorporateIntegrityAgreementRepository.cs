using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models.Repository.Domain.SiteData;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Data.Mongo.Repositories.SiteData
{
    internal class CorporateIntegrityAgreementRepository :
        Repository<CorporateIntegrityAgreementListSiteData>,
        ICorporateIntegrityAgreementRepository
    {
        public CorporateIntegrityAgreementRepository(IMongoDatabase db): base(db)
        {

        }
    }
}
