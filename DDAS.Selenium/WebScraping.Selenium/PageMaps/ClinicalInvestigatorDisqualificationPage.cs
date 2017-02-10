using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebScraping.Selenium.BaseClasses;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using OpenQA.Selenium.Support.UI;

namespace WebScraping.Selenium.Pages
{
    public partial class ClinicalInvestigatorDisqualificationPage: BaseSearchPage //BaseClasses.BasePage
    {

        public IWebElement DisqualifiedInvestigatorSearchTextBox
        {
            get
            {
                try
                {
                    //driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
                    //IWebElement SearchTextBox = driver.FindElement(By.Id("filter"));
                    //return SearchTextBox;

                    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                    Func<IWebDriver, IWebElement> waitForElement = 
                        new Func<IWebDriver, IWebElement>((IWebDriver Web) =>
                    {
                        IWebElement element = Web.FindElement(By.Id("filter"));
                        return element;
                    });
                    IWebElement targetElement = wait.Until(waitForElement);
                    return targetElement;
                }
                catch(Exception)
                {
                    throw new Exception(
                        "Could not find DisqualifiedInvestigatorSearchTextBox");
                }
            }
        }

        public IWebElement DisqualifiedInvestigatorSubmitButton
        {
            get
            {
                //IList<IWebElement> SubmitButton = 
                //    driver.FindElements(By.CssSelector("input[type='submit']"));

                //foreach(IWebElement Button in SubmitButton)
                //{
                //    if (Button.GetAttribute("value").ToLower() == "show items")
                //        return Button;
                //}
                //throw new Exception("Could not find Submit button!");


                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                Func<IWebDriver, IWebElement> waitForElement =
                    new Func<IWebDriver, IWebElement>((IWebDriver Web) =>
                    {
                        IList<IWebElement> SubmitButton =
                        driver.FindElements(By.CssSelector("input[type='submit']"));
                        
                        foreach(IWebElement Button in SubmitButton)
                        {
                            if (Button.GetAttribute("value").ToLower() == "show items")
                                return Button;
                        }
                        throw new Exception("Could not find button: " +
                            "DisqualifiedInvestigatorSubmitButton in PageMaps");
                    });
                IWebElement targetElement = wait.Until(waitForElement);
                return targetElement;
            }
        }

        public IWebElement DisqualifiedInvestigatorCountTable
        {
            get
            {
                try
                {
                    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                    Func<IWebDriver, IWebElement> waitForElement =
                        new Func<IWebDriver, IWebElement>((IWebDriver Web) =>
                        {
                            IList<IWebElement> Tables = Web.FindElements(By.XPath("//table"));
                            return Tables[2];
                        });
                    IWebElement targetElement = wait.Until(waitForElement);
                    return targetElement;
                }
                catch(Exception ex)
                {
                    throw new Exception(
                        "Could not find Table with search count. Error Message: " +
                        ex.Message);
                }
            }
        }

        public IWebElement DisqualifiedInvestigatorTable
        {
            get
            {
                try
                {
                    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                    Func<IWebDriver, IWebElement> waitForElement =
                        new Func<IWebDriver, IWebElement>((IWebDriver Web) =>
                        {
                            IList<IWebElement> Tables = Web.FindElements(By.XPath("//table"));
                            return Tables[3];
                        });
                    IWebElement targetElement = wait.Until(waitForElement);
                    return targetElement;
                }
                catch (Exception ex)
                {
                    throw new Exception(
                        "Could not find Table with search count. Error Message: " +
                        ex.Message);
                }
            }
        }

        public IWebElement PageLastUpdatedTextElement
        {
            get
            {
                try
                {
                    //driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(20));
                    //IWebElement Element = driver.FindElement(By.Id("pagetools_right"));
                    //return Element;

                    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                    Func<IWebDriver, IWebElement> waitForElement =
                        new Func<IWebDriver, IWebElement>((IWebDriver Web) =>
                        {
                            IWebElement Element = 
                            Web.FindElement(By.Id("pagetools_right"));
                            return Element;
                        });
                    IWebElement targetElement = wait.Until(waitForElement);
                    return targetElement;
                }
                catch (Exception ex)
                {
                    throw new Exception(
                        "Unable to find PageLastUpdatedTextElement. Error Message: " +
                        ex.Message);
                }
            }
        }
    }
}
