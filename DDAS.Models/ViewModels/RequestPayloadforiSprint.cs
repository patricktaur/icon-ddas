using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.ViewModels
{
    public class RequestPayloadforiSprint
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false)]
        public partial class Envelope
        {
            /// <remarks/>
            public object Header { get; set; }

            /// <remarks/>
            public EnvelopeBody Body { get; set; }

        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public partial class EnvelopeBody
        {
            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.iconplc.com/InitiateDDASiSprintFindings/xsd/v3/initiateDDASiSprintFind" +
                "ingsMsg")]
            public DueDiligenceiSprintRequest DueDiligenceiSprintRequest { get; set; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.iconplc.com/InitiateDDASiSprintFindings/xsd/v3/initiateDDASiSprintFind" +
            "ingsMsg")]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.iconplc.com/InitiateDDASiSprintFindings/xsd/v3/initiateDDASiSprintFind" +
            "ingsMsg", IsNullable = false)]
        public partial class DueDiligenceiSprintRequest
        {
            /// <remarks/>
            public DueDiligenceiSprintRequestHeader header { get; set; }

            /// <remarks/>
            public DueDiligenceiSprintRequestDDResults DDResults { get; set; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.iconplc.com/InitiateDDASiSprintFindings/xsd/v3/initiateDDASiSprintFind" +
            "ingsMsg")]
        public partial class DueDiligenceiSprintRequestHeader
        {
            /// <remarks/>
            public string sender { get; set; }

            /// <remarks/>
            public System.DateTime timestamp { get; set; }

            /// <remarks/>
            public string message_id { get; set; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.iconplc.com/InitiateDDASiSprintFindings/xsd/v3/initiateDDASiSprintFind" +
            "ingsMsg")]
        public partial class DueDiligenceiSprintRequestDDResults
        {

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://edh.esb.iconplc.com")]
            public project project { get; set; }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://edh.esb.iconplc.com")]
            public institutions institutions { get; set; }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://edh.esb.iconplc.com")]
            public investigatorResults investigatorResults { get; set; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://edh.esb.iconplc.com")]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://edh.esb.iconplc.com", IsNullable = false)]
        public partial class project
        {
            /// <remarks/>
            public string projectNumber { get; set; }

            /// <remarks/>
            public string sponsorProtocolNumber { get; set; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://edh.esb.iconplc.com")]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://edh.esb.iconplc.com", IsNullable = false)]
        public partial class institutions
        {
            /// <remarks/>
            public institutionsChecksCompleted checksCompleted { get; set; }

            /// <remarks/>
            public bool instituteComplianceIssue { get; set; }

            /// <remarks/>
            public institutionsDdFindings ddFindings { get; set; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://edh.esb.iconplc.com")]
        public partial class institutionsChecksCompleted
        {
            /// <remarks/>
            public institutionsChecksCompletedCheck check { get; set; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://edh.esb.iconplc.com")]
        public partial class institutionsChecksCompletedCheck
        {
            /// <remarks/>
            public string name { get; set; }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
            public System.DateTime? date { get; set; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://edh.esb.iconplc.com")]
        public partial class institutionsDdFindings
        {
            /// <remarks/>
            public institutionsDdFindingsFinding finding { get; set; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://edh.esb.iconplc.com")]
        public partial class institutionsDdFindingsFinding
        {
            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
            public System.DateTime? date { get; set; }

            /// <remarks/>
            public string type { get; set; }

            /// <remarks/>
            public string regulatoryCode { get; set; }

            /// <remarks/>
            public string regulatoryDeficiency { get; set; }

            /// <remarks/>
            public string worldCheckFinding { get; set; }

            /// <remarks/>
            public string comment { get; set; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://edh.esb.iconplc.com")]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://edh.esb.iconplc.com", IsNullable = false)]
        public partial class investigatorResults
        {
            /// <remarks/>
            public investigatorResultsInvestigatorResult[] investigatorResult { get; set; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://edh.esb.iconplc.com")]
        public partial class investigatorResultsInvestigatorResult
        {
            /// <remarks/>
            public string investigatorId { get; set; }

            /// <remarks/>
            public string memberId { get; set; }

            /// <remarks/>
            public string firstName { get; set; }

            /// <remarks/>
            public string middleName { get; set; }

            /// <remarks/>
            public string lastName { get; set; }

            /// <remarks/>
            public string ddStatus { get; set; }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
            public System.DateTime? ddCompletedDate { get; set; }

            /// <remarks/>
            public investigatorResultsInvestigatorResultChecksCompleted checksCompleted { get; set; }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
            public System.DateTime? dmc9002CheckDate {get; set;}

            /// <remarks/>
            public string dmc9002Exclusion { get; set; }

            /// <remarks/>
            public investigatorResultsInvestigatorResultDdFindings[] ddFindings { get; set; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://edh.esb.iconplc.com")]
        public partial class investigatorResultsInvestigatorResultChecksCompleted
        {
            /// <remarks/>
            public investigatorResultsInvestigatorResultChecksCompletedCheck check { get; set; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://edh.esb.iconplc.com")]
        public partial class investigatorResultsInvestigatorResultChecksCompletedCheck
        {
            /// <remarks/>
            public string name { get; set; }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
            public System.DateTime? date { get; set; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://edh.esb.iconplc.com")]
        public partial class investigatorResultsInvestigatorResultDdFindings
        {
            /// <remarks/>
            public investigatorResultsInvestigatorResultDdFindingsFinding finding { get; set; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://edh.esb.iconplc.com")]
        public partial class investigatorResultsInvestigatorResultDdFindingsFinding
        {
            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
            public System.DateTime? date { get; set; }

            /// <remarks/>
            public string type { get; set; }

            /// <remarks/>
            public string regulatoryCode { get; set; }

            /// <remarks/>
            public string regulatoryDeficiency { get; set; }

            /// <remarks/>
            public string worldCheckFinding { get; set; }

            /// <remarks/>
            public string comment { get; set; }
        }
    }
}
