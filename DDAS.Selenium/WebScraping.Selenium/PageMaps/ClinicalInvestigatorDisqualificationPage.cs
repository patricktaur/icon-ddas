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
    public partial class ClinicalInvestigatorDisqualificationPage: BaseSearchPage //BaseClasses.BasePage
    {

        public IWebElement DisqualifiedInvestigatorSearchTextBox
        {
            get
            {
                try
                {
                    IWebElement SearchTextBox = driver.FindElement(By.Id("filter"));
                    return SearchTextBox;
                }
                catch(Exception)
                {
                    throw new Exception(
                        "Unable to find DisqualifiedInvestigatorSearchTextBox");
                }
            }
        }

        public IWebElement DisqualifiedInvestigatorSubmitButton
        {
            get
            {
                IList<IWebElement> SubmitButton = 
                    driver.FindElements(By.CssSelector("input[type='submit']"));

                foreach(IWebElement Button in SubmitButton)
                {
                    if (Button.GetAttribute("value").ToLower() == "show items")
                        return Button;
                }
                throw new Exception("Unable to find Submit button!");
            }
        }

        public IWebElement DisqualifiedInvestigatorCountTable
        {
            get
            {
                IList<IWebElement> Tables = driver.FindElements(By.XPath("//table"));
                return Tables[2];
            }
        }

        public IWebElement DisqualifiedInvestigatorTable
        {
            get
            {
                IList<IWebElement> Tables = driver.FindElements(By.XPath("//table"));
                return Tables[3];
            }
        }

        public IWebElement PageLastUpdatedTextElement
        {
            get
            {
                try
                {
                    IWebElement Element = driver.FindElement(By.Id("pagetools_right"));
                    return Element;
                }
                catch (Exception)
                {
                    throw new Exception(
                        "Unable to find PageLastUpdatedTextElement");
                }

            }
        }
    }
}
