using DDAS.Data.Mongo;
using DDAS.Models.Entities.Domain;
using DDAS.Services.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml;
using WebScraping.Selenium.SearchEngine;
using static DDAS.Models.ViewModels.DDASResponseModel;
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
        public ddresponse iSprintToDDAS(ddRequest DR)
        {
            try
            {
                var ConnectionString =
                             System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

                var DBName =
                    System.Configuration.ConfigurationManager.AppSettings["DBName"];

                var _uow = new UnitOfWork(ConnectionString, DBName);
                var _config = new Config();
                var _SearchEngine = new SearchEngine(_uow, _config);
                ComplianceFormService c = new ComplianceFormService(_uow, _SearchEngine, _config);
                var obj = c.ImportIsprintData(DR);
                return ComplianceFormToResponse(obj);
            }
            catch (Exception ex)
            {
                SoapException retEx = new SoapException(ex.Message, SoapException.ServerFaultCode, "", ex.InnerException);
                throw retEx;
            }

        }

        [WebMethod]
        public ddresponse iSprintToDDASVerify(string Recid)
        {
            try
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
                return ComplianceFormToResponse(obj);
            }
            catch (Exception ex)
            {
                SoapException retEx = new SoapException(ex.Message, SoapException.ServerFaultCode, "", ex.InnerException);
                throw retEx;
            }

        }

        public ddresponse ComplianceFormToResponse(ComplianceForm form)
        {
            var obj = new ddresponse();

            obj.recid = form.RecId.ToString();

            var proj = new Models.ViewModels.DDASResponseModel.ddRequestProject();
            proj.projectNumber = form.ProjectNumber;
            proj.sponsorProtocolNumber = form.SponsorProtocolNumber;

            obj.project = proj;

            var institute = new Models.ViewModels.DDASResponseModel.ddRequestInstitute();

            institute.name = form.Institute;
            institute.address = form.Address;
            institute.country = form.Country;

            obj.institute = institute;

            int el = 0;
            obj.investigators = new Models.ViewModels.DDASResponseModel.ddRequestInvestigator[form.InvestigatorDetails.Count];

            foreach (var ddasinvestigator in form.InvestigatorDetails)
            {
                var investigator = new Models.ViewModels.DDASResponseModel.ddRequestInvestigator();

                investigator.firstName = ddasinvestigator.FirstName;
                investigator.middleName = ddasinvestigator.MiddleName;
                investigator.lastName = ddasinvestigator.LastName;
                investigator.investigatorId = ddasinvestigator.InvestigatorId;
                investigator.licenceNumber = ddasinvestigator.MedicalLiceseNumber;
                investigator.memberId = ddasinvestigator.MemberId;
                investigator.nameWithQualification = ddasinvestigator.Name;
                investigator.role = ddasinvestigator.Role;

                obj.investigators[el] = investigator;

                el += 1;
            }


            return obj;
        }
    }
}
