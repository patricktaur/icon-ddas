using DDAS.Models;
using System;
using System.Threading.Tasks;
using System.Threading;
using DDAS.Data.Mongo.Maps;
using DDAS.Models.Repository.Domain.SiteData;
using DDAS.Data.Mongo.Repositories.SiteData;
using MongoDB.Driver;
using DDAS.Data.Mongo.Repositories;
using DDAS.Models.Repository;

namespace DDAS.Data.Mongo
{
    public class UnitOfWork : IUnitOfWork
    {
        //private IMongo _provider;
        //private IMongoDatabase _db { get { return _provider.Database; } }
        private IMongoDatabase _db;

        #region PrivateRepositoryMembers
        private IFDADebarPageRepository _FDADebarPageRepository;

        private IAdequateAssuranceListRepository _AdequateAssuranceListRepository;

        private IERRProposalToDebarRepository _ERRProposalToDebarRepository;

        private IPHSAdministrativeActionListingRepository 
            _PHSAdministrativeActionListingRepository;

        private IClinicalInvestigatorInspectionListRepository
            _ClinicalInvestigatorInspectionListRepository;
        private IClinicalInvestigatorInspectionRepository
            _ClinicalInvestigatorInspectionRepository;

        private ISaveSearchResultRepository
            _SaveSearchResultRepository;

        private ISpeciallyDesignatedNationalsRepository
            _SpeciallyDesignatedNationalsRepository;
        private ISDNSiteDataRepository _SDNSiteDataRepository;

        private ICBERClinicalInvestigatorInspectionRepository
            _CBERClinicalInvestigatorRepository;

        private IExclusionDatabaseSearchRepository
            _ExclusionDatabaseSearchRepository;
        private IExclusionDatabaseRepository 
            _ExclusionDatabaseRepository;

        private ICorporateIntegrityAgreementRepository
            _CorporateIntegrityAgreemnetRepository;

        private IFDAWarningLettersRepository 
            _FDAWarningLettersRepository;
        private IFDAWarningRepository _FDAWarningRepository;

        private IClinicalInvestigatorDisqualificationRepository
            _ClinicalInvestigatorDisqualificationRepository;

        private ISystemForAwardManagementRepository
            _SystemForAwardManagementRepository;
        private ISystemForAwardManagementEntityRepository
            _SystemForAwardManagementEntityRepository;
        private ISAMSiteDataRepository _SAMSiteDataRepository;

        private IComplianceFormRepository _ComplianceFormRepository;

        private IRoleRepository _RoleRepository;

        private IUserRepository _UserRepository;

        private IUserRoleRepository _UserRoleRepository;

        private ILoginDetailsRepository _LoginDetailsRepository;

        private ILogRepository _LogRepository;

        private ISiteSourceRepository _SiteSourceRepository;

        private ICountryRepository _CountryRepository;

        private ISponsorProtocolRepository _SponsorProtocolRepository;

        private IDefaultSiteRepository _DefaultSiteRepository;

        private IExceptionLoggerRepository _ExceptionLoggerRepository;

        private IAuditRepository _AuditRepository;
        private IAssignmentHistoryRepository _AssignmentHistoryRepository;

        #endregion

        /*
         * var client = new MongoClient("mongodb://127.0.0.1");
            var DB = client.GetDatabase("DDAS");
         * */

        #region Constructor
        //public UnitOfWork(string nameOrConnectionString)
        //{
        //    //InitializeMaps();
        //    //_provider = Norm.Mongo.Create(nameOrConnectionString);
        //    //var conn = System.Configuration.ConfigurationManager.ConnectionStrings[nameOrConnectionString].ConnectionString;
        //    //var mongo = new MongoClient("mongodb://127.0.0.1");
        //    //var mongo = new MongoClient(conn);

        //    var mongo = new MongoClient(nameOrConnectionString);

        //    //var arr = nameOrConnectionString.Split('/');

        //    //string db = arr[arr.Length-1];

        //    _db = mongo.GetDatabase("ddastest");
        //    //_db = mongo.GetDatabase(db);

        //    //Forcing exception if Mongo is not running.
        //    try
        //    {
        //        var x = _db.ListCollections();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("MongoDB is not running",  ex);
        //    }
        //}

