using DDAS.Models.Repository;
using MongoDB.Driver;


namespace DDAS.Data.Mongo.Repositories
{
    internal class LogRepository : Repository<Log>, ILogRepository
    {
        internal LogRepository(IMongoDatabase db)
            : base(db)
        {

        }
    }
}
