using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDAS.Models.Entities.Domain.SiteData;
using MongoDB.Driver;
using DDAS.Models.Repository.Domain.SiteData;

namespace DDAS.Data.Mongo.Repositories.SiteData
{
    internal class CBERClinicalInvestigatorRepository : 
        Repository<CBERClinicalInvestigatorInspectionSiteData>,
        ICBERClinicalInvestigatorInspectionRepository
    {
        private IMongoDatabase _db;
        public CBERClinicalInvestigatorRepository(IMongoDatabase db) : base(db)
        {
            _db = db;
        }

        public CBERClinicalInvestigatorInspectionSiteData GetLatestDocument()
        {
            var collection = _db.GetCollection<CBERClinicalInvestigatorInspectionSiteData>(typeof(CBERClinicalInvestigatorInspectionSiteData).Name);
            var entity = collection.Find(x => x.DataExtractionSucceeded == true).SortByDescending(y => y.CreatedOn).FirstOrDefault();
            return entity;
        }

        public CBERClinicalInvestigatorInspectionSiteData GetLatestDocumentCached()
        {
            return null;
        }

    }
}
