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
    internal class CountryRepository: Repository<Country>,
        ICountryRepository
    {
        private IMongoDatabase _db;
        public CountryRepository(IMongoDatabase db): base(db)
        {
            _db = db;
        }

        public bool UpdateCountry(Country country)
        {
            _db.GetCollection<Country>(typeof(Country).Name).
                ReplaceOne(x => x.RecId == country.RecId, country);
            return true;
        }
    }
}
