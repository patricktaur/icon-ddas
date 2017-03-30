using System;
using System.Collections.Generic;
using DDAS.Models.Enums;
using OpenQA.Selenium;
using WebScraping.Selenium.BaseClasses;
using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models;
using System.Linq;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Interfaces;

namespace WebScraping.Selenium.Pages
{
    public partial class CorporateIntegrityAgreementsListPage : BaseSearchPage
    {
        private IUnitOfWork _UOW;
        private IConfig _config;
        private DateTime? _SiteLastUpdatedFromPage;

        public CorporateIntegrityAgreementsListPage(IWebDriver driver, IUnitOfWork uow,
            IConfig Config)
            : base(driver)
        {
            _UOW = uow;
            _config = Config;
            Open();
            _CIASiteData = new CorporateIntegrityAgreementListSiteData();
            _CIASiteData.RecId = Guid.NewGuid();
            _CIASiteData.ReferenceId = _CIASiteData.RecId;
            _CIASiteData.Source = driver.Url;
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

        public override IEnumerable<SiteDataItemBase> SiteData
        {
            get
            {
                return _CIASiteData.CIAListSiteData;
            }
        }

        public override DateTime? SiteLastUpdatedDateFromPage
        {
            get
            {
                if (_SiteLastUpdatedFromPage == null)
                    ReadSiteLastUpdatedDateFromPage();
                return _SiteLastUpdatedFromPage;
            }
        }

        public override BaseSiteData baseSiteData
        {
            get
            {
                return _CIASiteData;
            }
        }

        private CorporateIntegrityAgreementListSiteData _CIASiteData;

        private void LoadCIAList()
        {            
            IList<IWebElement> TRs = CIAListTable.FindElements(By.XPath("//tbody/tr"));

            int RowCount = 1;
            for (int TableRow = 9; TableRow < TRs.Count; TableRow++)
            {
                var CiaList = new CIAList();

                IList<IWebElement> TDs = TRs[TableRow].FindElements(By.XPath("td"));

                if(TDs.Count > 0)
                {
                    CiaList.RowNumber = RowCount;
                    CiaList.Provider = TDs[0].Text;
                    CiaList.City = TDs[1].Text;
                    CiaList.State = TDs[2].Text;
                    CiaList.Effective = TDs[3].Text;

                    //if(IsElementPresent(TDs[0], By.XPath("a")))
                    //{
                    //    IWebElement anchor = driver.FindElement(By.XPath("a"));
                    //    var link = new Link();
                    //    link.Title = "Provider";
                    //    link.url = anchor.GetAttribute("href");
                    //    CiaList.Links.Add(link);
                    //}

                    _CIASiteData.CIAListSiteData.Add(CiaList);
                    RowCount = RowCount + 1;
                }
            }
        }

        public override void LoadContent()
        {
            try
            {
                _CIASiteData.DataExtractionRequired = true;
                LoadCIAList();
                _CIASiteData.DataExtractionSucceeded = true;
            }
            catch (Exception e)
            {
                _CIASiteData.DataExtractionSucceeded = false;
                _CIASiteData.DataExtractionErrorMessage = e.Message;
                _CIASiteData.ReferenceId = null;
                throw new Exception(e.ToString());
            }
            finally
            {
                _CIASiteData.CreatedOn = DateTime.Now;
            }
        }

        public override void LoadContent(string NameToSearch, int MatchCountLowerLimit)
        {
            throw new NotImplementedException();
        }

        public void ReadSiteLastUpdatedDateFromPage()
        {
            string PageLastUpdated =
                PageLastUpdatedElement.Text.Replace("Updated ", "").
                Replace("-", "/").Trim();

            DateTime RecentLastUpdatedDate;

            DateTime.TryParseExact(PageLastUpdated, "M'/'d'/'yyyy", null,
                System.Globalization.DateTimeStyles.None, out RecentLastUpdatedDate);

            _SiteLastUpdatedFromPage = RecentLastUpdatedDate;
        }

        private void AssignReferenceIdOfPreviousDocument()
        {
            var SiteData = _UOW.CorporateIntegrityAgreementRepository.GetAll().
                OrderByDescending(t => t.CreatedOn).First();

            _CIASiteData.ReferenceId = SiteData.RecId;
        }

        public override void SaveData()
        {
            _UOW.CorporateIntegrityAgreementRepository.Add(_CIASiteData);
        }
    }
}
