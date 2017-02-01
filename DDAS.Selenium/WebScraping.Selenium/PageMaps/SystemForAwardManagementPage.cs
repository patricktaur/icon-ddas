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
                driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(10));

                IList<IWebElement> Anchors = driver.FindElements(By.XPath("//form/a"));
                
                foreach(IWebElement Anchor in Anchors)
                {
                    if (Anchor.GetAttribute("title").ToLower() == "search records")
                        return Anchor;
                }
                return null;
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
                try
                {
                    IList<IWebElement> Result = driver.FindElements(By.XPath("//h4"));

                    foreach(IWebElement Element in Result)
                    {
                        if (Element.Text.ToLower().Contains(
                            "no records found for current search"))
                            return Element;
                        else if (Element.Text.ToLower().Contains(
                            "returned the following results"))
                            return Element;
                    }
                    throw new Exception("Unable to check search results");
                }
                catch (Exception)
                {
                    throw new Exception("Unable to check search results");
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
                return ClearSearchElement[0];
            }
        }

        public IWebElement SAMPaginationElement
        {
            get
            {
                IList<IWebElement> PaginationElement = driver.FindElements(By.ClassName("pagination"));

                foreach(IWebElement element in PaginationElement)
                {
                    if(element.TagName.ToLower() == "div")
                    {
                        return element;
                    }
                }
                throw new Exception("Pagination element not found");
            }
        }

        public IWebElement PageLastUpdatedTextElement
        {
            get
            {
                string XPathValue = "//div[@id='foot']/div/p";
                IList<IWebElement> Elements = driver.FindElements(By.XPath(XPathValue));
                return Elements[1];
            }
        }
    }
}
