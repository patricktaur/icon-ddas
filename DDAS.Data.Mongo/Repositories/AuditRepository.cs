using DDAS.Models.Entities.Domain;
using DDAS.Models.Repository.Domain.SiteData;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Data.Mongo.Repositories
{
    internal class AuditRepository : Repository<Audit>, IAuditRepository
    {
        private IMongoDatabase _db;
        public AuditRepository(IMongoDatabase db): base(db) 
        {
            _db = db;
        }

        public Task UpdateAudit(Audit audit)
        {
            return _db.GetCollection<Audit>(typeof(Audit).Name).
                ReplaceOneAsync(s => s.RecId == audit.RecId, audit);
        }
    }
}
