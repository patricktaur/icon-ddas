using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models.Repository.Domain.MatchedSiteData;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Data.Mongo.Repositories.MathedRecordsSiteData
{
    internal class FDADebarMatchedRecordsRepository : 
        Repository<FDADebarPageMatchRecords>, IFDADebarMatchedRecordsRepository
    {
        public FDADebarMatchedRecordsRepository(IMongoDatabase db) : base(db)
        {

        }
    }
}