        public UnitOfWork(string ConnectionString, string DBName)
        {
            var mongo = new MongoClient(ConnectionString);

            _db = mongo.GetDatabase(DBName);

            //Forcing exception if Mongo is not running.
            try
            {
                var x = _db.ListCollections();
            }
            catch (Exception ex)
            {
                throw new Exception("MongoDB is not running", ex);
            }
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

        public IClinicalInvestigatorInspectionRepository
            ClinicalInvestigatorInspectionRepository
        {
            get
            {
                return _ClinicalInvestigatorInspectionRepository ??
                    (_ClinicalInvestigatorInspectionRepository =
                    new ClinicalInvestigatorInspectionRepository(_db));
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

        public ISpeciallyDesignatedNationalsRepository 
            SpeciallyDesignatedNationalsRepository
        {
            get
            {
                return _SpeciallyDesignatedNationalsRepository ??
                    (_SpeciallyDesignatedNationalsRepository = 
                    new SpeciallyDesignatedNationalsRepository(_db));
            }
        }

        public ISDNSiteDataRepository SDNSiteDataRepository
        {
            get
            {
                return _SDNSiteDataRepository ??
                    (_SDNSiteDataRepository = new SDNSiteDataRepository(_db));
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

        public ILoginDetailsRepository LoginDetailsRepository
        {
            get
            {
                return _LoginDetailsRepository ??
                    (_LoginDetailsRepository = new LoginDetailsRepository(_db));
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

        public IExclusionDatabaseRepository ExclusionDatabaseRepository
        {
            get
            {
                return _ExclusionDatabaseRepository ??
                    (_ExclusionDatabaseRepository =
                    new ExclusionDatabaseRepository(_db));
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

        public IFDAWarningRepository FDAWarningRepository
        {
            get
            {
                return _FDAWarningRepository ??
                    (_FDAWarningRepository = new FDAWarningRepository(_db));
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

        public ISystemForAwardManagementEntityRepository
            SystemForAwardManagementEntityRepository
        {
            get
            {
                return _SystemForAwardManagementEntityRepository ??
                    (_SystemForAwardManagementEntityRepository = new
                    SystemForAwardManagementEntityRepository(_db));
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

        public ISAMSiteDataRepository SAMSiteDataRepository
        {
            get
            {
                return _SAMSiteDataRepository ??
                    (_SAMSiteDataRepository = new
                    SAMSiteDataRepository(_db));
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

        public ILogRepository LogRepository
        {
            get
            {
                return _LogRepository ??
                    (_LogRepository = new LogRepository(_db));
            }
        }

        public ISiteSourceRepository SiteSourceRepository
        {
            get
            {
                return _SiteSourceRepository ??
                    (_SiteSourceRepository = new SiteSourceRepository(_db));
            }
        }

        public ICountryRepository CountryRepository
        {
            get
            {
                return _CountryRepository ??
                    (_CountryRepository = new CountryRepository(_db));
            }
        }

        public ISponsorProtocolRepository SponsorProtocolRepository
        {
            get
            {
                return _SponsorProtocolRepository ??
                    (_SponsorProtocolRepository = new SponsorProtocolRepository(_db));
            }
        }

        public IDefaultSiteRepository DefaultSiteRepository
        {
            get
            {
                return _DefaultSiteRepository ??
                    (_DefaultSiteRepository = new DefaultSiteRepository(_db));
            }
        }
        
        public IExceptionLoggerRepository ExceptionLoggerRepository
        {
            get
            {
                return _ExceptionLoggerRepository ??
                    (_ExceptionLoggerRepository = new ExceptionLoggerRepository(_db));
            } 
        }

        public IAuditRepository AuditRepository
        {
            get
            {
                return _AuditRepository ??
                    (_AuditRepository = new AuditRepository(_db));
            }
        }

        public IAssignmentHistoryRepository AssignmentHistory
        {
            get
            {
                return _AssignmentHistoryRepository ??
                    (_AssignmentHistoryRepository = 
                    new AssignmentHistoryRepository(_db));
            }
        }
        #endregion

        private void InitializeMaps()
        {
            MongoMaps.Initialize();
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