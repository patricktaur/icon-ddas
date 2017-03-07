using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Interfaces
{
    public interface IAppAdmin
    {
        List<DataExtractionHistory> GetDataExtractionHistory();
        List<DataExtractionHistory> GetDataExtractionPerSite(SiteEnum Enum);
        void DeleteExtractionEntry(SiteEnum Enum, Guid? RecId);
    }
}
