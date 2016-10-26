using DDAS.Models.Entities.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Interfaces
{
    public interface ISearchPage
    {
        void LoadContent(string NameToSearch);
        void SaveData();
       
        //ResultAtSite GetResultAtSite(string NameToSearch);
    }
}
