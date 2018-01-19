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
        bool RequestQC(ComplianceForm Form);
        List<QCListViewModel> ListQCs();
        ComplianceForm GetQC(Guid RecId, string AssignedTo, string LoggedInUserName);
        bool SaveQC(ComplianceForm Form);
        List<QCSummaryViewModel> ListQCSummary(Guid ComplianceFormId);
        bool UndoQCRequest(Guid ComplianceFormId);
        //bool UndoQCSubmit(Guid ComplianceFormId);
        //bool Undo(string ComplianceFormId);
    }
}
