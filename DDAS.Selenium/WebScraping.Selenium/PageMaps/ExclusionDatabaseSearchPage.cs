﻿using OpenQA.Selenium;
using System;
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
                IWebElement FirstName = driver.FindElement(By.Id("ctl00_cpExclusions_txtSPFirstName"));
                return FirstName;
            }
        }

        public IWebElement ExclusionDatabaseSearchLastName
        {
            get
            {
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
    }
}
