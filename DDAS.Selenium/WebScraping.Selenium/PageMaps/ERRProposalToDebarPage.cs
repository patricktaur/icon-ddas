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
                string XPathValue = "//div[@id='pagetools_right']/p";
                IList<IWebElement> Elements = driver.FindElements(By.XPath(XPathValue));
                return Elements[0];
            }
        }
    }
}
