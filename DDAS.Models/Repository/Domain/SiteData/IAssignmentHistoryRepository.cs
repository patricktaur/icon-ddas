using DDAS.Models.Entities.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Repository.Domain.SiteData
{
    public interface IAssignmentHistoryRepository : IRepository<AssignmentHistory>
    {
        AssignmentHistory GetAssignmentHistoryByComplianceFormId(Guid Id);
        bool UpdateRemovedOn(Guid Id);
    }
}
