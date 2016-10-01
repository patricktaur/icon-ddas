using DDAS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDAS.Models.Repository.Domain;
using System.Threading;
using Norm.Configuration;
using DDAS.Data.Mongo.Maps;
using DDAS.Models.Repository.Domain.SiteData;
using DDAS.Data.Mongo.Repositories.SiteData;
using MongoDB.Bson.Serialization;
using DDAS.Models.Entities.Domain.SiteData;
using MongoDB.Driver;

namespace DDAS.Data.Mongo
{
    public class UnitOfWork : IUnitOfWork
    {
        //private IMongo _provider;
        //private IMongoDatabase _db { get { return _provider.Database; } }
        private IMongoDatabase _db;
        #region PrivateRepositoryMemebers
        private IFDADebarPageRepository _FDADebarPageRepository;
        private IAdequateAssuranceListRepository _AdequateAssuranceListRepository;
        private IERRProposalToDebarRepository _ERRProposalToDebarRepository;
        private IPHSAdministrativeActionListingRepository 
            _PHSAdministrativeActionListingRepository;
        #endregion
        /*
         * var client = new MongoClient("mongodb://127.0.0.1");
            var DB = client.GetDatabase("DDAS");
         * */

        #region Constructor
        public UnitOfWork(string nameOrConnectionString)
        {
            //InitializeMaps();
            //_provider = Norm.Mongo.Create(nameOrConnectionString);

            var mongo = new MongoClient("mongodb://127.0.0.1");
             _db = mongo.GetDatabase("DDAS");
        }
        #endregion

        #region IUnitOfWork Members

        public IFDADebarPageRepository FDADebarPageRepository
        {
            get
            {
                return _FDADebarPageRepository ?? 
                    (_FDADebarPageRepository = new FDADebarPageRepository(_db));
            }
        }
        public IAdequateAssuranceListRepository AdequateAssuranceListRepository
        {
            get
            {
                return _AdequateAssuranceListRepository ?? 
                (_AdequateAssuranceListRepository = 
                new AdequateAssuranceListRepository(_db));
            }
        }

        public IERRProposalToDebarRepository ERRProposalToDebarRepository
        {
            get
            {
                return _ERRProposalToDebarRepository ?? 
                    (_ERRProposalToDebarRepository = new ERRProposalToDebarRepository(_db));
            }
        }

        public IPHSAdministrativeActionListingRepository PHSAdministrativeActionListingRepository
        {
            get
            {
                return _PHSAdministrativeActionListingRepository ??
                    (_PHSAdministrativeActionListingRepository =
                    new PHSAdministrativeActionListingRepository(_db));
            }
        }

        #endregion
        private void InitializeMaps()
        {
            MongoMaps.Initialize();
            //BsonClassMap.RegisterClassMap<FDADebarPageSiteData>(map =>
            //{
            //    map.MapIdProperty(u => u.RecId);
            //});
            //MongoConfiguration.Initialize(config =>
            //config.AddMap<FDADebarPageSiteDataMap>());

            //MongoConfiguration.Initialize(config =>
            //config.AddMap<ERRProposalToDebarPageSiteDataMap>());

            //MongoConfiguration.Initialize(config =>
            //config.AddMap<PHSAdministrativeActionListingSiteDataMap>());

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

            
            //_provider.Dispose();
           
        }
    }
}