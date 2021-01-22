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
            for (int Counter = 1; Counter <= 20; Counter++)
            {
                try
                {
                    if (!IsPageLoaded)
                    {
                        //driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(10);
                        driver.Navigate().GoToUrl(string.Concat(Url, part));
                        if (!driver.Url.ToLower().Contains("about:blank"))
                        {
                            IsPageLoaded = true;
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new Exception(String.Format("Unable to load the page @  {0}, Error: {1} ", Url, e.Message));
                }
            }
            if(!IsPageLoaded)
                throw new Exception("Unable to load the page");
        }

        public bool IsElementPresent(IWebElement elem, By by)
        {
            try
            {
                //driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0);
                elem.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            finally
            {
                //driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            }
        }


        public static bool isElementPresentAndDisplayed( IWebElement element)
        {
            try
            {
                 return element.Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public void SaveScreenShot(string fileName)
        {
            Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();
            //ss.SaveAsFile(fileName, System.Drawing.Imaging.ImageFormat.Png);
            ss.SaveAsFile(fileName, ScreenshotImageFormat.Jpeg);
        }

        public void Dispose()
        {
            driver.Quit();
        }
    }
}
