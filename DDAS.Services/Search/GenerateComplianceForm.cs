using DDAS.Models;
using System;
using Utilities.WordTemplate;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Services.Search
{
    public class GenerateComplianceForm
    {
        private IUnitOfWork _UOW;
        public GenerateComplianceForm(IUnitOfWork UOW)
        {
            _UOW = UOW;
        }

        public void GetComplianceForm(Guid? ComplianceFormId)
        {
            var complianceform = 
                _UOW.ComplianceFormRepository.FindById(ComplianceFormId);

            var WordTemplate = new ReplaceTextFromWordTemplate();

            WordTemplate.ReplaceTextFromWord(complianceform, "", "");

        }
    }
}
