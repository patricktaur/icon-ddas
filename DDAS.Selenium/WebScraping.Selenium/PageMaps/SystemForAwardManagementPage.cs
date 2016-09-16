﻿using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebScraping.Selenium.BaseClasses;

namespace WebScraping.Selenium.Pages
{
    public partial class SystemForAwardManagementPage: BaseSearchPage
    {
        public IWebElement SAMAnchorTag {
            get {
                IList<IWebElement> Anchors = driver.FindElements(By.XPath("//form/a"));
                return Anchors[2];
            }
        }

        public IWebElement SAMInputTag {
            get {
                IList<IWebElement> InputTags = driver.FindElements(By.Id("q"));
                return InputTags[0];
            }
        }

        public IWebElement SAMSubmitButton {
            get {
                IWebElement Submit = driver.FindElement(By.Id("RegSearchButton"));
                return Submit;
            }
        }

        public IWebElement SAMCheckResult {
            get {
                try {
                    IWebElement Result = driver.FindElement(By.Id("its_docs"));
                    return Result;
                }
                catch (NoSuchElementException) {
                    return null;
                }
            }
        }

        public IWebElement SAMSearchResult {
            get {
                IWebElement element = driver.FindElement(By.Id("search_results"));
                return element;
            }
        }

        public IWebElement SAMClearSearch {
            get {
                IList<IWebElement> ClearSearchElement = 
                    driver.FindElements(By.CssSelector("input[type='submit']"));

                foreach (IWebElement element in ClearSearchElement)
                {
                    if (element.GetAttribute("title").ToLower() == "clear search")
                    {
                        return element;
                    }
                }
                return null;
            }
        }
    }
}
