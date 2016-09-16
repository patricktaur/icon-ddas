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
    public partial class PHSAdministrativeActionListingPage : BaseSearchPage
    {
        public PHSAdministrativeActionListingPage(IWebDriver driver) : base(driver)
        {
            Open();
        }

        public override SiteEnum SiteName {
            get {
                return SiteEnum.PHSAdministrativeActionListingPage;
            }
        }

        public override string Url {
            get {
                return @"https://ori.hhs.gov/ORI_PHS_alert.html?d=update";
            }
        }

        private List<PHSAdministrativeAction> _administrativeActionList;

        public List<PHSAdministrativeAction> AdministrativeActionList {
            get {
                return _administrativeActionList;
            }
        }

        public void LoadAdministrativeActionLists()
        {
            _administrativeActionList = new List<PHSAdministrativeAction>();

            IList<IWebElement> TRs = PHSTable.FindElements(By.XPath("//tbody/tr"));

            foreach (IWebElement TR in TRs)
            {
                var AdministrativeActionListing = new PHSAdministrativeAction();

                IList<IWebElement> TDs = TR.FindElements(By.XPath("td"));

                if (TDs.Count > 0)
                {
                    AdministrativeActionListing.LastName = TDs[0].Text;
                    AdministrativeActionListing.FirstName = TDs[1].Text;
                    AdministrativeActionListing.MiddleName = TDs[2].Text;
                    AdministrativeActionListing.DebarmentUntil = TDs[3].Text;
                    AdministrativeActionListing.NoPHSAdvisoryUntil = TDs[4].Text;
                    AdministrativeActionListing.CertificationOfWorkUntil = TDs[5].Text;
                    AdministrativeActionListing.SupervisionUntil = TDs[6].Text;
                    AdministrativeActionListing.RetractionOfArticle = TDs[7].Text;
                    AdministrativeActionListing.CorrectionOfArticle = TDs[8].Text;
                    AdministrativeActionListing.Memo = TDs[9].Text;

                    _administrativeActionList.Add(AdministrativeActionListing);
                }
            }
        }

        public override ResultAtSite GetResultAtSite(string NameToSearch)
        {
            ResultAtSite searchResult = new ResultAtSite();

            searchResult.SiteName = SiteName.ToString();

            foreach (PHSAdministrativeAction AdminAction in _administrativeActionList)
            {
                string FullName = AdminAction.FirstName + " " + AdminAction.MiddleName + " " +
                        AdminAction.LastName;

                string WordFound = FindSubString(FullName, NameToSearch);

                if (WordFound != null)
                {
                    searchResult.SiteName = SiteName.ToString();

                    searchResult.Results.Add(new MatchResult
                    {
                        MatchName = FullName,
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

            LoadAdministrativeActionLists();
        }

        public class PHSAdministrativeAction
        {
            public string LastName { get; set; }
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string DebarmentUntil { get; set; }
            public string NoPHSAdvisoryUntil { get; set; }
            public string CertificationOfWorkUntil { get; set; }
            public string SupervisionUntil { get; set; }
            public string RetractionOfArticle { get; set; }
            public string CorrectionOfArticle { get; set; }
            public string Memo { get; set; }
        }
    }
}
