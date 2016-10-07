﻿using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebScraping.Selenium.BaseClasses;

namespace WebScraping.Selenium.Pages
{
    public partial class PHSAdministrativeActionListingPage : BaseSearchPage
    {
        public IWebElement PHSTable {
            get {
                IList<IWebElement> table = driver.FindElements(By.XPath("//table"));
                return table[0];
            }
        }
    }
}