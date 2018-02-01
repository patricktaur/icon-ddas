using DDAS.Models.Repository;
using DDAS.Models.Repository.Domain.SiteData;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DDAS.Models
{
    public interface IUnitOfWork : IDisposable
    {
        #region Properties

        ILoginDetailsRepository LoginDetailsRepository { get; }
        ILogRepository LogRepository { get; }
        ISiteSourceRepository SiteSourceRepository { get; }
        ICountryRepository CountryRepository { get; }
        ISponsorProtocolRepository SponsorProtocolRepository { get; }
        IDefaultSiteRepository DefaultSiteRepository { get; }

        IExceptionLoggerRepository ExceptionLoggerRepository { get; }

        IAuditRepository AuditRepository { get; }
        IAssignmentHistoryRepository AssignmentHistoryRepository { get; }

        ILogWSDDASRepository LogWSDDASRepository { get; }
        ILogWSISPRINTRepository LogWSISPRINTRepository { get; }


        #region SiteData
        IFDADebarPageRepository FDADebarPageRepository { get; }

        IAdequateAssuranceListRepository AdequateAssuranceListRepository { get; }

        IERRProposalToDebarRepository ERRProposalToDebarRepository { get; }

        IPHSAdministrativeActionListingRepository 
            PHSAdministrativeActionListingRepository { get; }

        IClinicalInvestigatorInspectionListRepository
            ClinicalInvestigatorInspectionListRepository { get; }
        IClinicalInvestigatorInspectionRepository
            ClinicalInvestigatorInspectionRepository { get; }

        ISaveSearchResultRepository
            SaveSearchResultRepository { get; }

        ISpeciallyDesignatedNationalsRepository
            SpeciallyDesignatedNationalsRepository { get; }
        ISDNSiteDataRepository SDNSiteDataRepository { get; }

        IRoleRepository RoleRepository { get; }

        IUserRepository UserRepository { get; }

        IUserRoleRepository UserRoleRepository { get; }

        ICBERClinicalInvestigatorInspectionRepository 
            CBERClinicalInvestigatorRepository { get; }

        IExclusionDatabaseSearchRepository 
            ExclusionDatabaseSearchRepository { get; }
        IExclusionDatabaseRepository 
            ExclusionDatabaseRepository { get; }

        ICorporateIntegrityAgreementRepository 
            CorporateIntegrityAgreementRepository { get; }

        IFDAWarningLettersRepository FDAWarningLettersRepository { get; }
        IFDAWarningRepository FDAWarningRepository { get; }

        IClinicalInvestigatorDisqualificationRepository
            ClinicalInvestigatorDisqualificationRepository { get; }

        ISystemForAwardManagementRepository 
            SystemForAwardManagementRepository { get; }
        ISystemForAwardManagementEntityRepository
            SystemForAwardManagementEntityRepository { get; }
        ISAMSiteDataRepository SAMSiteDataRepository { get; }

        IComplianceFormRepository ComplianceFormRepository { get; }
        #endregion

        #endregion

        #region Methods

        int SaveChanges();
            Task<int> SaveChangesAsync();
            Task<int> SaveChangesAsync(CancellationToken cancellationToken);

        #endregion
    }
}
