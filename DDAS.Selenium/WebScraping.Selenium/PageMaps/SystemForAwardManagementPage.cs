﻿using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using WebScraping.Selenium.BaseClasses;

namespace WebScraping.Selenium.Pages
{
    public partial class SystemForAwardManagementPage: BaseSearchPage
    {
        #region Related to Live Search
        public IWebElement SAMAnchorTag {
            get {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                Func<IWebDriver, IWebElement> waitForElement =
                    new Func<IWebDriver, IWebElement>((IWebDriver Web) =>
                    {
                        IList<IWebElement> Anchors = driver.FindElements(By.XPath("//form/a"));

                        foreach (IWebElement Anchor in Anchors)
                        {
                            if (Anchor.GetAttribute("title").ToLower() == "search records")
                                return Anchor;
                        }
                        throw new Exception("Could not find SAMAchorTag");
                    });
                IWebElement targetElement = wait.Until(waitForElement);
                return targetElement;
            }
        }

        public IWebElement SAMInputTag {
            get {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                Func<IWebDriver, IWebElement> waitForElement =
                    new Func<IWebDriver, IWebElement>((IWebDriver Web) =>
                    {
                        //PatrickFeb122017
                        IList<IWebElement> InputTags = Web.FindElements(By.Id("q"));

                        if (InputTags != null && InputTags.Count > 0)
                        {
                            return InputTags[0];
                        }
                        else
                        {
                            return null;
                        }
                    });
                IWebElement targetElement = wait.Until(waitForElement);
                return targetElement;
            }
        }

        public IWebElement SAMSubmitButton {
            get {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                Func<IWebDriver, IWebElement> waitForElement =
                    new Func<IWebDriver, IWebElement>((IWebDriver Web) =>
                    {
                        IWebElement Submit = driver.FindElement(By.Id("RegSearchButton"));
                        return Submit;
                    });
                IWebElement targetElement = wait.Until(waitForElement);
                return targetElement;
            }
        }

        public bool SAMCheckResult {
            get {
                try
                {
                    //driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
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
                    //driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(20));

                    IWebElement element = driver.FindElement(By.Id("its_docs"));
                    return element;
                }
                catch(Exception)
                {
                    return null;
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

        private IWebElement Test1{
            get {
                  WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                    Func<IWebDriver, IWebElement> waitForElement = new Func<IWebDriver, IWebElement>((IWebDriver Web) =>
                    {
                        IWebElement element = Web.FindElement(By.Id("its_docs"));
                        return element;
                    });
                    IWebElement targetElement = wait.Until(waitForElement);
                    return null;
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

        #endregion

        public IWebElement PageLastUpdatedTextElement
        {
            get
            {
                try
                {
                    string XPathValue = "//div[@id='foot']/div/p";
                    IList<IWebElement> Elements = driver.FindElements(By.XPath(XPathValue));
                    if (Elements.Count > 0)
                        return Elements[0];
                    else
                        throw new Exception();
                }
                catch (Exception ex)
                {
                    throw new Exception("Could not find PageLastUpdatedTextElement. " +
                        "Site may have been updated. Error Message: "
                        + ex.Message);
                }
            }
        }

        public IWebElement DataAccessAnchorTag
        {
            get
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                Func<IWebDriver, IWebElement> waitForElement =
                    new Func<IWebDriver, IWebElement>((IWebDriver Web) =>
                    {
                        //IList<IWebElement> Elements = driver.FindElements(By.XPath("//small"));
                        //return Elements[0];
                        IWebElement Anchor = driver.FindElement(
                            By.XPath("//div[@id='nav']/ul/li[3]/form/a"));

                        return Anchor;
                        //foreach (IWebElement Anchor in Anchor)
                        //{
                        //    if (Anchor.GetAttribute("title").ToLower().Trim() == "data access")
                        //        return Anchor;
                        //}
                        throw new Exception("Could not find SAMAchorTag");
                    });
                IWebElement targetElement = wait.Until(waitForElement);
                return targetElement;
            }
        }

        public IWebElement LatestExclusionExtractAnchorTag
        {
            get
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                Func<IWebDriver, IWebElement> waitForElement =
                    new Func<IWebDriver, IWebElement>((IWebDriver Web) =>
                    {
                        var AnchorTags = 
                        driver.FindElements(
                            By.CssSelector(
                                "div[class='contentDiv']"));

                        var temp = 
                        AnchorTags[AnchorTags.Count - 1].FindElements(
                            By.XPath(
                                "//table/tbody/tr[3]/td[1]/a"));

                        var AnchorTag = temp[temp.Count - 1];
                        return AnchorTag;
                    });
                IWebElement targetElement = wait.Until(waitForElement);
                return targetElement;
            }
        }
    }
}
