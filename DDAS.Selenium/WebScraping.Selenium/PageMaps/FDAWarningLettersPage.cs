using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using OpenQA.Selenium;
using WebScraping.Selenium.BaseClasses;

namespace WebScraping.Selenium.Pages
{
    public partial class FDAWarningLettersPage : BaseSearchPage
    {
        public IWebElement FDASearchTextBox
        {
            get
            {
                try
                {
                    IWebElement Element = driver.FindElement(By.Id("qryStr"));
                    return Element;
                }
                catch(Exception)
                {
                    throw new Exception("Could not find FDASearchTextBox");
                }
            }
        }
        public IWebElement FDASearchButton
        {
            get
            {
                driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));

                IList<IWebElement> SearchButtons =
                    driver.FindElements(By.TagName("input"));
                //"input[value='Search']"

                foreach (IWebElement element in SearchButtons)
                {
                    if (element.GetAttribute("value").ToLower() == "search")
                        return element;
                }
                throw new Exception("Could not click on Search Button");
            }
        }

        public IWebElement FDASortTable
        {
            get
            {
                IList<IWebElement> Table = driver.FindElements(By.XPath("//table"));
                return Table[7];
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
                catch(Exception)
                {
                    throw new Exception("Could not find PageLastUpdatedElement");
                }
            }
        }
    }
}
