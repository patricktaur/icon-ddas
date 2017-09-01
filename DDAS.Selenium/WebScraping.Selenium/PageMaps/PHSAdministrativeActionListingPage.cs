using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebScraping.Selenium.BaseClasses;

namespace WebScraping.Selenium.Pages
{
    public partial class PHSAdministrativeActionListingPage : BaseSearchPage
    {
        public IWebElement PHSTable {
            get {
                IList<IWebElement> table = driver.FindElements(By.XPath("//table"));
                return table[0];
            }
        }

        public IWebElement PageLastUpdatedTextElement
        {
            get
            {
                try
                {
                    IWebElement Element = driver.FindElement(By.XPath("//h1"));
                    return Element;
                }
                catch (Exception ex)
                {
                    throw new Exception("Could not find PageLastUpdatedElement. " +
                        "Site may have been updated. Error Message: " +
                        ex.Message);
                }
            }
        }
    }
}
