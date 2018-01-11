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

            private object headerField;

            private EnvelopeBody bodyField;

            /// <remarks/>
            public object Header
            {
                get; set;
            }

            /// <remarks/>
            public EnvelopeBody Body { get; set; }

        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public partial class EnvelopeBody
        {

            private DueDiligenceiSprintRequest dueDiligenceiSprintRequestField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.iconplc.com/InitiateDDASiSprintFindings/xsd/v3/initiateDDASiSprintFind" +
                "ingsMsg")]
            public DueDiligenceiSprintRequest DueDiligenceiSprintRequest
            {
                get; set;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.iconplc.com/InitiateDDASiSprintFindings/xsd/v3/initiateDDASiSprintFind" +
            "ingsMsg")]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.iconplc.com/InitiateDDASiSprintFindings/xsd/v3/initiateDDASiSprintFind" +
            "ingsMsg", IsNullable = false)]
        public partial class DueDiligenceiSprintRequest
        {

            private DueDiligenceiSprintRequestHeader headerField;

            private DueDiligenceiSprintRequestDDResults dDResultsField;

            /// <remarks/>
            public DueDiligenceiSprintRequestHeader header
            {
                get;set;
            }

            /// <remarks/>
            public DueDiligenceiSprintRequestDDResults DDResults
            {
                get; set;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.iconplc.com/InitiateDDASiSprintFindings/xsd/v3/initiateDDASiSprintFind" +
            "ingsMsg")]
        public partial class DueDiligenceiSprintRequestHeader
        {

            private string senderField;

            private System.DateTime timestampField;

            private string message_idField;

            /// <remarks/>
            public string sender
            {
                get; set;
            }

            /// <remarks/>
            public System.DateTime timestamp
            {
                get; set;
            }

            /// <remarks/>
            public string message_id
            {
                get; set;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.iconplc.com/InitiateDDASiSprintFindings/xsd/v3/initiateDDASiSprintFind" +
            "ingsMsg")]
        public partial class DueDiligenceiSprintRequestDDResults
        {

            private project projectField;

            private institutuions institutuionsField;

            private investigatorResults investigatorResultsField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://edh.esb.iconplc.com")]
            public project project
            {
                get; set;
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://edh.esb.iconplc.com")]
            public institutuions institutuions
            {
                get; set;
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://edh.esb.iconplc.com")]
            public investigatorResults investigatorResults
            {
                get; set;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://edh.esb.iconplc.com")]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://edh.esb.iconplc.com", IsNullable = false)]
        public partial class project
        {

            private ushort projectNumberField;

            private byte sponsorProtocolNumberField;

            /// <remarks/>
            public ushort projectNumber
            {
                get; set;
            }

            /// <remarks/>
            public byte sponsorProtocolNumber
            {
                get; set;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://edh.esb.iconplc.com")]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://edh.esb.iconplc.com", IsNullable = false)]
        public partial class institutuions
        {

            private institutuionsChecksCompleted checksCompletedField;

            private bool instituteComplianceIssueField;

            private institutuionsDdFindings ddFindingsField;

            /// <remarks/>
            public institutuionsChecksCompleted checksCompleted
            {
                get; set;
            }

            /// <remarks/>
            public bool instituteComplianceIssue
            {
                get; set;
            }

            /// <remarks/>
            public institutuionsDdFindings ddFindings
            {
                get; set;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://edh.esb.iconplc.com")]
        public partial class institutuionsChecksCompleted
        {

            private institutuionsChecksCompletedCheck checkField;

            /// <remarks/>
            public institutuionsChecksCompletedCheck check
            {
                get; set;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://edh.esb.iconplc.com")]
        public partial class institutuionsChecksCompletedCheck
        {

            private string nameField;

            private System.DateTime dateField;

            /// <remarks/>
            public string name
            {
                get; set;
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
            public System.DateTime date
            {
                get; set;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://edh.esb.iconplc.com")]
        public partial class institutuionsDdFindings
        {

            private institutuionsDdFindingsFinding findingField;

            /// <remarks/>
            public institutuionsDdFindingsFinding finding
            {
                get; set;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://edh.esb.iconplc.com")]
        public partial class institutuionsDdFindingsFinding
        {

            private System.DateTime dateField;

            private string typeField;

            private string regulatoryCodeField;

            private string regulatoryDeficiencyField;

            private string worldCheckFindingField;

            private string commentField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
            public System.DateTime date
            {
                get; set;
            }

            /// <remarks/>
            public string type
            {
                get; set;
            }

            /// <remarks/>
            public string regulatoryCode
            {
                get; set;
            }

            /// <remarks/>
            public string regulatoryDeficiency
            {
                get; set;
            }

            /// <remarks/>
            public string worldCheckFinding
            {
                get; set;
            }

            /// <remarks/>
            public string comment
            {
                get; set;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://edh.esb.iconplc.com")]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://edh.esb.iconplc.com", IsNullable = false)]
        public partial class investigatorResults
        {

            private investigatorResultsInvestigatorResult investigatorResultField;

            /// <remarks/>
            public investigatorResultsInvestigatorResult investigatorResult
            {
                get; set;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://edh.esb.iconplc.com")]
        public partial class investigatorResultsInvestigatorResult
        {

            private uint investigatorIdField;

            private uint memberIdField;

            private string firstNameField;

            private string middleNameField;

            private string lastNameField;

            private string ddStatusField;

            private System.DateTime ddCompletedDateField;

            private investigatorResultsInvestigatorResultChecksCompleted checksCompletedField;

            private System.DateTime dmc9002CheckDateField;

            private string dmc9002ExclusionField;

            private investigatorResultsInvestigatorResultDdFindings ddFindingsField;

            /// <remarks/>
            public uint investigatorId
            {
                get; set;
            }

            /// <remarks/>
            public uint memberId
            {
                get; set;
            }

            /// <remarks/>
            public string firstName
            {
                get; set;
            }

            /// <remarks/>
            public string middleName
            {
                get; set;
            }

            /// <remarks/>
            public string lastName
            {
                get; set;
            }

            /// <remarks/>
            public string ddStatus
            {
                get; set;
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
            public System.DateTime ddCompletedDate
            {
                get; set;
            }

            /// <remarks/>
            public investigatorResultsInvestigatorResultChecksCompleted checksCompleted
            {
                get; set;
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
            public System.DateTime dmc9002CheckDate
            {
                get; set;
            }

            /// <remarks/>
            public string dmc9002Exclusion
            {
                get; set;
            }

            /// <remarks/>
            public investigatorResultsInvestigatorResultDdFindings ddFindings
            {
                get; set;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://edh.esb.iconplc.com")]
        public partial class investigatorResultsInvestigatorResultChecksCompleted
        {

            private investigatorResultsInvestigatorResultChecksCompletedCheck checkField;

            /// <remarks/>
            public investigatorResultsInvestigatorResultChecksCompletedCheck check
            {
                get; set;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://edh.esb.iconplc.com")]
        public partial class investigatorResultsInvestigatorResultChecksCompletedCheck
        {

            private string nameField;

            private System.DateTime dateField;

            /// <remarks/>
            public string name
            {
                get; set;
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
            public System.DateTime date
            {
                get; set;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://edh.esb.iconplc.com")]
        public partial class investigatorResultsInvestigatorResultDdFindings
        {

            private investigatorResultsInvestigatorResultDdFindingsFinding findingField;

            /// <remarks/>
            public investigatorResultsInvestigatorResultDdFindingsFinding finding
            {
                get; set;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://edh.esb.iconplc.com")]
        public partial class investigatorResultsInvestigatorResultDdFindingsFinding
        {

            private System.DateTime dateField;

            private string typeField;

            private string regulatoryCodeField;

            private string regulatoryDeficiencyField;

            private string worldCheckFindingField;

            private string commentField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
            public System.DateTime date
            {
                get; set;
            }

            /// <remarks/>
            public string type
            {
                get; set;
            }

            /// <remarks/>
            public string regulatoryCode
            {
                get; set;
            }

            /// <remarks/>
            public string regulatoryDeficiency
            {
                get; set;
            }

            /// <remarks/>
            public string worldCheckFinding
            {
                get; set;
            }

            /// <remarks/>
            public string comment
            {
                get; set;
            }
        }
    }
}
