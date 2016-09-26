using DDAS.Models.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ConsoleApplication1
{
    
    class MongoDBRepository : IRepository<MongoDBRepository>
    {
        public void Add(MongoDBRepository entity)
        {
            throw new NotImplementedException();
        }

        public MongoDBRepository FindById(object id)
        {
            throw new NotImplementedException();
        }

        public Task<MongoDBRepository> FindByIdAsync(object id)
        {
            throw new NotImplementedException();
        }

        public Task<MongoDBRepository> FindByIdAsync(CancellationToken cancellationToken, object id)
        {
            throw new NotImplementedException();
        }

        public List<MongoDBRepository> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<List<MongoDBRepository>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<MongoDBRepository>> GetAllAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public List<MongoDBRepository> PageAll(int skip, int take)
        {
            throw new NotImplementedException();
        }

        public Task<List<MongoDBRepository>> PageAllAsync(int skip, int take)
        {
            throw new NotImplementedException();
        }

        public Task<List<MongoDBRepository>> PageAllAsync(CancellationToken cancellationToken, int skip, int take)
        {
            throw new NotImplementedException();
        }

        public void Remove(MongoDBRepository entity)
        {
            throw new NotImplementedException();
        }

        public void Update(MongoDBRepository entity)
        {
            throw new NotImplementedException();
        }
    }
}
