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
            _UOW.ComplianceFormRepository.Add(form);
        }

        public Guid? GetComplianceFormId(string NameToSearch)
        {
            var complianceForm = _UOW.ComplianceFormRepository.
                FindComplianceFormIdByNameToSearch(NameToSearch);

            return complianceForm.RecId;
        }
    }
}
