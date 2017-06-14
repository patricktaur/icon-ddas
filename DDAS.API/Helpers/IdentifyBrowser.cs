using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DDAS.API.Helpers
{
    public static class IdentifyBrowser
    {
        public static string GetBrowserType(string BrowserType)
        {
            if (BrowserType.ToLower().Contains("edge"))
                return "Edge";
            else if (BrowserType.ToLower().Contains("trident"))
                return "IE";
            else if (BrowserType.ToLower().Contains("chrome"))
                return "Chrome";
            else if (BrowserType.ToLower().Contains("mozilla"))
                return "Mozilla";

            return "unknown";
        }
    }
}