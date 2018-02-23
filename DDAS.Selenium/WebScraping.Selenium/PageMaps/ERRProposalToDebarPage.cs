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
                    //Site modified: 10Feb2018
                    //string XPathValue = "//div[@id='pagetools_right']/p";
                    //IList<IWebElement> Elements = driver.FindElements(By.XPath(XPathValue));
                    //return Elements[0];

                    string XPathValue = "//div[@class='col-lg-12 pagetools-bottom']";
                    IWebElement Elem = driver.FindElement(By.XPath(XPathValue));
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
