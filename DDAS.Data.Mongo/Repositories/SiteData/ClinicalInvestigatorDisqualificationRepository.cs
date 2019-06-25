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
    internal class ClinicalInvestigatorDisqualificationRepository : 
        Repository<ClinicalInvestigatorDisqualificationSiteData>,
        IClinicalInvestigatorDisqualificationRepository
    {
        private IMongoDatabase _db;
        public ClinicalInvestigatorDisqualificationRepository(IMongoDatabase db)
            : base(db)
        {
            _db = db;
        }

        public ClinicalInvestigatorDisqualificationSiteData GetLatestDocument()
        {
            var collection = _db.GetCollection<ClinicalInvestigatorDisqualificationSiteData>(typeof(ClinicalInvestigatorDisqualificationSiteData).Name);
            var entity = collection.Find(x => true).SortByDescending(y => y.CreatedOn).FirstOrDefault();
            return entity;
        }
    }
}
