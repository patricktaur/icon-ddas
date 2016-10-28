using DDAS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDAS.Models.Repository.Domain;
using System.Threading;
using DDAS.Data.Mongo.Maps;
using DDAS.Models.Repository.Domain.SiteData;
using DDAS.Data.Mongo.Repositories.SiteData;
using MongoDB.Bson.Serialization;
using DDAS.Models.Entities.Domain.SiteData;
using MongoDB.Driver;
using DDAS.Data.Mongo.Repositories;

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
        private IClinicalInvestigatorInspectionListRepository
            _ClinicalInvestigatorInspectionListRepository;
        private ISaveSearchResultRepository
            _SaveSearchResultRepository;
        private ISpeciallyDesignatedNationalsRepository
            _SpeciallyDesignatedNationalsRepository;
        private ICBERClinicalInvestigatorInspectionRepository
            _CBERClinicalInvestigatorRepository;
        private IExclusionDatabaseSearchRepository
            _ExclusionDatabaseSearchRepository;
        private ICorporateIntegrityAgreementRepository
            _CorporateIntegrityAgreemnetRepository;
        private IFDAWarningLettersRepository _FDAWarningLettersRepository;
        private IClinicalInvestigatorDisqualificationRepository
            _ClinicalInvestigatorDisqualificationRepository;
        private ISystemForAwardManagementRepository
            _SystemForAwardManagementRepository;
        private IComplianceFormRepository _ComplianceFormRepository;

        private IRoleRepository _RoleRepository;
        private IUserRepository _UserRepository;
        private IUserRoleRepository _UserRoleRepository;

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

        public IClinicalInvestigatorInspectionListRepository ClinicalInvestigatorInspectionListRepository
        {
            get
            {
                return _ClinicalInvestigatorInspectionListRepository ??
                    (_ClinicalInvestigatorInspectionListRepository =
                    new ClinicalInvestigatorInspectionListRepository(_db));
            }
        }

        public ISaveSearchResultRepository SaveSearchResultRepository
        {
            get
            {
                return _SaveSearchResultRepository ??
                    (_SaveSearchResultRepository = new SaveSearchResultRepository(_db));
            }
        }

        public ISpeciallyDesignatedNationalsRepository SpeciallyDesignatedNationalsRepository
        {
            get
            {
                return _SpeciallyDesignatedNationalsRepository ??
                    (_SpeciallyDesignatedNationalsRepository = 
                    new SpeciallyDesignatedNationalsRepository(_db));
            }
        }

        public IRoleRepository RoleRepository
        {
            get
            {
                return _RoleRepository ??
                    (_RoleRepository = new RoleRepository(_db));
            }
        }

        public IUserRepository UserRepository
        {
            get
            {
                return _UserRepository ??
                    (_UserRepository = new UserRepository(_db));
            }
        }

        public IUserRoleRepository UserRoleRepository
        {
            get
            {
                return _UserRoleRepository ??
                    (_UserRoleRepository = new UserRoleRepository(_db));
            }
        }

        public ICBERClinicalInvestigatorInspectionRepository 
            CBERClinicalInvestigatorRepository
        {
            get
            {
                return _CBERClinicalInvestigatorRepository ??
                    (_CBERClinicalInvestigatorRepository = 
                    new CBERClinicalInvestigatorRepository(_db));
            }
        }

        public IExclusionDatabaseSearchRepository ExclusionDatabaseSearchRepository
        {
            get
            {
                return _ExclusionDatabaseSearchRepository ??
                    (_ExclusionDatabaseSearchRepository = 
                    new ExclusionDatabaseSearchRepository(_db));
            }
        }

        public ICorporateIntegrityAgreementRepository 
            CorporateIntegrityAgreementRepository
        {
            get
            {
                return _CorporateIntegrityAgreemnetRepository ??
                    (_CorporateIntegrityAgreemnetRepository =
                    new CorporateIntegrityAgreementRepository(_db));
            }
        }

        public IFDAWarningLettersRepository FDAWarningLettersRepository
        {
            get
            {
                return _FDAWarningLettersRepository ??
                    (_FDAWarningLettersRepository = new FDAWarningLettersRepository(_db));
            }
        }

        public IClinicalInvestigatorDisqualificationRepository 
            ClinicalInvestigatorDisqualificationRepository
        {
            get
            {
                return _ClinicalInvestigatorDisqualificationRepository ??
                    (_ClinicalInvestigatorDisqualificationRepository = 
                    new ClinicalInvestigatorDisqualificationRepository(_db));
            }
        }

        public ISystemForAwardManagementRepository SystemForAwardManagementRepository
        {
            get
            {
                return _SystemForAwardManagementRepository ??
                    (_SystemForAwardManagementRepository = new 
                    SystemForAwardManagementRepository(_db));
            }
        }

        public IComplianceFormRepository ComplianceFormRepository
        {
            get
            {
                return _ComplianceFormRepository ??
                    (_ComplianceFormRepository = new ComplianceFormRepository(_db));
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