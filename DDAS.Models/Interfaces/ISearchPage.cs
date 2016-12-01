using DDAS.Models.Entities.Domain.SiteData;
using System.Collections.Generic;

namespace DDAS.Models.Interfaces
{
    public interface ISearchPage
    {
        void LoadContent(string NameToSearch, string DownloadFolder);
        void SaveData();

        IEnumerable<SiteDataItemBase> SiteData { get; }
        //ResultAtSite GetResultAtSite(string NameToSearch);
    }
}
