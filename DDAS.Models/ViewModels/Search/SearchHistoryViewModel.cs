using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.ViewModels.Search
{
    public class SearchHistoryViewModel
    {

        public string searchedOn { get; set; }
        public string searchedBy { get; set; }
        public string searchName { get; set; }
        public int searchCount { get; set; }

    }

    public class StudyNumberViewModel
    {
        public string StudyNumber { get; set; }

    }
}

