using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
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

        public bool SAMCheckResult {
            get {
                try
                {

                    return
                    driver.PageSource.ToLower().Contains("total records: 0") ? true : false;

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
                    IWebElement element = driver.FindElement(By.Id("its_docs"));
                    return element;
                }
                catch(Exception)
                {
                    throw new Exception("Could not find Element 'its_docs'");
                }
            }
        }

        private IWebElement Test
        {
            get
            {
                 WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                Func<IWebDriver, bool> waitForElement = new Func<IWebDriver, bool>((IWebDriver Web) =>
                {
                    IWebElement element = Web.FindElement(By.Id("target"));
                    if (element.GetAttribute("style").Contains("red"))
                    {
                        return true;
                    }
                    return false;
                });
                wait.Until(waitForElement);
                return null;
            }
        }


        private string Test1{
            get {
               
        {
                  WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromMinutes(1));
                    Func<IWebDriver, IWebElement> waitForElement = new Func<IWebDriver, IWebElement>((IWebDriver Web) =>
                    {
                        Console.WriteLine("Waiting for color to change");
                        IWebElement element = Web.FindElement(By.Id("target"));
                        if (element.GetAttribute("style").Contains("red"))
                        {
                            return element;
                        }
                        return null;
                    });

                    IWebElement targetElement = wait.Until(waitForElement);
                    Console.WriteLine("Inner HTML of element is " + targetElement.GetAttribute("innerHTML"));
                    return null;
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
