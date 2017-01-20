using DDAS.Models.Entities.Domain;
using DDAS.Models.Repository.Domain.SiteData;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Data.Mongo.Repositories.SiteData
{
    internal class LoginDetailsRepository : Repository<LoginDetails>, 
        ILoginDetailsRepository
    {
        internal LoginDetailsRepository(IMongoDatabase db) : base(db)
        {

        }
    }
}
