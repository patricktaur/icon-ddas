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

                driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));

                IList<IWebElement> table = driver.FindElements(By.TagName("table"));
                return table[1];
            }
        }
    }
}
