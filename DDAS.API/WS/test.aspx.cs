using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using static DDAS.Models.ViewModels.RequestPayloadforiSprint;
using System.Globalization;
using System.Xml.Serialization;
using System.Xml;

namespace DDAS.API.WS
{
    public partial class test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }

        public static string ObjectToSOAP(object Object)
        {
            if (Object == null)
            {
                throw new ArgumentException("Object can not be null");
            }
            using (MemoryStream Stream = new MemoryStream())
            {
                SoapFormatter Serializer = new SoapFormatter();
                Serializer.Serialize(Stream, Object);
                Stream.Flush();
                return UTF8Encoding.UTF8.GetString(Stream.GetBuffer(), 0, (int)Stream.Position);
            }
        }

        private void test1()
        {

            Envelope en = new Envelope();
            EnvelopeBody b = new EnvelopeBody();

            DueDiligenceiSprintRequest x = new DueDiligenceiSprintRequest();
            CultureInfo ci = CultureInfo.InvariantCulture;

            //x.header.message_id = "jojo";
            //x.header.sender = "toto";
            DueDiligenceiSprintRequestHeader h = new DueDiligenceiSprintRequestHeader();
            DueDiligenceiSprintRequestDDResults r = new DueDiligenceiSprintRequestDDResults();

            project p = new project();
            institutuions i = new institutuions();

            p.projectNumber = 9002;
            p.sponsorProtocolNumber = 202;
            //i.checksCompleted.check.name = "institute world check";
            //i.checksCompleted.check.date = DateTime.ParseExact("2017-08-20", "yyyy-MM-dd", ci);

            r.project = p;
            //r.institutuions = i;

            x.DDResults = r;
            b.DueDiligenceiSprintRequest = x;
            en.Body = b;


            //XmlTypeMapping myTypeMapping = new SoapReflectionImporter().ImportTypeMapping(typeof(DueDiligenceiSprintRequest));
            //XmlSerializer xsSubmit = new XmlSerializer(myTypeMapping);


            XmlSerializer xsSubmit = new XmlSerializer(typeof(Envelope));
            //var subReq = new MyObject();
            string xml = "";

            using (var sww = new Utf8StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    xsSubmit.Serialize(writer, en);
                    xml = sww.ToString(); // Your XML
                }
            }


            //using (StringWriter writer = new StringWriter())
            //{
            //    xsSubmit.Serialize(writer, en);
            //    xml = writer.ToString();
            //}


            //xml = ObjectToSOAP(en);

            txtBody.Text = xml;
        }

        private void test2()
        {
            Envelope en = new Envelope();
            EnvelopeBody b = new EnvelopeBody();

            DueDiligenceiSprintRequest x = new DueDiligenceiSprintRequest();
            CultureInfo ci = CultureInfo.InvariantCulture;

            DueDiligenceiSprintRequestHeader h = new DueDiligenceiSprintRequestHeader();
            h.sender = "sri";
            h.timestamp = DateTime.Now.ToUniversalTime();
            h.message_id = "sri1";

            x.header = h;

            DueDiligenceiSprintRequestDDResults r = new DueDiligenceiSprintRequestDDResults();



            //<<<<<<<<<< project

            project p = new project();

            p.projectNumber = 9002;
            p.sponsorProtocolNumber = 202;

            r.project = p;

            //<<<<<<<<<<< institutuions

            institutuions i = new institutuions();
            institutuionsChecksCompleted ichk = new institutuionsChecksCompleted();
            institutuionsChecksCompletedCheck ichkchk = new institutuionsChecksCompletedCheck();
            ichkchk.name = "institute world check";
            ichkchk.date = DateTime.ParseExact(DateTime.Now.Date.ToString("yyyy-MM-dd"), "yyyy-MM-dd", ci);

            ichk.check = ichkchk;
            i.checksCompleted = ichk;
            i.instituteComplianceIssue = true;

            institutuionsDdFindings iddf = new institutuionsDdFindings();
            institutuionsDdFindingsFinding iddff = new institutuionsDdFindingsFinding();

            iddff.date = DateTime.ParseExact("2017-08-20", "yyyy-MM-dd", ci);
            iddff.type = "Regulatory";
            iddff.regulatoryCode = "OAI";
            iddff.regulatoryDeficiency = "No";
            iddff.worldCheckFinding = "No";
            iddff.comment = "Test123";

            iddf.finding = iddff;
            i.ddFindings = iddf;

            r.institutuions = i;

            //>>>>>>>>>>>>>>>>>


            //<<<<<<<<<<<<<<<<< investigatorResults

            investigatorResults irs = new investigatorResults();
            investigatorResultsInvestigatorResult ir = new investigatorResultsInvestigatorResult();

            ir.investigatorId = 101010;
            ir.memberId = 101010;
            ir.firstName = "Srinivas";
            ir.middleName = "A";
            ir.lastName = "Suri";
            ir.ddStatus = "Available";
            ir.ddCompletedDate = DateTime.ParseExact("2017-08-20", "yyyy-MM-dd", ci);


            investigatorResultsInvestigatorResultChecksCompleted irschk = new investigatorResultsInvestigatorResultChecksCompleted();
            investigatorResultsInvestigatorResultChecksCompletedCheck irschkchk = new investigatorResultsInvestigatorResultChecksCompletedCheck();

            irschkchk.name = "institute world check";
            irschkchk.date = DateTime.ParseExact("2017-08-20", "yyyy-MM-dd", ci);

            irschk.check = irschkchk;
            ir.checksCompleted = irschk;


            ir.dmc9002CheckDate = DateTime.ParseExact("2017-08-20", "yyyy-MM-dd", ci);
            ir.dmc9002Exclusion = "Exclusion";

            investigatorResultsInvestigatorResultDdFindings irsddf = new investigatorResultsInvestigatorResultDdFindings();
            investigatorResultsInvestigatorResultDdFindingsFinding irsddff = new investigatorResultsInvestigatorResultDdFindingsFinding();

            irsddff.date = DateTime.ParseExact("2017-08-20", "yyyy-MM-dd", ci);
            irsddff.type = "Regulatory";
            irsddff.regulatoryCode = "OAI";
            irsddff.regulatoryDeficiency = "No";
            irsddff.worldCheckFinding = "No";
            irsddff.comment = "TestInv";

            irsddf.finding = irsddff;
            ir.ddFindings = irsddf;

            irs.investigatorResult = ir;
            r.investigatorResults = irs;

            //>>>>>>>>>>>>>>>>>>>>>>

            x.DDResults = r;
            b.DueDiligenceiSprintRequest = x;
            en.Body = b;


            //XmlTypeMapping myTypeMapping = new SoapReflectionImporter().ImportTypeMapping(typeof(DueDiligenceiSprintRequest));
            //XmlSerializer xsSubmit = new XmlSerializer(myTypeMapping);


            XmlSerializer xsSubmit = new XmlSerializer(typeof(Envelope));
            //var subReq = new MyObject();
            string xml = "";

            using (var sww = new Utf8StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    xsSubmit.Serialize(writer, en);
                    xml = sww.ToString(); // Your XML
                }
            }


            //using (StringWriter writer = new StringWriter())
            //{
            //    xsSubmit.Serialize(writer, en);
            //    xml = writer.ToString();
            //}


            //xml = ObjectToSOAP(en);

            txtBody.Text = xml;

        }

        protected void btnTest_Click(object sender, EventArgs e)
        {
            test2();
        }
    }
}