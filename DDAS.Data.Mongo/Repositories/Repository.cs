using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DDAS.Models.Repository;
//using Norm;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using MongoDB.Driver;




namespace DDAS.Data.Mongo.Repositories
{

    internal class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        //private IMongo _provider;
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
            //_db.GetCollection<TEntity>().Save(entity);
            _db.GetCollection<TEntity>(typeof(TEntity).Name).InsertOne(entity);
        }

        public TEntity FindById(object id)
        {
            //Can this return more than one item? 
            //var filter = Builders<TEntity>.Filter.Eq("id", ObjectId.Parse(id.ToString()));

            var filter = Builders<TEntity>.Filter.Eq("_id", id);
            var collection = _db.GetCollection<TEntity>(typeof(TEntity).Name);
            var entity = collection.Find(filter).FirstOrDefault();

            return entity;
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
            var collections = _db.GetCollection<TEntity>(typeof(TEntity).Name);
            var documents = collections.Find(_ => true).ToList();
            return documents;
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
            //_db.GetCollection<TEntity>().Delete(entity);
        }

        public void Update(TEntity entity)
        {
            //same as Add:
            //??entity, entity ?
            //_db.GetCollection<TEntity>().UpdateOne(entity, entity);

           



        }
    }
}
