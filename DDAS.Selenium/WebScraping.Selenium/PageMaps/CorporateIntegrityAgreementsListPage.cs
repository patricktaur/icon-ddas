using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebScraping.Selenium.BaseClasses;

namespace WebScraping.Selenium.Pages
{
    public partial class CorporateIntegrityAgreementsListPage : BaseSearchPage
    {
        public IWebElement CIAListTable {
            get {
                IList<IWebElement> table = driver.FindElements(By.TagName("table"));
                return table[1];
                //return driver.FindElement(By.Id("cia_list"));
            }
        }

        public IWebElement PageLastUpdatedElement
        {
            get
            {
                try
                {
                    IWebElement Element = driver.FindElement(By.Id("updated-list"));
                    return Element;
                }
                catch(Exception ex)
                {
                    throw new Exception("Unable to find PageLastUpdatedElement. " +
                        "Site may have been updated. Error Message: " +
                        ex.Message);
                }
            }
        }
    }
}
