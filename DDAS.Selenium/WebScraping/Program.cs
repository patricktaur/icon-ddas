using DDAS.Data.Mongo;
using DDAS.Models;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using DDAS.Services.Search;
using System.Collections.Generic;

namespace WebScraping
{
    class Program
    {
        static void Main(string[] args)
        {
            test t = new test();
            t.TestConnection();
            //IUnitOfWork uow = new UnitOfWork("DefaultConnection");
            //var test = new Search(uow);
            //List<SearchQuerySite> SearchList = new List<SearchQuerySite>();

            //SearchQuerySite search = new SearchQuerySite { SiteEnum = SiteEnum.FDADebarPage,
            //SiteName = "FDADebarPage",
            //SiteShortName = "FDA Debar",
            //SiteUrl = "ABC"};

            //SearchList.Add(search);

            //var query = new SearchQuery { NameToSearch = "", SearchSites = SearchList };

            //test.GetSearchSummary(query);
        }
    }
}
