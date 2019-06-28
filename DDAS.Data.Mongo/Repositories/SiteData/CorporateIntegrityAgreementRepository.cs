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
        private IMongoDatabase _db;
        public CorporateIntegrityAgreementRepository(IMongoDatabase db): base(db)
        {
            _db = db;
        }

        public CorporateIntegrityAgreementListSiteData GetLatestDocument()
        {
            var collection = _db.GetCollection<CorporateIntegrityAgreementListSiteData>(typeof(CorporateIntegrityAgreementListSiteData).Name);
            var entity = collection.Find(x => true).SortByDescending(y => y.CreatedOn).FirstOrDefault();
            return entity;
        }
    }
}
