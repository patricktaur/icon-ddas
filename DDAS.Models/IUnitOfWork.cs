using DDAS.Models.Repository.Domain;
using DDAS.Models.Repository.Domain.SiteData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        #endregion


        #endregion

        #region Methods

        int SaveChanges();
            Task<int> SaveChangesAsync();
            Task<int> SaveChangesAsync(CancellationToken cancellationToken);

        #endregion
    }
}
