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
using System.Collections.ObjectModel;

namespace WebScraping.Selenium.Pages
{
    public partial class FDAWarningLettersPage : BaseSearchPage
    {
        public bool IsFeedbackPopUpDisplayed
        {
            get
            {
                if (driver.PageSource.ToLower().Contains("give feeback"))
                    return true;
                else
                    return false;
            }
        } 

		public IWebElement FDASearchTextBox
        {
            get
            {
                try
                {
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
                 WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                Func<IWebDriver, IWebElement> waitForElement =
                    new Func<IWebDriver, IWebElement>((IWebDriver Web) =>
                    {
                        IList<IWebElement> Elements = Web.FindElements(By.TagName("input"));

                        if (Elements.Count == 0)
                            return null;
                        foreach(IWebElement element in Elements)
                        {
                            if (element.GetAttribute("value").ToLower() =="search")
                            {
                                return element;
                            }
                        }
                        return null;
                    });
                IWebElement targetElement = wait.Until(waitForElement);
                return targetElement;
            }
        }

        private IWebElement FDAWarningSortTable
        {
            get
            {
                try
                {
                    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                    Func<IWebDriver, IWebElement> waitForElement =
                        new Func<IWebDriver, IWebElement>((IWebDriver Web) =>
                        {
                            IWebElement element = Web.FindElement(By.Id("fd-table-2"));
                            return element;
                        });
                    IWebElement targetElement = wait.Until(waitForElement);
                    return targetElement;
                }
                catch (Exception)
                {
                    throw new Exception("Unable to find table with id 'fd-table-2'");
                }
            }
        }

        public bool IsSiteDown
        {
            get
            {
                if (driver.PageSource.ToLower().Contains("unexpected error"))
                {
                    if (driver.PageSource.ToLower().Contains("contact the website administrator"))
                        return true;
                    else
                        return false;
                }
                else
                    return false;
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
