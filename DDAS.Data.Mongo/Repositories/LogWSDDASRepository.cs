using DDAS.Models.Entities;
using DDAS.Models.Repository;
using MongoDB.Driver;

namespace DDAS.Data.Mongo.Repositories
{
    internal class LogWSDDASRepository : Repository<LogWSDDAS>, ILogWSDDASRepository
    {
        internal LogWSDDASRepository(IMongoDatabase db)
            : base(db)
        {

        }
    }
}
