using OpenQA.Selenium;
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
                driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(10));
                try {
                    IWebElement Result = driver.FindElement(By.Id("its_docs"));
                    return Result;
                }
                catch(Exception)
                {
                    throw new Exception("Unable to get count of search result!");
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
                return null;
            }
        }
    }
}
