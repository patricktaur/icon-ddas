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
        public bool IsFeedbackPopUpDisplayed
        {
            get
            {
                if (driver.PageSource.ToLower().Contains("give feeback"))
                    return true;
                else
                    return false;
            }
        }

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
                    IList<IWebElement> Elements = driver.FindElements(By.XPath("//aside/ul/li/div/p"));
                    return Elements[0];
                }
                catch(Exception ex)
                {
                    throw new Exception("Unable to find PageLastUpdatedElement. " +
                        "Site May have been updated. Error Message: " +
                        ex.Message);
                }
            }
        }
    }
}
