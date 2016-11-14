using DDAS.Models;
using DDAS.Models.Entities.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            form.SearchStartedOn = DateTime.Now;
            foreach(SitesIncludedInSearch site in form.SiteDetails)
            {
                //form.Sites_FullMatchCount += site.FullMatchCount;
                //form.Sites_PartialMatchCount += site.PartialMatchCount;
                var MatchedList = site.MatchedRecords.Where(
                    item => item.Matched > 1).ToList();
                var FullMatchedList = site.MatchedRecords.Where(
                    item => item.Matched > 2).ToList();
                form.Sites_FullMatchCount += MatchedList.Count;
                form.Sites_PartialMatchCount += FullMatchedList.Count;
                site.MatchedRecords = MatchedList;
            }

            form.Sites_MatchStatus = form.Sites_FullMatchCount +
                " full matches and " +
                form.Sites_PartialMatchCount +
                " partial matches in " + 
                form.SiteDetails.Count + 
                " sites";

            _UOW.ComplianceFormRepository.Add(form);
        }

        public ComplianceForm GetComplianceFormId(string NameToSearch)
        {
            var complianceForm = _UOW.ComplianceFormRepository.
                FindComplianceFormIdByNameToSearch(NameToSearch);

            return complianceForm;
        }
    }
}
