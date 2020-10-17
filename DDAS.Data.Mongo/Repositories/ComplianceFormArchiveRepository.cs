using System;
using System.Threading.Tasks;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Repository.Domain.SiteData;
using MongoDB.Driver;

using MongoDB.Driver.Linq;
using System.Collections.Generic;
using System.Linq;
using DDAS.Models.Enums;
using MongoDB.Bson;


namespace DDAS.Data.Mongo.Repositories
{
    internal class ComplianceFormArchiveRepository : Repository<ComplianceFormArchive>,
        IComplianceFormArchiveRepository
    {
        private IMongoDatabase _db;
        public ComplianceFormArchiveRepository(IMongoDatabase db) : base(db)
        {
            _db = db;
        }

        
        public bool DropComplianceForm(object ComplianceFormId)
        {
            var filter = Builders<ComplianceFormArchive>.Filter.Eq("_id", ComplianceFormId);
            var collection = _db.GetCollection<ComplianceFormArchive>(typeof(ComplianceForm).Name);
            var entity = collection.DeleteOne(filter);
            return true;
        }

        
        

        

        
    }
}
