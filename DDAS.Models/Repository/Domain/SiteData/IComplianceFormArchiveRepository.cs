using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DDAS.Models.Repository.Domain.SiteData
{
    public interface IComplianceFormArchiveRepository : IRepository<ComplianceFormArchive>
    {
        List<ComplianceFormArchive> FindComplianceForms(ComplianceFormArchiveFilter CompFormFilter);
        ComplianceFormArchive FindByComplianceFormId(string RecId);
        bool DropComplianceForm(object ComplianceFormId);
    }
}
