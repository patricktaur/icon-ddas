using DDAS.Models.Entities.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.ViewModels
{
    public class CurrentReviewStatusViewModel
    {
        public Guid? ReviewerRecId { get; set; }
        public Guid? QCVerifierRecId { get; set; }
        public Review CurrentReview { get; set; }
    }
}
