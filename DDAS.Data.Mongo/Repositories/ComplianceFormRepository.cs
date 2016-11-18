using System;
using System.Threading.Tasks;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Repository.Domain.SiteData;
using MongoDB.Driver;
using System.Collections.Generic;

namespace DDAS.Data.Mongo.Repositories
{
    internal class ComplianceFormRepository : Repository<ComplianceForm>,
        IComplianceFormRepository
    {
        private IMongoDatabase _db;
        public ComplianceFormRepository(IMongoDatabase db) : base(db)
        {
            _db = db;
        }

        public ComplianceForm FindComplianceFormIdByNameToSearch(string NameToSearch)
        {
            var filter = Builders<ComplianceForm>.Filter.Eq("NameToSearch", NameToSearch);
            var collection = _db.GetCollection<ComplianceForm>(typeof(ComplianceForm).Name);
            var entity = collection.Find(filter).FirstOrDefault();
            return entity;
        }

        public List<ComplianceForm> FindActiveComplianceForms(bool value)
        {
            var Filter = Builders<ComplianceForm>.Filter.Eq("Active", value);
            var collection = _db.GetCollection<ComplianceForm>(typeof(ComplianceForm).Name);
            var entity = collection.Find(Filter).ToList();
            return entity;
        }

        public Task UpdateCollection(ComplianceForm form)
        {
            return _db.GetCollection<ComplianceForm>(typeof(ComplianceForm).Name).
                ReplaceOneAsync(Site => Site.RecId == form.RecId, form);
        }

        public bool DropComplianceForm(object ComplianceFormId)
        {
            var filter = Builders<ComplianceForm>.Filter.Eq("_id", ComplianceFormId);
            var collection = _db.GetCollection<ComplianceForm>(typeof(ComplianceForm).Name);
            var entity = collection.DeleteOne(filter);
            return true;
        }
    }
}
