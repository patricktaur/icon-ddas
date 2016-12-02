using DDAS.Models;
using DDAS.Models.Entities.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace DDAS.Services.Search
{
    public class ComplianceFormService
    {
        private IUnitOfWork _UOW;
        public ComplianceFormService(IUnitOfWork uow)
        {
            _UOW = uow;
        }

        public void CreateComplianceForm(ComplianceForm form)
        {   
            foreach(InvestigatorSearched Investigator in form.InvestigatorDetails)
            {
                foreach (SitesIncludedInSearch site in Investigator.SiteDetails)
                {
                    Investigator.Sites_FullMatchCount +=
                        site.FullMatchCount;
                    Investigator.Sites_PartialMatchCount +=
                        site.PartialMatchCount;
                }
            }
            _UOW.ComplianceFormRepository.Add(form);
        }

        public List<RowData> AddDetailsToComplianceForm(string FilePath)
        {
            var readUploadedExcelFile = new ReadUploadedExcelFile();

            var DataFromExcelFile = readUploadedExcelFile.
                ReadData(FilePath);

            return DataFromExcelFile;
        }

        public ComplianceForm GetComplianceFormId(string NameToSearch)
        {
            var complianceForm = _UOW.ComplianceFormRepository.
                FindComplianceFormIdByNameToSearch(NameToSearch);

            return complianceForm;
        }
    }
}
