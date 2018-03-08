using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using static DDAS.Models.ViewModels.RequestPayloadforiSprint;
using System.Globalization;
using System.Xml.Serialization;
using System.Xml;
using DDAS.Data.Mongo;
using DDAS.Models.Entities.Domain;
using WebScraping.Selenium.SearchEngine;
using DDAS.Services.Search;

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
            institutions i = new institutions();

            p.projectNumber = "09002";
            p.sponsorProtocolNumber = "202";
            //i.checksCompleted.check.name = "institute world check";
            //i.checksCompleted.check.date = DateTime.ParseExact("2017-08-20", "yyyy-MM-dd", ci);

            r.project = p;
            //r.institutions = i;

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

            p.projectNumber = "9002";
            p.sponsorProtocolNumber = "202";

            r.project = p;

            //<<<<<<<<<<< institutions

            institutions i = new institutions();
            institutionsChecksCompleted ichk = new institutionsChecksCompleted();
            institutionsChecksCompletedCheck ichkchk = new institutionsChecksCompletedCheck();
            ichkchk.name = "institution world check";
            ichkchk.date = DateTime.ParseExact(DateTime.Now.Date.ToString("yyyy-MM-dd"), "yyyy-MM-dd", ci);

            ichk.check = ichkchk;
            i.checksCompleted = ichk;
            i.instituteComplianceIssue = true;

            institutionsDdFindings iddf = new institutionsDdFindings();
            institutionsDdFindingsFinding iddff = new institutionsDdFindingsFinding();

            iddff.date = DateTime.ParseExact("2017-08-20", "yyyy-MM-dd", ci);
            iddff.type = "Regulatory";
            iddff.regulatoryCode = "OAI";
            iddff.regulatoryDeficiency = "No";
            iddff.worldCheckFinding = "No";
            iddff.comment = "Test123";

            iddf.finding = iddff;
            i.ddFindings = iddf;

            r.institutions = i;

            //>>>>>>>>>>>>>>>>>


            //<<<<<<<<<<<<<<<<< investigatorResults

            investigatorResults irs = new investigatorResults();
            investigatorResultsInvestigatorResult ir = new investigatorResultsInvestigatorResult();

            ir.investigatorId = "101010";
            ir.memberId = "101010";
            ir.firstName = "Srinivas";
            ir.middleName = "A";
            ir.lastName = "Suri";
            ir.ddStatus = "Available";
            ir.ddCompletedDate = DateTime.ParseExact("2017-08-20", "yyyy-MM-dd", ci);


            investigatorResultsInvestigatorResultChecksCompleted irschk = new investigatorResultsInvestigatorResultChecksCompleted();
            investigatorResultsInvestigatorResultChecksCompletedCheck irschkchk = new investigatorResultsInvestigatorResultChecksCompletedCheck();

            irschkchk.name = "investigator world check";
            irschkchk.date = DateTime.ParseExact("2017-08-20", "yyyy-MM-dd", ci);

            irschk.check = irschkchk;
            ir.checksCompleted = irschk;


            ir.dmc9002CheckDate = DateTime.ParseExact("2017-08-20", "yyyy-MM-dd", ci);
            ir.dmc9002Exclusion = "Exclusion";

            investigatorResultsInvestigatorResultDdFindings[] irsddf = new investigatorResultsInvestigatorResultDdFindings[1];
            investigatorResultsInvestigatorResultDdFindingsFinding irsddff = new investigatorResultsInvestigatorResultDdFindingsFinding();

            irsddff.date = DateTime.ParseExact("2017-08-20", "yyyy-MM-dd", ci);
            irsddff.type = "Regulatory";
            irsddff.regulatoryCode = "OAI";
            irsddff.regulatoryDeficiency = "No";
            irsddff.worldCheckFinding = "No";
            irsddff.comment = "TestInv";

            irsddf[0].finding = irsddff;
            ir.ddFindings = irsddf;

            //irs.investigatorResult = ir; jojo
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

        private void test3()
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
                txtBody.Text =  c.ExportDataToIsprint(Guid.Parse(txtRecid.Text)).Message;

                //var objCF = c.GetComplianceForm(Guid.Parse(txtRecid.Text));

            }
            catch (Exception ex)
            {
                throw ex;
            }

           


        }

        protected void btnTest_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(txtMode.Text) == 2)
            {
                test2();
            }
            else if (Convert.ToInt32(txtMode.Text) == 3)
            {
                test3();
            }
            else
            {
                test1();
            }
        }
    }
}