using DDAS.Models.Entities;
using DDAS.Models.Repository;
using MongoDB.Driver;


namespace DDAS.Data.Mongo.Repositories
{
    internal class LogWSISPRINTRepository : Repository<LogWSISPRINT>, ILogWSISPRINTRepository
    {
        internal LogWSISPRINTRepository(IMongoDatabase db)
            : base(db)
        {

        }
    }
}
