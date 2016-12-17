using DDAS.Models.Enums;
using DDAS.Models.Interfaces;
using OpenQA.Selenium;
using System.Collections.Generic;
using DDAS.Models.Entities.Domain.SiteData;
using System;
using DDAS.Models.Entities.Domain;

namespace WebScraping.Selenium.BaseClasses
{
    public abstract class BaseSearchPage: BasePage, ISearchPage
    {
        public BaseSearchPage(IWebDriver driver) : base(driver)
        {
        }

        public abstract SiteEnum SiteName { get; }
        
        //public abstract void LoadContent();
        public abstract void LoadContent(string NameToSearch, string DownloadFolder);

        public virtual void SavePageImage() {
        SaveScreenShot("XXXXX");
        }

        public abstract void SaveData();

        public abstract DateTime? SiteLastUpdatedDateFromPage { get; }

        public abstract DateTime? SiteLastUpdatedDateFromDatabase { get; }

        public abstract IEnumerable<SiteDataItemBase> SiteData { get; }

        public abstract BaseSiteData baseSiteData { get; }
    }
} 
