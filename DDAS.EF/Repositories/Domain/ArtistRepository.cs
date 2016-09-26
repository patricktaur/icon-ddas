using DDAS.EF;
using DDAS.Models.Repository.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDAS.Models.Entities.Domain;
using System.Threading;

namespace DDAS.EF.Repositories.Domain
{
    internal class ArtistRepository : Repository<Artist>, IFDADebarPageRepository

    {
        private ApplicationIdentityDBContext _ctx;

        internal ArtistRepository(ApplicationIdentityDBContext context)
            : base(context)
        {
            _ctx = context;
        }

        public Artist UpdateRecord(Artist ArtistModel)
        {
            if (ArtistModel.RecId == 0)
            {
                //is new record
                Add(ArtistModel);
            }
            else
            {
                //existing record. load it
                Update(ArtistModel);
            }

            return ArtistModel;
        }
    }
}
