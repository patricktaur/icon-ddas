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
    internal class ClinicalInvestigatorInspectionListRepository :
        Repository<ClinicalInvestigatorInspectionSiteData>, 
        IClinicalInvestigatorInspectionListRepository
    {
        private IMongoDatabase _db;
        public ClinicalInvestigatorInspectionListRepository(IMongoDatabase db) :
            base(db)
        {
            _db = db;
        }

        public ClinicalInvestigatorInspectionSiteData GetLatestDocument()
        {
            var collection = _db.GetCollection<ClinicalInvestigatorInspectionSiteData>(typeof(ClinicalInvestigatorInspectionSiteData).Name);
            var entity = collection.Find(x => x.DataExtractionSucceeded == true).SortByDescending(y => y.CreatedOn).FirstOrDefault();
            return entity;
        }
    }
}
