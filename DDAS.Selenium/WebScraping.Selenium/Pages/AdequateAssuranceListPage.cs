using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using OpenQA.Selenium;
using WebScraping.Selenium.BaseClasses;
using DDAS.Models.Repository;
using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models;

namespace WebScraping.Selenium.Pages
{
    public partial class AdequateAssuranceListPage : BaseSearchPage
    {
        private IUnitOfWork _UOW;

        public AdequateAssuranceListPage(IWebDriver driver, IUnitOfWork uow) : base(driver)
        {
            _UOW = uow;
            Open();
            SavePageImage();
            //SaveScreenShot("AdequateAssuranceListPage.png");
        }

        public override string Url
        {
            get
            {
                return @"http://www.fda.gov/ora/compliance_ref/bimo/asurlist.htm";
            }
        }

        public override SiteEnum SiteName {
            get {
                return SiteEnum.AdequateAssuranceListPage;
            }
        }

        public override void LoadContent(string NameToSearch, string DownloadFolder)
        {
            LoadAdequateAssuranceInvestigators();
        }

        private AdequateAssuranceListSiteData _adequateAssuranceListSiteData;

        public void LoadAdequateAssuranceInvestigators()
        {
            _adequateAssuranceListSiteData = new AdequateAssuranceListSiteData();

            int RowCount = 1;
            foreach(IWebElement TR in 
                AdequateAssuranceListTable.FindElements(By.XPath("//tbody/tr")))
            {
                var AdequateAssuranceInvestigator = new AdequateAssuranceList();

                IList<IWebElement> TDs = TR.FindElements(By.XPath("td"));
                if (TDs.Count > 0)
                {
                    AdequateAssuranceInvestigator.RowNumber = RowCount;
                    AdequateAssuranceInvestigator.NameAndAddress = TDs[0].Text;
                    AdequateAssuranceInvestigator.Center = TDs[1].Text;
                    AdequateAssuranceInvestigator.Type = TDs[2].Text;
                    AdequateAssuranceInvestigator.ActionDate = TDs[3].Text;
                    AdequateAssuranceInvestigator.Comments = TDs[4].Text;

                    _adequateAssuranceListSiteData.AdequateAssurances.Add
                        (AdequateAssuranceInvestigator);
                    RowCount = RowCount + 1;
                }
            }
        }

        public override void SaveData() {
            _UOW.AdequateAssuranceListRepository.
                Add(_adequateAssuranceListSiteData);
        }
    }
}
