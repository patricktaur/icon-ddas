using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.ViewModels
{
    public class StudySpecificInvestigatorVM
    {
        //Project Number	Sponsor Protocol Number	Role	Investigator Name	Institute	Country	Medical License Number	Review Completed On	Finding Status	Assigned To

        public string ProjectNumber { get; set; }
        public string ProjectNumber2 { get; set; }
        public string SponsorProtocolNumber { get; set; }
        public string SponsorProtocolNumber2 { get; set; }
        public string InvestigatorName { get; set; }
        public string Role { get; set; }
        public string MedicalLicenseNumber { get; set; }
        public string Institute { get; set; }
        public string Country { get; set; }
        public DateTime ReviewCompletedOn { get; set; }
        public string FindingStatus { get; set; }
        public string AssignedTo { get; set; }
    }
}
