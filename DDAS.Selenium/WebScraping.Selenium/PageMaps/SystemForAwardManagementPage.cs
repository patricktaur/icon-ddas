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
                throw new Exception("Could not find SAMAchorTag");
            }
        }

        public IWebElement SAMInputTag {
            get {
                try
                {
                    IList<IWebElement> InputTags = driver.FindElements(By.Id("q"));
                    return InputTags[0];
                }
                catch(Exception)
                {
                    throw new Exception("Could not find SAMInputTag");
                }
            }
        }

        public IWebElement SAMSubmitButton {
            get {
                try
                {
                    IWebElement Submit = driver.FindElement(By.Id("RegSearchButton"));
                    return Submit;
                }
                catch(Exception)
                {
                    throw new Exception("Could not find SAMSubmitButton");
                }
            }
        }

        public IWebElement SAMCheckResult {
            get {
                try
                {
                    //IList<IWebElement> CheckHeader4 = driver.FindElements(By.XPath("//h4"));

                    //foreach (IWebElement Element in CheckHeader4)
                    //{
                    //    if (Element.Text.ToLower().Contains(
                    //        "no records found for current search"))
                    //        return null;
                    //    else if (Element.Text.ToLower().Contains(
                    //        "returned the following results"))
                    //        return Element;
                    //}

                    IList<IWebElement> CheckHeader3 = driver.FindElements(By.XPath("//h3"));

                    foreach (IWebElement Element in CheckHeader3)
                    {
                        if (Element.Text.ToLower().Contains(
                            "total records"))
                            return Element;
                    }
                    throw new Exception("Could not find 'Total Records' header");
                }
                catch (Exception)
                {
                    throw new Exception("Could not find 'Total Records' header, Selenium/PageMaps");
                }
            }
        }

        public IWebElement SAMSearchResult {
            get {
                try
                {
                    IWebElement element = driver.FindElement(By.Id("search_results"));
                    return element;
                }
                catch(Exception)
                {
                    throw new Exception("Could not find Element 'search_results'");
                }
            }
        }

        public IWebElement SAMClearSearch {
            get {
                try
                {
                    IList<IWebElement> ClearSearchElement =
                        driver.FindElements(By.CssSelector("input[type='submit']"));
                    return ClearSearchElement[0];
                }
                catch(Exception)
                {
                    throw new Exception("Could not find SAMClearSearch Element");
                }
            }
        }

        public IWebElement SAMPaginationElement
        {
            get
            {
                IList<IWebElement> PaginationElement = 
                    driver.FindElements(By.ClassName("pagination"));

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
                try
                {
                    string XPathValue = "//div[@id='foot']/div/p";
                    IList<IWebElement> Elements = driver.FindElements(By.XPath(XPathValue));
                    return Elements[1];
                }
                catch(Exception)
                {
                    throw new Exception("Could not find PageLastUpdatedTextElement");
                }
            }
        }
    }
}
