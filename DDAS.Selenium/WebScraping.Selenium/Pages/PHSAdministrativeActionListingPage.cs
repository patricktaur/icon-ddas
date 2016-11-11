using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using OpenQA.Selenium;
using WebScraping.Selenium.BaseClasses;
using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models;

namespace WebScraping.Selenium.Pages
{
    public partial class PHSAdministrativeActionListingPage : BaseSearchPage
    {
        private IUnitOfWork _UOW;

        public PHSAdministrativeActionListingPage(IWebDriver driver, IUnitOfWork uow)
            : base(driver)
        {
            Open();
            _UOW = uow;
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

        private PHSAdministrativeActionListingSiteData _PHSAdministrativeSiteData;

        public void LoadAdministrativeActionList()
        {
            _PHSAdministrativeSiteData = new PHSAdministrativeActionListingSiteData();

            _PHSAdministrativeSiteData.CreatedBy = "Patrick";
            _PHSAdministrativeSiteData.SiteLastUpdatedOn = DateTime.Now;
            _PHSAdministrativeSiteData.CreatedOn = DateTime.Now;
            _PHSAdministrativeSiteData.Source = driver.Url;

            IList<IWebElement> TRs = PHSTable.FindElements(By.XPath("//tbody/tr"));

            int RowCount = 1;
            foreach (IWebElement TR in TRs)
            {
                var AdministrativeActionListing = new PHSAdministrativeAction();

                IList<IWebElement> TDs = TR.FindElements(By.XPath("td"));

                if (TDs.Count > 0)
                {
                    AdministrativeActionListing.RowNumber = RowCount;
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

                    _PHSAdministrativeSiteData.PHSAdministrativeSiteData.Add
                        (AdministrativeActionListing);
                    RowCount = RowCount + 1;
                }
            }
        }

        public override void LoadContent(string NameToSearch)
        {
            LoadAdministrativeActionList();
        }

        public override void SaveData()
        {
            _UOW.PHSAdministrativeActionListingRepository.
                Add(_PHSAdministrativeSiteData);
        }
    }
}
