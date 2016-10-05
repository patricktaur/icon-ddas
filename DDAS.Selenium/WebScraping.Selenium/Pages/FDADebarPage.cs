using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using WebScraping.Selenium.BaseClasses;
using DDAS.Models.Enums;
using DDAS.Models;
using DDAS.Models.Entities.Domain.SiteData;

namespace WebScraping.Selenium.Pages
{
    public partial class FDADebarPage : BaseSearchPage
    {

        IUnitOfWork _UOW;
        public FDADebarPage(IWebDriver driver, IUnitOfWork uow) : base(driver)
        {
            Open();
            _UOW = uow;
            //SaveScreenShot("abc.png");
        }

        public override string Url
        {
            get
            {
                return @"http://www.fda.gov/ora/compliance_ref/debar/default.htm";
            }
        }

        private List<DebarredPerson> _DebarredPersonList;
        public List<DebarredPerson> DebarredPersons
        {
            get
            {
                return _DebarredPersonList;
            }
        }

        public void LoadDebarredPersonList()
        {
            _DebarredPersonList = new List<DebarredPerson>();

            foreach (IWebElement TR in PersonsTable.FindElements(By.XPath("tbody/tr")))
            {
                var debarredPerson = new DebarredPerson();
                IList<IWebElement> TDs = TR.FindElements(By.XPath("td"));

                debarredPerson.NameOfPerson = TDs[0].Text;
                debarredPerson.EffectiveDate =TDs[1].Text;
                debarredPerson.EndOfTermOfDebarment = TDs[2].Text;
                debarredPerson.FrDateText = TDs[3].Text;
                debarredPerson.VolumePage = TDs[4].Text;

                if (IsElementPresent(TDs[4], By.XPath("a")))
                {
                    IWebElement anchor = TDs[4].FindElement(By.XPath("a"));
                    debarredPerson.DocumentLink = anchor.GetAttribute("href");
                }

                _DebarredPersonList.Add(debarredPerson);
            }
        }

        private FDADebarPageSiteData _FDADebarPageSiteData;

        public void LoadDebarredPersonListAlt()
        {
            _FDADebarPageSiteData = new FDADebarPageSiteData();

            _FDADebarPageSiteData.CreatedBy = "pat";
            _FDADebarPageSiteData.SiteLastUpdatedOn = DateTime.Now;
            _FDADebarPageSiteData.CreatedOn = DateTime.Now;

            int RowCount = 1;
            foreach (IWebElement TR in PersonsTable.FindElements(By.XPath("tbody/tr")))
            {
                var debarredPerson = new DebarredPerson();

                IList<IWebElement> TDs = TR.FindElements(By.XPath("td"));
                debarredPerson.RowNumber = RowCount;
                debarredPerson.NameOfPerson = TDs[0].Text;
                debarredPerson.EffectiveDate = TDs[1].Text;
                debarredPerson.EndOfTermOfDebarment = TDs[2].Text;
                debarredPerson.FrDateText = TDs[3].Text;
                debarredPerson.VolumePage = TDs[4].Text;

                if (IsElementPresent(TDs[4], By.XPath("a")))
                {
                    IWebElement anchor = TDs[4].FindElement(By.XPath("a"));
                    debarredPerson.DocumentLink = anchor.GetAttribute("href");
                }

                _FDADebarPageSiteData.DebarredPersons.Add(debarredPerson);
                RowCount = RowCount + 1;
            }
        }

        public override SiteEnum SiteName {
            get {
                return SiteEnum.FDADebarPage;
            }
        }

        public override void LoadContent()
        {
            //LoadDebarredPersonList();
            LoadDebarredPersonListAlt();
        }

        public override void LoadContent(string NameToSearch)
        {
            LoadDebarredPersonListAlt();
        }
        public override void SaveData()
        {
            _UOW.FDADebarPageRepository.Add(_FDADebarPageSiteData);
        }
    }
}
