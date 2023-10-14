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
    class FDAWarningLettersRepository : Repository<FDAWarningLettersSiteData>,
        IFDAWarningLettersRepository
    {
        private IMongoDatabase _db;
        public FDAWarningLettersRepository(IMongoDatabase db) : base(db)
        {
            _db = db;
        }

        public FDAWarningLettersSiteData GetLatestDocument()
        {
            var collection = _db.GetCollection<FDAWarningLettersSiteData>(typeof(FDAWarningLettersSiteData).Name);
            var entity = collection.Find(x => x.DataExtractionSucceeded == true).SortByDescending(y => y.CreatedOn).FirstOrDefault();
            return entity;
        }
    }
}
