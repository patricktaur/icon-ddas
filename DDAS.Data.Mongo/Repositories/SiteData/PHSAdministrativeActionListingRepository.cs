using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models.Repository.Domain.SiteData;
using Norm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Data.Mongo.Repositories.SiteData
{
    internal class PHSAdministrativeActionListingRepository :
        Repository<PHSAdministrativeActionListingSiteData>, 
        IPHSAdministrativeActionListingRepository
    {
        public PHSAdministrativeActionListingRepository(IMongoDatabase db) : base(db)
        {

        }
    }
}
