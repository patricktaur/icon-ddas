using DDAS.Models.Entities.Domain;
using DDAS.Models.Entities.Domain.SiteData;
using System;
using System.Collections.Generic;

namespace DDAS.Models.Interfaces
{
    public interface ISearchPage
    {
        void LoadContent(string DownloadFolder);
        void LoadContent(
            string NameToSearch, 
            string DownloadFolder,
            string ErrorScreenCapture,
            int MatchCountLowerLimit);
        void SaveData();
        DateTime? SiteLastUpdatedDateFromPage { get; }
        //DateTime? SiteLastUpdatedDateFromDatabase { get; }
        IEnumerable<SiteDataItemBase> SiteData { get; }
        BaseSiteData baseSiteData { get; }
    }
}
