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
                catch(Exception)
                {
                    throw new Exception("Could not find PageLastUpdatedElement");
                }
            }
        }
    }
}
