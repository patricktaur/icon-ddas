using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebScraping.Selenium.BaseClasses;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using DDAS.Models;
using DDAS.Models.Entities.Domain.SiteData;

namespace WebScraping.Selenium.Pages
{
   public partial class FDADebarPage : BaseSearchPage //BaseClasses.BasePage
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

           

            foreach (IWebElement TR in PersonsTable.FindElements(By.XPath("tbody/tr")))
            {
                var debarredPerson = new DebarredPerson(); //new DebarredPerson();

                IList<IWebElement> TDs = TR.FindElements(By.XPath("td"));

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

                //_DebarredPersonList.Add(debarredPerson);
            }
        }

        public override SiteEnum SiteName {
            get {
                return SiteEnum.FDADebarPage;
            }
        }

        public override ResultAtSite GetResultAtSite(string NameToSearch)
        {
            ResultAtSite searchResult = new ResultAtSite();

            searchResult.SiteName = SiteName.ToString();

            foreach (DebarredPerson person in _DebarredPersonList)
            {
                string WordFound = FindSubString(person.NameOfPerson, NameToSearch);

                if (WordFound != null)
                {
                    searchResult.SiteName = SiteName.ToString();

                    searchResult.Results.Add(new MatchResult
                    {
                        MatchName = person.NameOfPerson,
                        MatchLocation = "Word(s) matched - " + WordFound
                    });
                }
            }

            if (searchResult.Results.Count == 0)
            {
                searchResult.Results.Add(new MatchResult {
                MatchName = "None",
                MatchLocation = "None"});
                return searchResult;
            }
            else
                return searchResult;
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


        //public class DebarredPerson
        //{
        //    public string NameOfPerson { get; set; }
        //    public string EffectiveDate { get; set; }
        //    public string EndOfTermOfDebarment { get; set; }
        //    public string FrDateText { get; set; }
        //    public string VolumePage { get; set; }
        //    public string DocumentLink { get; set; }
        //}

    }
}
