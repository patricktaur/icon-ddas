using DDAS.Models.Entities;
using DDAS.Models.Repository;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Data.Mongo.Repositories
{
    internal class ExceptionLoggerRepository : Repository<ExceptionLogger>,
        IExceptionLoggerRepository
    {
        public ExceptionLoggerRepository(IMongoDatabase db): base(db)
        {
        }
    }
}
