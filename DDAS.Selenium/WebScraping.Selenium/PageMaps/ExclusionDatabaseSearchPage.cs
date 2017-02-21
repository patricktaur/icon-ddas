using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using WebScraping.Selenium.BaseClasses;

namespace WebScraping.Selenium.Pages
{
    public partial class ExclusionDatabaseSearchPage : BaseSearchPage
    {
        public IWebElement ExclusionDatabaseSearchFirstName
        {
            get {
                IWebElement FirstName = driver.FindElement(By.Id("ctl00_cpExclusions_txtSPFirstName"));
                return FirstName;
            }
        }

        public IWebElement ExclusionDatabaseSearchLastName
        {
            get {
                IWebElement LastName = driver.FindElement(By.Id("ctl00_cpExclusions_txtSPLastName"));
                return LastName;
            }
        }

        public IWebElement ExclusionDatabaseSearchElement
        {
            get
            {
                IWebElement Search = driver.FindElement(By.Id("ctl00_cpExclusions_ibSearchSP"));
                return Search;
            }
        }

        public IWebElement ExclusionDatabaseSearchTable
        {
            get
            {
                try
                {
                    IWebElement Table = driver.FindElement(By.Id("ctl00_cpExclusions_gvEmployees"));
                    return Table;
                }
                catch (NoSuchElementException)
                {
                    return null;
                }
            }
        }

        public IWebElement ExclusionDatabaseSearchAgain
        {
            get
            {
                IWebElement SearchAgain = driver.FindElement(
                    By.Id("ctl00_cpExclusions_lbGridBackToSearch"));
                return SearchAgain;
            }
        }

        public IWebElement ExclusionDatabaseAnchorToDownloadCSV
        {
            get
            {
                IList<IWebElement> Anchors = driver.FindElements(By.XPath("//a"));

                foreach(IWebElement Anchor in Anchors)
                {
                    if (Anchor.Text.ToLower().Contains(
                        "updated leie database"))
                        return Anchor;
                }
                throw new Exception("Could not download LEIE database file!");
            }
        }

        public IWebElement PageLastUpdatedTextElement
        {
            get
            {
                try
                {
                    IList<IWebElement> Elements = driver.FindElements(By.XPath("//h2"));
                    return Elements[1];
                }
                catch (Exception)
                {
                    throw new Exception("Unable to find PageLastUpdatedElement");
                }
            }
        }
    }
}
