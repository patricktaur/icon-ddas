using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using OpenQA.Selenium;
using WebScraping.Selenium.BaseClasses;
using OpenQA.Selenium.Support.UI;

namespace WebScraping.Selenium.Pages
{
    public partial class FDAWarningLettersPage : BaseSearchPage
    {
        public IWebElement FDASearchTextBox
        {
            get
            {
                try
                {
                    //driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(20));

                    //IWebElement Element = driver.FindElement(By.Id("qryStr"));
                    //return Element;

                    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                    Func<IWebDriver, IWebElement> waitForElement =
                        new Func<IWebDriver, IWebElement>((IWebDriver Web) =>
                        {
                            IWebElement element = Web.FindElement(By.Id("qryStr"));
                            return element;
                        });
                    IWebElement targetElement = wait.Until(waitForElement);
                    return targetElement;
                }
                catch(Exception)
                {
                    throw new Exception("Could not find FDASearchTextBox");
                }
            }
        }
        public IWebElement FDASearchButton
        {
            get
            {
                //IList<IWebElement> SearchButtons =
                //    driver.FindElements(By.TagName("input"));

                //foreach (IWebElement element in SearchButtons)
                //{
                //    if (element.GetAttribute("value").ToLower() == "search")
                //        return element;
                //}
                //throw new Exception("Could not click on Search Button");


                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                Func<IWebDriver, IWebElement> waitForElement =
                    new Func<IWebDriver, IWebElement>((IWebDriver Web) =>
                    {
                        IList<IWebElement> Elements = Web.FindElements(By.TagName("input"));

                        foreach(IWebElement element in Elements)
                        {
                            if (element.GetAttribute("value").ToLower() =="search")
                            {
                                return element;
                            }
                        }
                        throw new Exception("Could not find 'Search' button");
                    });
                IWebElement targetElement = wait.Until(waitForElement);
                return targetElement;
            }
        }

        public IWebElement FDASortTable
        {
            get
            {
                //driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(30));

                IList<IWebElement> Table = driver.FindElements(By.XPath("//table"));

                IWebElement SortTable = driver.FindElement(By.Id("fd-table-2"));

                return SortTable; //Table[7];
            }
        }

        private IWebElement SortTableTest1
        {
            get
            {
                try
                {
                    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                    Func<IWebDriver, IWebElement> waitForElement =
                        new Func<IWebDriver, IWebElement>((IWebDriver Web) =>
                        {
                            IWebElement element = Web.FindElement(By.Id("fd-table-2"));
                            return element;
                        });
                    IWebElement targetElement = wait.Until(waitForElement);
                    return targetElement;
                }
                catch
                {
                    throw new Exception("Unable to find table with id 'fd-table-2'");
                }

            }
        }


        public IWebElement PageLastUpdatedTextElement
        {
            get
            {
                try
                {
                    IWebElement Element = driver.FindElement(By.Id("pagetools_right"));
                    return Element;
                }
                catch(Exception)
                {
                    throw new Exception("Could not find PageLastUpdatedElement");
                }
            }
        }
    }
}
