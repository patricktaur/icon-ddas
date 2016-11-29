using System;
using System.Collections.Generic;
using DDAS.Models.Enums;
using OpenQA.Selenium;
using WebScraping.Selenium.BaseClasses;
using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models;
using System.Linq;

namespace WebScraping.Selenium.Pages
{
    public partial class CorporateIntegrityAgreementsListPage : BaseSearchPage
    {
        private IUnitOfWork _UOW;

        public CorporateIntegrityAgreementsListPage(IWebDriver driver, IUnitOfWork uow)
            : base(driver)
        {
            _UOW = uow;
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

        private CorporateIntegrityAgreementListSiteData _CIASiteData;

        public void LoadCIAList()
        {
            _CIASiteData = new CorporateIntegrityAgreementListSiteData();

            _CIASiteData.CreatedBy = "patrick";
            _CIASiteData.SiteLastUpdatedOn = DateTime.Now;
            _CIASiteData.CreatedOn = DateTime.Now;
            _CIASiteData.Source = driver.Url;

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

                    _CIASiteData.CIAListSiteData.Add(CiaList);
                    RowCount = RowCount + 1;
                }
            }
        }

        public override void LoadContent(string NameToSearch, string DownloadFolder)
        {
            //refactor - add code to validate ExtractionDate
            try
            {
                if (_CIASiteData.DataExtractionRequired)
                {
                    LoadCIAList();
                    _CIASiteData.DataExtractionSucceeded = true;
                }
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
                if (!_CIASiteData.DataExtractionRequired)
                    AssignReferenceIdOfPreviousDocument();
            }
        }

        public void AssignReferenceIdOfPreviousDocument()
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
