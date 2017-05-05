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
    internal class SponsorProtocolRepository : Repository<SponsorProtocol>,
        ISponsorProtocolRepository
    {
        private IMongoDatabase _db;
        public SponsorProtocolRepository(IMongoDatabase db) : base(db)
        {
            _db = db;
        }

        public bool UpdateSponsorProtocol(SponsorProtocol sponsorProtocol)
        {
            _db.GetCollection<SponsorProtocol>(typeof(SponsorProtocol).Name).
                ReplaceOne(x => x.RecId == sponsorProtocol.RecId, sponsorProtocol);
            return true;
        }
    }
}
