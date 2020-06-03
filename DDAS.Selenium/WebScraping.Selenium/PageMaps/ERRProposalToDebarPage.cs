using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebScraping.Selenium.BaseClasses;

namespace WebScraping.Selenium.Pages
{
    public partial class ERRProposalToDebarPage : BaseSearchPage //BaseClasses.BasePage
    {
        public IWebElement ProposalToDebarTable
        {
            get
            {
                IList<IWebElement> Tables = driver.FindElements(By.XPath("//table"));
                return Tables[0];
            }
        }

        public IWebElement PageLastUdpatedElement
        {
            get
            {
                try
                {
                    //string XPathValue = "//div[@class='col-lg-12 pagetools-bottom']";
                    //WebSite layout changes
                    //corrected on: 03June2020
                    //IWebElement Elem = driver.FindElement(By.XPath("//aside/ul/li/div/p"));
                    IWebElement Elem = driver.FindElement(By.XPath("//aside/section/div/aside/ul/div/li/div/p"));

                    return Elem;
                }
                catch (Exception ex)
                {
                    throw new Exception("Unable to find PageLastUpdatedElement. " +
                        "Site may have been udpated. Error Message: " +
                        ex.Message);
                }
            }
        }
    }
}
