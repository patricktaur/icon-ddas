using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DDAS.Models.Repository.Domain.SiteData
{
    public interface IComplianceFormRepository : IRepository<ComplianceForm>
    {
        ComplianceForm FindComplianceFormIdByNameToSearch(string NameToSearch);
        List<ComplianceForm> FindActiveComplianceForms(bool value);
        Task UpdateCollection(ComplianceForm form);
        bool DropComplianceForm(object ComplianceFormId);

        bool UpdateAssignedTo(Guid id, string AssignedTo);
        bool UpdateComplianceForm(Guid id, ComplianceForm form);
        bool UpdateInvestigator(Guid formId, InvestigatorSearched Investigator);
        bool UpdateFindings(UpdateFindigs updateFindings);

        bool AddAttachmentsToFindings(List<Attachment> Attachments);

        bool AddFindings(Guid formId, List<Finding> findings);
        bool UpdateExtractionQueStart(Guid id, DateTime? dateValue, int QueueNumber);
        bool UpdateExtractionQueEnd(Guid id, DateTime? dateValue);
    }
}
