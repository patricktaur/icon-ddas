using DDAS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDAS.Models.Repository.Domain;
using System.Threading;
using Norm;
using Norm.Configuration;
using DDAS.Data.Mongo.Maps;
using DDAS.Models.Repository.Domain.SiteData;
using DDAS.Data.Mongo.Repositories.SiteData;

namespace DDAS.Data.Mongo
{
    public class UnitOfWork : IUnitOfWork
    {
        private IMongo _provider;
        private IMongoDatabase _db { get { return this._provider.Database; } }

        #region PrivateRepositoryMemebers
        private IFDADebarPageRepository _FDADebarPageRepository;
        private IAdequateAssuranceListRepository _AdequateAssuranceListRepository;
        #endregion

        #region Constructor
        public UnitOfWork(string nameOrConnectionString)
        {
            InitializeMaps();
            _provider = Norm.Mongo.Create(nameOrConnectionString);
        }
        #endregion

        #region IUnitOfWork Members

        public IFDADebarPageRepository FDADebarPageRepository
        {
            get
            {
                return _FDADebarPageRepository ?? (_FDADebarPageRepository = new FDADebarPageRepository(_db));
            }
        }
        public IAdequateAssuranceListRepository AdequateAssuranceListRepository
        {
            get
            {
                return _AdequateAssuranceListRepository ?? (_AdequateAssuranceListRepository = new AdequateAssuranceListRepository(_db));
            }
        }
        #endregion
        private void InitializeMaps()
        {
            MongoConfiguration.Initialize(config => config.AddMap<FDADebarPageSiteDataMap>());
        }


        #region Methods

        public int SaveChanges()
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveChangesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        #endregion

        public void Dispose()
        {
            _provider.Dispose();
           
        }
    }
}
