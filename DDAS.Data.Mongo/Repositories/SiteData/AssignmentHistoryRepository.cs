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
    class AssignmentHistoryRepository : Repository<AssignmentHistory>, IAssignmentHistoryRepository
    {
        public AssignmentHistoryRepository(IMongoDatabase db): base(db)
        {

        }
    }
}
