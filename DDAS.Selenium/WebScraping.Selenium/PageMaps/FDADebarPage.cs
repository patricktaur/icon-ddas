using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebScraping.Selenium.BaseClasses;

namespace WebScraping.Selenium.Pages
{
    public partial class FDADebarPage : BaseSearchPage // BaseClasses.BasePage
    {
        public IWebElement PersonsTable {
            get {
                IList<IWebElement> Tables = driver.FindElements(By.XPath("//table"));
                return Tables[1];
            }
        }

        public IWebElement PageLastUpdatedTextElement {
            get {
                IList<IWebElement> Elements = driver.FindElements(By.XPath("//small"));
                return Elements[0];
            }
        }
    }
}
