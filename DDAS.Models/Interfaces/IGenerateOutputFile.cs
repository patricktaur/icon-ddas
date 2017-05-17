using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Interfaces
{
    public interface IGenerateOutputFile
    {
        void AddInvestigator(
            string InvestigatorId,
            string MemberId,
            string FirstName,
            string LastName,
            int FOIDone,
            DateTime? DateFOIDone,
            DateTime? DueDiligenceCheckCompletedOn,
            DateTime? WordCheckCompletedOn,
            int InvestigatorWorldCheckCompleted,
            DateTime? InvestigatorWorldCheckCompletedOn,
            int InstituteWorldCheckCompleted,
            DateTime? InstituteWorldCheckCompletedOn,
            string InstituteComplianceIssue,
            DateTime? DMCCheckCompletedOn,
            string DMCExclusion, int Row
            );

        void SaveChanges(string FileSaveAs);

        MemoryStream GetMemoryStream();
    }
}
