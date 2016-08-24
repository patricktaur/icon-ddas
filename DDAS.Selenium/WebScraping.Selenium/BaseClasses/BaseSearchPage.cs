using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using OpenQA.Selenium;
using System.Collections.Generic;

namespace WebScraping.Selenium.BaseClasses
{
    public abstract class BaseSearchPage:BasePage
    {
        public BaseSearchPage(IWebDriver driver) : base(driver)
        {
        }

        public abstract SiteEnum SiteName { get;  }
        public abstract ResultAtSite Search(string NameToSearch);
        public abstract void LoadContent();
    }
} 
