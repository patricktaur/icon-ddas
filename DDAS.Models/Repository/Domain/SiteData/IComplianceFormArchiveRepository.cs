using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DDAS.Models.Repository.Domain.SiteData
{
    public interface IComplianceFormArchiveRepository : IRepository<ComplianceFormArchive>
    {
        ComplianceFormArchive FindComplianceFormIdByNameToSearch(string NameToSearch);
        List<ComplianceFormArchive> FindActiveComplianceForms(bool value);
        Task UpdateCollection(ComplianceFormArchive form);
        bool DropComplianceForm(object ComplianceFormId);

        bool UpdateAssignedTo(Guid id, string AssignedBy, string AssignedFrom, string AssignedTo);
        bool UpdateComplianceForm(Guid id, ComplianceFormArchive form);
        bool UpdateInvestigator(Guid formId, InvestigatorSearched Investigator);
        bool UpdateFindings(UpdateFindigs updateFindings);

        bool AddAttachmentsToFindings(List<Attachment> Attachments);

        bool AddFindings(Guid formId, List<Finding> findings);
        bool UpdateExtractionQueStart(Guid id, DateTime? dateValue, int QueueNumber);
        bool UpdateExtractionQueEnd(Guid id, DateTime? dateValue);

        List<ComplianceFormArchive> FindComplianceForms(string AssignedTo, ReviewStatusEnum ReviewStatus);
        List<ComplianceFormArchive> FindComplianceForms(string AssignedTo);
        List<ComplianceFormArchive> FindComplianceForms(ReviewStatusEnum ReviewStatus);
        List<ComplianceFormArchive> FindComplianceForms(ComplianceFormFilter CompFormFilter);

        List<ComplianceFormArchive> FindQCComplianceForms(DateTime? ReviewAssignedOn, DateTime? ReviewCompletedOn);
    }
}
