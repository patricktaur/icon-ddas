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
       
		public string CheckForFeedbackWindow
        {
            get
            {
                try
                {
                    string currentHandle = driver.CurrentWindowHandle;
                    ReadOnlyCollection<string> originalHandles = driver.WindowHandles;

                    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                    string popupWindowHandle = wait.Until((d) =>
                    {
                        string foundHandle = null;

                        // Subtract out the list of known handles. In the case of a single
                        // popup, the newHandles list will only have one value.
                        List<string> newHandles = 
                        driver.WindowHandles.Except(originalHandles).ToList();

                        if (newHandles.Count > 0)
                        {
                            SaveScreenShot(
                                @"c:\Development\p926-ddas\documents\technical\images\" +
                                "FDAWarningsLetter_" + 
                                 DateTime.Now.ToString("dd MMM yyyy hh_mm") 
                                 + ".png");

                            foundHandle = newHandles[0];
                        }
                        return foundHandle;
                    });

                    if (popupWindowHandle == null)
                        return null;
                    else
                    {
                        driver.SwitchTo().Window(popupWindowHandle);

                        // Do whatever you need to on the popup browser, then...
                        driver.Close();
                        driver.SwitchTo().Window(currentHandle);

                        SaveScreenShot(
                            @"c:\Development\p926-ddas\documents\technical\images\" +
                            "FDAWarningsLetter_"
                            + DateTime.Now.ToString("dd MMM yyyy hh_mm")
                            + ".png");

                        return null;
                    }
                }
                catch
                {
                    return null;
                }
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
