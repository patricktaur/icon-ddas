using System;
using System.Collections.Generic;
using DDAS.Models.Enums;
using OpenQA.Selenium;
using WebScraping.Selenium.BaseClasses;
using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models;

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


        private List<CIAList> _ciaList;

        public List<CIAList> CorporateIntegrityAgreementList {
            get {
                return _ciaList;
            }
        }

        private CorporateIntegrityAgreementsListSiteData _CIASiteData;

        public void LoadCIAList()
        {
            _CIASiteData = new CorporateIntegrityAgreementsListSiteData();

            _CIASiteData.CreatedBy = "patrick";
            _CIASiteData.SiteLastUpdatedOn = DateTime.Now;

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

                    _CIASiteData.CIAListSiteData.Add(CiaList);
                }
            }
        }

        public override void LoadContent()
        {
            LoadCIAList();
        }
        public override void LoadContent(string NameToSearch)
        {

        }

        public override void SaveData()
        {

        }
    }
}
