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
            get
            {
                driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
                
                IList<IWebElement> FirstName = driver.FindElements(By.XPath("//input[@type='text']"));

                foreach (IWebElement element in FirstName)
                {
                    if (element.GetAttribute("Id") == "ctl00_cpExclusions_txtSPFirstName")
                    {
                        return element;
                    }
                }
                return null;
            }
        }

        public IWebElement ExclusionDatabaseSearchLastName
        {
            get
            {
                IList<IWebElement> LastName = driver.FindElements(By.XPath("//input"));

                foreach (IWebElement element in LastName)
                {
                    if (element.GetAttribute("Id") == "ctl00_cpExclusions_txtSPLastName")
                    {
                        return element;
                    }
                }
                return null;
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
    }
}
