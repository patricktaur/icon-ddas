using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.PhantomJS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebScraping.Selenium.SearchEngine;

namespace WebScraping.Tests
{
    public class TestSites
    {
        private IWebDriver _Driver;

        public void TestDDASSites()
        {
            //string path = @"C:\Documents";
            //SearchEngine search = new SearchEngine(path);
            //search.SearchByName("Hari", DDAS.Models.Enums.SiteEnum.SystemForAwardManagementPage);
        }

        public IWebDriver Driver
        {
            get
            {
                if (_Driver == null)
                {
                    //PhantomJSDriverService service = PhantomJSDriverService.CreateDefaultService();
                    //service.IgnoreSslErrors = true;
                    //service.SslProtocol = "any";
                    ////service.LocalStoragePath = _config.AppDataDownloadsFolder;
                    //_Driver = new PhantomJSDriver(service);

                    //Patrick 16Feb2017
                    //_Driver.Manage().Window.Maximize();
                    //_Driver.Manage().Window.Size = new Size(1124, 850);
                    //_Driver.Manage().Window.Size = new Size(800, 600);

                    _Driver = new ChromeDriver(@"C:\Development\p926-ddas\Libraries1\ChromeDriver");

                    _Driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(20));
                    _Driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(2));

                    return _Driver;
                }
                else
                    return _Driver;
            }
        }

        public void TestLogin()
        {
            Driver.Navigate().GoToUrl("http://ddasuat.claritytechnologies.com/login");

            IList<IWebElement> Elements = Driver.FindElements(By.TagName("input"));

            IWebElement UserNameElement = null;
            IWebElement PasswordElement = null;
            foreach (IWebElement element in Elements)
            {
                if (element.GetAttribute("name").ToLower() == "username")
                {
                    UserNameElement = element;
                }
                else if(element.GetAttribute("name").ToLower() == "password")
                {
                    PasswordElement = element;
                }
            }

            IWebElement SubmitElement = null;

            if(UserNameElement != null && PasswordElement != null)
            {
                UserNameElement.SendKeys("pradeep");
                PasswordElement.SendKeys("Clarity@148");

                SubmitElement = Driver.FindElement(By.CssSelector("button[type='submit']"));
                SubmitElement.SendKeys(Keys.Enter);

                if (Driver.Url.ToLower().Contains("search"))
                {
                    Console.WriteLine("Login Successful");
                }
            }
        }
    }
}
