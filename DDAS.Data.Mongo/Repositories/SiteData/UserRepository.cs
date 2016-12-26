using DDAS.Models.Entities.Domain;
using DDAS.Models.Repository.Domain.SiteData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using MongoDB.Driver;

namespace DDAS.Data.Mongo.Repositories.SiteData
{
    internal class UserRepository : Repository<User>, IUserRepository
    {
        private IMongoDatabase _db;
        public UserRepository(IMongoDatabase db) : base(db)
        {
            _db = db;
        }
        public Task AddAsync(User entity)
        {
            //_db.GetCollection<TEntity>().Save(entity);
            return _db.GetCollection<User>(typeof(User).Name).InsertOneAsync(entity);
        }

        public object GetAllUsers()
        {
            //var filter = Builders<User>.Filter.Eq("UserName", UserName);
            var collection = _db.GetCollection<User>(typeof(User).Name);
            var documents = collection.Find(_ => true).ToList();
            //var entity = collection.Find(filter).FirstOrDefault();
            return documents;
        }

        public User FindByUserName(string UserName)
        {
            //Patrick 21Dec2016
            //var filter = Builders<User>.Filter.Eq("UserName", UserName);
            //var collection = _db.GetCollection<User>(typeof(User).Name);
            //var entity = collection.Find(filter).FirstOrDefault();

            //Case insesnetive for login.
            return GetAll().FirstOrDefault(u => u.UserName.ToLower() == UserName.ToLower());
            
        }

        public Task UpdateUserAsync(string UserName)
        {
            var filter = Builders<User>.Filter.Eq("UserName", UserName);
            var collection = _db.GetCollection<User>(typeof(User).Name);
            var update = Builders<User>.Update.Set("UserName", UserName);
            return collection.UpdateOneAsync(filter,update);
        }        

        public Task<User> FindByUserNameAsync(string UserName)
        {
            throw new NotImplementedException();
        }

        public Task<User> FindByUserNameAsync(CancellationToken cancellationToken, string UserName)
        {
            throw new NotImplementedException();
        }

        //Patrick:
        public Task UpdateUser(User entity)
        {
            return _db.GetCollection<User>(typeof(User).Name).ReplaceOneAsync(item => item.UserId == entity.UserId, entity );
  
        }

      

    }
}
