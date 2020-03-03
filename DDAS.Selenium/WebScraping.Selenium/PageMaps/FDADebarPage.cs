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
                try
                {
                    //IList<IWebElement> Elements = driver.FindElements(By.XPath("//aside/ul/li/div/p"));
                    IList<IWebElement> Elements = driver.FindElements(By.TagName("time"));
                    return Elements[0];
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
