using DDAS.Data.Mongo;
using DDAS.Models.Entities.Domain;
using DDAS.Services.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using WebScraping.Selenium.SearchEngine;
using static DDAS.Models.ViewModels.RequestPayloadforDDAS;

namespace DDAS.API.WS
{
    /// <summary>
    /// Summary description for ddaswebservice
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class ddaswebservice : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod]
        public ComplianceForm iSprintToDDAS(ddRequest DR)
        {
            var ConnectionString =
               System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            var DBName =
                System.Configuration.ConfigurationManager.AppSettings["DBName"];

            var _uow = new UnitOfWork(ConnectionString, DBName);
            var _config = new Config();
            var _SearchEngine = new SearchEngine(_uow, _config);
            ComplianceFormService c = new ComplianceFormService(_uow,_SearchEngine,_config);
            var obj = c.ImportIsprintData(DR);
            return obj;
        }

        [WebMethod]
        public ComplianceForm iSprintToDDASVerify(string Recid)
        {

            var ConnectionString =
              System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            var DBName =
                System.Configuration.ConfigurationManager.AppSettings["DBName"];

            var _uow = new UnitOfWork(ConnectionString, DBName);
            var _config = new Config();
            var _SearchEngine = new SearchEngine(_uow, _config);
            ComplianceFormService c = new ComplianceFormService(_uow, _SearchEngine, _config);
            var obj = c.GetComplianceForm(Guid.Parse(Recid));
            return obj;
        }
    }
}
