using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebScraping.Selenium.BaseClasses;

namespace WebScraping.Selenium.Pages
{
    public partial class AdequateAssuranceListPage : BaseSearchPage //BaseClasses.BasePage
    {
        public IWebElement AdequateAssuranceListTable
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
                    string XPathValue = "//div[@id='pagetools_right']/p";
                    IList<IWebElement> Elements = driver.FindElements(By.XPath(XPathValue));
                    return Elements[0];
                }
                catch(Exception)
                {
                    throw new Exception(
                        "Unable to find PageLastUpdatedElement. " + 
                        "Site May have been updated");
                }
            }
        }

        private bool IsSiteDown
        {
            get
            {
                if (driver.PageSource.ToLower().Contains("page not found"))
                {
                    return true;
                }
                else
                    return false;
            }
        }
    }
}
