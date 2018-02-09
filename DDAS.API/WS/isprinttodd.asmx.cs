using DDAS.Data.Mongo;
using DDAS.Models.Entities;
using DDAS.Models.Entities.Domain;
using DDAS.Services.Search;
using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Serialization;
using WebScraping.Selenium.SearchEngine;
using static DDAS.Models.ViewModels.DDASResponseModel;
using static DDAS.Models.ViewModels.RequestPayloadforDDAS;

namespace DDAS.API.WS
{
    /// <summary>
    /// Summary description for isprinttoddas
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class isprinttodd : System.Web.Services.WebService
    {

        [WebMethod]
        public ddresponse iSprintToDD(ddRequest DR)
        {
            var objLog = new LogWSDDAS();

            objLog.CreatedOn = DateTime.Now;
            objLog.LocalIPAddress = HttpContext.Current.Request.ServerVariables.Get("LOCAL_ADDR");
            objLog.HostIPAddress = HttpContext.Current.Request.ServerVariables.Get("REMOTE_ADDR");
            objLog.PortNumber = HttpContext.Current.Request.ServerVariables.Get("SERVER_PORT");
            objLog.ServerProtocol = HttpContext.Current.Request.ServerVariables.Get("SERVER_PROTOCOL");
            objLog.ServerSoftware = HttpContext.Current.Request.ServerVariables.Get("SERVER_SOFTWARE");
            objLog.HttpHost = HttpContext.Current.Request.ServerVariables.Get("HTTP_HOST");
            objLog.ServerName = HttpContext.Current.Request.ServerVariables.Get("SERVER_NAME");
            objLog.GatewayInterface = HttpContext.Current.Request.ServerVariables.Get("GATEWAY_INTERFACE");
            objLog.Https = HttpContext.Current.Request.ServerVariables.Get("HTTPS");


            XmlSerializer xsSubmit = new XmlSerializer(typeof(ddRequest));
            //var subReq = new MyObject();
            string xml = "";

            using (var sww = new Utf8StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    xsSubmit.Serialize(writer, DR);
                    xml = sww.ToString(); // Your XML
                }
            }

            objLog.RequestPayload = xml;


            //===========================

            var ConnectionString =
                         System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            var DBName =
                System.Configuration.ConfigurationManager.AppSettings["DBName"];

            var _uow = new UnitOfWork(ConnectionString, DBName);
            var _config = new Config();
            var _SearchEngine = new SearchEngine(_uow, _config);



            try
            {

                ComplianceFormService c = new ComplianceFormService(_uow, _SearchEngine, _config);
                var obj = c.ImportIsprintData(DR);
                var objResponse = ComplianceFormToResponse(obj);


                //<<<< Convert response to log

                XmlSerializer xsResponse = new XmlSerializer(typeof(ddresponse));

                xml = "";

                using (var sww = new Utf8StringWriter())
                {
                    using (XmlWriter writer = XmlWriter.Create(sww))
                    {
                        xsResponse.Serialize(writer, objResponse);
                        xml = sww.ToString(); // Your XML
                    }
                }

                //>>>>>

                objLog.Response = xml;
                objLog.Status = "Success";

                _uow.LogWSDDASRepository.Add(objLog);

                return objResponse;
            }
            catch (Exception ex)
            {
                SoapException retEx = new SoapException(ex.Message, SoapException.ServerFaultCode, "", ex.InnerException);

                //<<<< Convert response to log

                XmlSerializer xsException = new XmlSerializer(typeof(SoapException));

                xml = "";

                using (var sww = new Utf8StringWriter())
                {
                    using (XmlWriter writer = XmlWriter.Create(sww))
                    {
                        xsException.Serialize(writer, retEx);
                        xml = sww.ToString(); // Your XML
                    }
                }

                //>>>>>

                objLog.Response = xml;
                objLog.Status = "Failed";

                _uow.LogWSDDASRepository.Add(objLog);


                throw retEx;
            }
            finally
            {
               // objLog.Response = xml;

               // _uow.LogWSDDASRepository.Add(objLog);
            }
        }

        public class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
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
