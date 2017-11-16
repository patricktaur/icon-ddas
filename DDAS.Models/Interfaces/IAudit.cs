using DDAS.Models.Entities.Domain;
using DDAS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Interfaces
{
    public interface IAudit
    {
        bool RequestAudit(Audit Audit);
        List<AuditListViewModel> ListAudits();
        Audit GetAudit(Guid RecId);
        bool SaveAudit(Audit Audit);
    }
}
