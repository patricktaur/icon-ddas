using DDAS.Models.Entities.Domain;
using DDAS.Models.Repository.Domain.SiteData;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Data.Mongo.Repositories.SiteData
{
    internal class AssignmentHistoryRepository : Repository<AssignmentHistory>, IAssignmentHistoryRepository
    {
        private IMongoDatabase _db;
        public AssignmentHistoryRepository(IMongoDatabase db): base(db)
        {
            _db = db;
        }

        public AssignmentHistory GetAssignmentHistoryByComplianceFormId(Guid Id)
        {
            var filter = Builders<AssignmentHistory>.Filter.Eq("ComplianceFormId", Id);
            var collection = _db.GetCollection<AssignmentHistory>(
                typeof(AssignmentHistory).Name);
            var entity = collection.Find(filter).FirstOrDefault();
            return entity;
        }

        public bool UpdateRemovedOn(Guid Id)
        {
            var filter = Builders<AssignmentHistory>.Filter.Eq("ComplianceFormId", Id);

            var collection = _db.GetCollection<AssignmentHistory>(
                typeof(AssignmentHistory).Name);

            var update = Builders<AssignmentHistory>.Update.CurrentDate("RemovedOn");

            var result = collection.UpdateOne(filter, update);
            if (result.IsAcknowledged && result.ModifiedCount == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
