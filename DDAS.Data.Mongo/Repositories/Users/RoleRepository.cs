using DDAS.Models.Entities.Domain;
using DDAS.Models.Repository.Domain.SiteData;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace DDAS.Data.Mongo.Repositories.SiteData
{
    internal class RoleRepository : Repository<Role>, IRoleRepository
    {
        private IMongoDatabase _db;
        public RoleRepository(IMongoDatabase db) : base(db)
        {
            _db = db;
        }

        public Role FindByName(string RoleName)
        {
            var filter = Builders<Role>.Filter.Eq("Name", RoleName);
            var collection = _db.GetCollection<Role>(typeof(Role).Name);
            var entity = collection.Find(filter).FirstOrDefault();
            return entity;
        }

        public Task AddAsync(Role entity)
        {
            //_db.GetCollection<TEntity>().Save(entity);
            return _db.GetCollection<Role>(typeof(Role).Name).InsertOneAsync(entity);
        }

        public Task<Role> FindByNameAsync(CancellationToken cancellationToken, string RoleName)
        {
            throw new NotImplementedException();
        }

        public Task<Role> FindByNameAsync(string RoleName)
        {
            throw new NotImplementedException();
        }
    }
}
