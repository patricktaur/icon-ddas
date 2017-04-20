using DDAS.Models.Interfaces;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class GenerateOutputFile : IGenerateOutputFile
    {
        private SLDocument _Document;

        public GenerateOutputFile(string FilePath)
        {
            _Document = new SLDocument(FilePath);
        }

        public void AddInvestigator(
            string InvestigatorId, string MemberId, 
            string FirstName, string LastName, 
            int FOIDone, DateTime? DateFOIDone, 
            DateTime? DueDiligenceCheckCompletedOn, 
            DateTime? WordCheckCompletedOn, 
            int InvestigatorWorldCheckCompleted, 
            DateTime? InvestigatorWorldCheckCompletedOn, 
            int InstituteWorldCheckCompleted, 
            DateTime? InstituteWorldCheckCompletedOn, 
            string InstituteComplianceIssue, 
            DateTime? DMCCheckCompletedOn, 
            string DMCExclusion, int Row)
        {
            _Document.SetCellValue("A" + Row, InvestigatorId);
            _Document.SetCellValue("B" + Row, MemberId);
            _Document.SetCellValue("C" + Row, FirstName);
            _Document.SetCellValue("D" + Row, LastName);
            _Document.SetCellValue("E" + Row, FOIDone);
            _Document.SetCellValue("F" + Row, 
                DateFOIDone.Value.ToString("dd-MM-yyyy"));

            _Document.SetCellValue("G" + Row, 
                DueDiligenceCheckCompletedOn == null ? "" :
                DueDiligenceCheckCompletedOn.Value.ToString("dd-MM-yyyy"));

            _Document.SetCellValue("H" + Row, 
                WordCheckCompletedOn == null ? "" :
                WordCheckCompletedOn.Value.ToString("dd-MM-yyyy"));

            _Document.SetCellValue("I" + Row, InvestigatorWorldCheckCompleted);

            _Document.SetCellValue("J" + Row, 
                InvestigatorWorldCheckCompletedOn == null ? "" :
                InvestigatorWorldCheckCompletedOn.Value.ToString("dd-MM-yyyy"));

            _Document.SetCellValue("K" + Row, InstituteWorldCheckCompleted);

            _Document.SetCellValue("L" + Row, 
                InstituteWorldCheckCompletedOn == null ? "" :
                InstituteWorldCheckCompletedOn.Value.ToString("dd-MM-yyyy"));

            _Document.SetCellValue("M" + Row, InstituteComplianceIssue);

            _Document.SetCellValue("N" + Row, 
                DMCCheckCompletedOn == null ? "" :
                DMCCheckCompletedOn.Value.ToString("dd-MM-yyyy"));

            _Document.SetCellValue("O" + Row, DMCExclusion);
        }

        public void SaveChanges(string FileSaveAs)
        {
            _Document.SaveAs(FileSaveAs);
        }
    }
}
