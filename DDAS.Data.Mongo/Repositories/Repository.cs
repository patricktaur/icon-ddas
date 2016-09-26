using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DDAS.Models.Repository;
using Norm;
using System.Configuration;
using DDAS.Data.Mongo.Maps;
using System.Linq;
using Norm.Configuration;
using DDAS.Models.Entities.Domain.SiteData;

namespace DDAS.Data.Mongo.Repositories
{
   
    internal class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private IMongo _provider;
        private IMongoDatabase _db;
        //private IMongoDatabase _db { get { return this._provider.Database; } }

        internal Repository(IMongoDatabase db)
        {
            _db = db;
        }
        internal Repository(string removeThisConstructor)
        {

            //MongoConfiguration.Initialize(config => config.AddMap<FDADebarPageSiteDataMap>());
            //_provider =  Norm.Mongo.Create(ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString);
            
        }

        public void Add(TEntity entity)
        {
            _db.GetCollection<TEntity>().Save(entity);
        }

        public TEntity FindById(object id)
        {
            //Can this return more than one item? 
            return _db.GetCollection<TEntity>().FindOne(new { _id = id }); ;
           
        }

        public Task<TEntity> FindByIdAsync(object id)
        {
            return null;
        }

        public Task<TEntity> FindByIdAsync(CancellationToken cancellationToken, object id)
        {
            throw new NotImplementedException();
        }

        public List<TEntity> GetAll()
        {
            return _db.GetCollection<TEntity>().AsQueryable().ToList<TEntity>();
        }

        public Task<List<TEntity>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public List<TEntity> PageAll(int skip, int take)
        {
            throw new NotImplementedException();
        }

        public Task<List<TEntity>> PageAllAsync(int skip, int take)
        {
            throw new NotImplementedException();
        }

        public Task<List<TEntity>> PageAllAsync(CancellationToken cancellationToken, int skip, int take)
        {
            throw new NotImplementedException();
        }

        public void Remove(TEntity entity)
        {
            _db.GetCollection<TEntity>().Delete(entity);
        }

        public void Update(TEntity entity)
        {
            //same as Add:
            //??entity, entity ?
            _db.GetCollection<TEntity>().UpdateOne(entity, entity);
        }
    }
}
