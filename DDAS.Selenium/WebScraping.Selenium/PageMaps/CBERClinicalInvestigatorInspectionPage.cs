using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebScraping.Selenium.BaseClasses;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;

namespace WebScraping.Selenium.Pages
{
    public partial class CBERClinicalInvestigatorInspectionPage : BaseSearchPage //BaseClasses.BasePage
    {
        public IWebElement CBERClinicalInvestigatorTable
        {
            get
            {
                IList<IWebElement> Tables = driver.FindElements(By.XPath("//table"));
                return Tables[0];
            }
        }

        public IWebElement PageLastUpdatedTextElement
        {
            get
            {
                try
                {
                    IList<IWebElement> Elements = driver.FindElements(By.XPath("//small"));
                    return Elements[0];
                }
                catch(Exception)
                {
                    throw new Exception("Unable to find PageLastUpdatedElement");
                }
            }
        }
    }
}
