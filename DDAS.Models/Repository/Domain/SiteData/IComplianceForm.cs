using DDAS.Models.Entities.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DDAS.Models.Repository.Domain.SiteData
{
    public interface IComplianceFormRepository : IRepository<ComplianceForm>
    {
        ComplianceForm FindComplianceFormIdByNameToSearch(string NameToSearch);
        List<ComplianceForm> FindActiveComplianceForms(bool value);
        Task UpdateCollection(ComplianceForm form);
    }
}
