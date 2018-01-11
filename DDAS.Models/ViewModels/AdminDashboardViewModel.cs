using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public string UserName { get; set; }
        public int OpeningBalance { get; set; }
        public int InvestigatorUploaded { get; set; }
        public int InvestigatorReviewCompleted { get; set; }
        public int ClosingBalance {
            get {
                return (OpeningBalance + InvestigatorUploaded) - 
                    InvestigatorReviewCompleted;
            }
        }
    }
}
