using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using DDAS.Models.Interfaces;
using OpenQA.Selenium;
using System.Collections.Generic;
using System;
using DDAS.Models.Repository;
using DDAS.Models;

namespace WebScraping.Selenium.BaseClasses
{
    public abstract class BaseSearchPage:BasePage, ISearchPage
    {
        
        public BaseSearchPage(IWebDriver driver) : base(driver)
        {
           
        }

        public abstract SiteEnum SiteName { get;  }
        //public abstract ResultAtSite Search(string NameToSearch);
        public abstract void LoadContent();
        public abstract void LoadContent(string NameToSearch);
        //public abstract ResultAtSite GetResultAtSite(string NameToSearch);

        public virtual void SavePageImage() {
        SaveScreenShot("XXXXX");
        }

        public string FindSubString(string SearchString, string NameToSearch)
        {
            SearchString = SearchString.ToLower();

            string[] FullName = NameToSearch.Trim().Split(' ');

            int count = 0;
            string WordMatched = null;

            for (int i = 0; i < FullName.Length; i++)
            {
                if (SearchString.Contains(FullName[i].ToLower()))
                {
                    count = count + 1;
                    WordMatched = WordMatched + " " + FullName[i].Trim();
                }
            }
            return WordMatched;
        }

        public abstract void SaveData();
    }
} 
