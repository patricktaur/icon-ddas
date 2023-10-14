using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models.Repository.Domain.SiteData;
using MongoDB.Driver;
//using Norm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Data.Mongo.Repositories.SiteData
{
    internal class PHSAdministrativeActionListingRepository :
        Repository<PHSAdministrativeActionListingSiteData>, 
        IPHSAdministrativeActionListingRepository
    {
        private IMongoDatabase _db;
        public PHSAdministrativeActionListingRepository(IMongoDatabase db) : base(db)
        {
            _db = db;
        }

        public PHSAdministrativeActionListingSiteData GetLatestDocument()
        {
            var collection = _db.GetCollection<PHSAdministrativeActionListingSiteData>(typeof(PHSAdministrativeActionListingSiteData).Name);
            var entity = collection.Find(x => x.DataExtractionSucceeded == true).SortByDescending(y => y.CreatedOn).FirstOrDefault();
            return entity;
        }
    }
}
