using DDAS.Models;
using DDAS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using static DDAS.Models.ViewModels.iSprintResponseModel;
using static DDAS.Models.ViewModels.RequestPayloadforDDAS;
using static DDAS.Models.ViewModels.RequestPayloadforiSprint;

namespace DDAS.API.WS
{
    /// <summary>
    /// Summary description for mockisprint
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class mockisprint : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod]
        public DueDiligenceiSprintRequest DDASRequest(DueDiligenceiSprintRequest DR)
        {
            //iSprintResponseModel.Envelope en = new iSprintResponseModel.Envelope();

            //EnvelopeHeader eh = new EnvelopeHeader();
            //FaultTo f2 = new FaultTo();
            //ReplyTo r2 = new ReplyTo();
            //ReplyToReferenceParameters r2rp = new ReplyToReferenceParameters();


            //r2rp.trackingecid = "4f08045f-232c-4398-afcc-1e64f8ee1854-021a77a0";
            //r2rp.trackingFlowEventId = 188495062;
            //r2rp.trackingFlowId = 6490135;
            //r2rp.trackingCorrelationFlowId = "0000M2TWqePDoY_5xR8DyW1QF1sP000027";

            //r2.ReferenceParameters = r2rp;

            //f2.Address = "http://www.w3.org/2005/08/addressing/anonymous";

            //eh.MessageID = "urn:f10c4399-ebda-11e7-86dd-005056ac33ff";
            //eh.FaultTo = f2;
            //eh.ReplyTo = r2;

            //en.Header = eh;

            //iSprintResponse iResp = new iSprintResponse();

            //iSprintResponseHeader iRespH = new iSprintResponseHeader();

            //DueDiligenceiSprintRequestHeader drh = new DueDiligenceiSprintRequestHeader();
            //drh = DR.header;

            //iRespH.sender = drh.sender;
            //iRespH.timestamp = drh.timestamp;
            //iRespH.message_id = drh.message_id;


            //iResp.success = true;

            //iSprintResponseModel.EnvelopeBody enb = new iSprintResponseModel.EnvelopeBody();

            //enb.iSprintResponse = iResp;

            //en.Body = enb;

            //return en;
            return DR;
        }
    }
}
