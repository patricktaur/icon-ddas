using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using OpenQA.Selenium;
using WebScraping.Selenium.BaseClasses;

namespace WebScraping.Selenium.Pages
{
    public partial class CorporateIntegrityAgreementsListPage : BaseSearchPage
    {
        public CorporateIntegrityAgreementsListPage(IWebDriver driver) : base(driver)
        {
            Open();
        }

        public override SiteEnum SiteName {
            get {
                return SiteEnum.CorporateIntegrityAgreementsListPage;
            }
        }

        public override string Url {
            get {
                return @"http://oig.hhs.gov/compliance/corporate-integrity-agreements/cia-documents.asp";
            }
        }


        private List<CIAList> _ciaList;

        public List<CIAList> CorporateIntegrityAgreementList {
            get {
                return _ciaList;
            }
        }

        public void LoadCIAList()
        {
            _ciaList = new List<CIAList>();

            IList<IWebElement> TRs = CIAListTable.FindElements(By.XPath("//tbody/tr"));

            for (int TableRow = 9; TableRow < TRs.Count; TableRow++)
            {
                var CiaList = new CIAList();

                IList<IWebElement> TDs = TRs[TableRow].FindElements(By.XPath("td"));

                if(TDs.Count > 0)
                {
                    CiaList.Provider = TDs[0].Text;
                    CiaList.City = TDs[1].Text;
                    CiaList.State = TDs[2].Text;
                    CiaList.Effective = TDs[3].Text;

                    _ciaList.Add(CiaList);
                }
            }
        }

        public override ResultAtSite GetResultAtSite(string NameToSearch)
        {
            ResultAtSite searchResult = new ResultAtSite();

            searchResult.SiteName = SiteName.ToString();

            foreach (CIAList CiaList in _ciaList)
            {
                string WordFound = FindSubString(CiaList.Provider, NameToSearch);

                if (WordFound != null)
                {
                    searchResult.SiteName = SiteName.ToString();

                    searchResult.Results.Add(new MatchResult
                    {
                        MatchName = CiaList.Provider,
                        MatchLocation = "Word(s) matched - " + WordFound
                    });
                }
            }

            if (searchResult.Results.Count == 0)
            {
                searchResult.Results.Add(new MatchResult
                {
                    MatchName = "None",
                    MatchLocation = "None"
                });
                return searchResult;
            }
            else
                return searchResult;
        }

        public override void LoadContent(string NameToSearch)
        {
            NameToSearch.Replace(",", "");

            LoadCIAList();
        }

        public class CIAList
        {
            public string Provider { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string Effective { get; set; }
        }

    }
}
