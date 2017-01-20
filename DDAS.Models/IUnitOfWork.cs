using DDAS.Models.Repository.Domain.SiteData;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DDAS.Models
{
    public interface IUnitOfWork : IDisposable
    {
        #region Properties

        #region SiteData
        IFDADebarPageRepository FDADebarPageRepository { get; }
        IAdequateAssuranceListRepository AdequateAssuranceListRepository { get; }
        IERRProposalToDebarRepository ERRProposalToDebarRepository { get; }
        IPHSAdministrativeActionListingRepository 
            PHSAdministrativeActionListingRepository { get; }
        IClinicalInvestigatorInspectionListRepository
            ClinicalInvestigatorInspectionListRepository { get; }
        ISaveSearchResultRepository
            SaveSearchResultRepository { get; }
        ISpeciallyDesignatedNationalsRepository
            SpeciallyDesignatedNationalsRepository { get; }
        IRoleRepository RoleRepository { get; }
        IUserRepository UserRepository { get; }
        IUserRoleRepository UserRoleRepository { get; }
        ICBERClinicalInvestigatorInspectionRepository 
            CBERClinicalInvestigatorRepository { get; }
        IExclusionDatabaseSearchRepository 
            ExclusionDatabaseSearchRepository { get; }
        ICorporateIntegrityAgreementRepository 
            CorporateIntegrityAgreementRepository { get; }
        IFDAWarningLettersRepository FDAWarningLettersRepository { get; }
        IClinicalInvestigatorDisqualificationRepository
            ClinicalInvestigatorDisqualificationRepository { get; }
        ISystemForAwardManagementRepository 
            SystemForAwardManagementRepository { get; }
        IComplianceFormRepository ComplianceFormRepository { get; }
        
        ILoginDetailsRepository LoginDetailsRepository { get; }
        #endregion

        #endregion

        #region Methods

        int SaveChanges();
            Task<int> SaveChangesAsync();
            Task<int> SaveChangesAsync(CancellationToken cancellationToken);

        #endregion
    }
}
