using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace DDAS.Models.Repository
{
    public interface IRepository<TEntity> where TEntity : class
    {
        List<TEntity> GetAll();
        Task<List<TEntity>> GetAllAsync();
        Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken);

        List<TEntity> PageAll(int skip, int take);
        Task<List<TEntity>> PageAllAsync(int skip, int take);
        Task<List<TEntity>> PageAllAsync(CancellationToken cancellationToken, int skip, int take);

        TEntity FindById(object id);
        Task<TEntity> FindByIdAsync(object id);
        Task<TEntity> FindByIdAsync(CancellationToken cancellationToken, object id);

        void Add(TEntity entity);
        void Update(TEntity entity);
        void Remove(TEntity entity);
        void RemoveById(object Id);

        bool DropRecord(TEntity Entity);
        bool DropAll(TEntity Entity);

        List<TEntity> GetRecordsByDate(TEntity Entity, DateTime RecordsTillDate);
        List<TEntity> FilterRecordsByDate(TEntity Entity, DateTime FromDate, DateTime ToDate);
        bool MoveCollection(TEntity Entity, string NewCollection);
        bool CollectionExists(string CollectionName);
    }
}
