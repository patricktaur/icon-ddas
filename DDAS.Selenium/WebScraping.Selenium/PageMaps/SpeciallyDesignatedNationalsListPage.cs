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
        public IWebElement PageLastUpdatedElement
        {
            get
            {
                try
                {
                    IList<IWebElement> Divs =
                        driver.FindElements(By.ClassName("updated"));
                    return Divs[0];
                }
                catch(Exception ex)
                {
                    throw new Exception("Could not find PageLastUpdatedElement. " +
                        "Site may have been updated. Error Message: " +
                        ex.Message);
                }
            }
        }
    }
}
