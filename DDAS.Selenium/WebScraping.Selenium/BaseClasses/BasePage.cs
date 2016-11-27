using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScraping.Selenium.BaseClasses
{
    public abstract class BasePage :IDisposable
    {
        protected IWebDriver driver;
        public BasePage(IWebDriver driver)
        {
            this.driver = driver;
        }

        public abstract string Url { get; }

        public virtual void Open(string part = "")
        {
            bool IsPageLoaded = false;
            for (int Counter = 1; Counter <= 10; Counter++)
            {
                try
                {
                    if (!IsPageLoaded)
                    {
                        driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
                        driver.Navigate().GoToUrl(string.Concat(Url, part));
                        IsPageLoaded = true;
                    }
                }
                catch (WebDriverTimeoutException)
                {
                    //
                }
            }
        }

        public bool IsElementPresent(IWebElement elem, By by)
        {
            try
            {
                elem.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public void SaveScreenShot(string fileName)
        {
            Screenshot ss = ((ITakesScreenshot)this.driver).GetScreenshot();
            //ss.SaveAsFile(fileName, System.Drawing.Imaging.ImageFormat.Png);
        }

        public void Dispose()
        {
            driver.Quit();
        }
    }
}
