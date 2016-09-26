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
        public IWebElement FDASearchTextBox {
            get {
                IWebElement Element = driver.FindElement(By.Id("qryStr"));
                return Element;
            }
        }
        public IWebElement FDASearchButton
        {
            get
            {
                IList<IWebElement> SearchButtons = driver.FindElements(By.TagName("input"));
                
                foreach(IWebElement element in SearchButtons)
                {
                    if (element.GetAttribute("value").ToLower() == "search")
                        return element;
                }
                return null;
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
    }
}
