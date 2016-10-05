using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScraping.Selenium.Pages
{
    public partial class SpeciallyDesignatedNationalsListPage : 
        BaseClasses.BasePage
    {
        public IWebElement SDNSiteUpdatedDate
        {
            get
            {
                IList<IWebElement> Divs = 
                    driver.FindElements(By.ClassName("updated"));
                return Divs[0];
            }
        }
    }
}
