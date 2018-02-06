using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.ViewModels
{
    public class iSprintResponseModel
    {


        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false)]
        public partial class Envelope
        {

            private EnvelopeHeader headerField;

            private EnvelopeBody bodyField;

            /// <remarks/>
            public EnvelopeHeader Header
            {
                get
                {
                    return this.headerField;
                }
                set
                {
                    this.headerField = value;
                }
            }

            /// <remarks/>
            public EnvelopeBody Body
            {
                get
                {
                    return this.bodyField;
                }
                set
                {
                    this.bodyField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public partial class EnvelopeHeader
        {

            private string messageIDField;

            private ReplyTo replyToField;

            private FaultTo faultToField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.w3.org/2005/08/addressing")]
            public string MessageID
            {
                get
                {
                    return this.messageIDField;
                }
                set
                {
                    this.messageIDField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.w3.org/2005/08/addressing")]
            public ReplyTo ReplyTo
            {
                get
                {
                    return this.replyToField;
                }
                set
                {
                    this.replyToField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.w3.org/2005/08/addressing")]
            public FaultTo FaultTo
            {
                get
                {
                    return this.faultToField;
                }
                set
                {
                    this.faultToField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2005/08/addressing")]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.w3.org/2005/08/addressing", IsNullable = false)]
        public partial class ReplyTo
        {

            private string addressField;

            private ReplyToReferenceParameters referenceParametersField;

            /// <remarks/>
            public string Address
            {
                get
                {
                    return this.addressField;
                }
                set
                {
                    this.addressField = value;
                }
            }

            /// <remarks/>
            public ReplyToReferenceParameters ReferenceParameters
            {
                get
                {
                    return this.referenceParametersField;
                }
                set
                {
                    this.referenceParametersField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2005/08/addressing")]
        public partial class ReplyToReferenceParameters
        {

            private string trackingecidField;

            private uint trackingFlowEventIdField;

            private uint trackingFlowIdField;

            private string trackingCorrelationFlowIdField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("tracking.ecid", Namespace = "http://xmlns.oracle.com/sca/tracking/1.0")]
            public string trackingecid
            {
                get
                {
                    return this.trackingecidField;
                }
                set
                {
                    this.trackingecidField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("tracking.FlowEventId", Namespace = "http://xmlns.oracle.com/sca/tracking/1.0")]
            public uint trackingFlowEventId
            {
                get
                {
                    return this.trackingFlowEventIdField;
                }
                set
                {
                    this.trackingFlowEventIdField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("tracking.FlowId", Namespace = "http://xmlns.oracle.com/sca/tracking/1.0")]
            public uint trackingFlowId
            {
                get
                {
                    return this.trackingFlowIdField;
                }
                set
                {
                    this.trackingFlowIdField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("tracking.CorrelationFlowId", Namespace = "http://xmlns.oracle.com/sca/tracking/1.0")]
            public string trackingCorrelationFlowId
            {
                get
                {
                    return this.trackingCorrelationFlowIdField;
                }
                set
                {
                    this.trackingCorrelationFlowIdField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2005/08/addressing")]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.w3.org/2005/08/addressing", IsNullable = false)]
        public partial class FaultTo
        {

            private string addressField;

            /// <remarks/>
            public string Address
            {
                get
                {
                    return this.addressField;
                }
                set
                {
                    this.addressField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public partial class EnvelopeBody
        {

            private iSprintResponse iSprintResponseField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.iconplc.com/InitiateDDASiSprintFindings/xsd/v3/initiateDDASiSprintFind" +
                "ingsMsg")]
            public iSprintResponse iSprintResponse
            {
                get
                {
                    return this.iSprintResponseField;
                }
                set
                {
                    this.iSprintResponseField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.iconplc.com/InitiateDDASiSprintFindings/xsd/v3/initiateDDASiSprintFind" +
            "ingsMsg")]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.iconplc.com/InitiateDDASiSprintFindings/xsd/v3/initiateDDASiSprintFind" +
            "ingsMsg", IsNullable = false)]
        public partial class iSprintResponse
        {

            private iSprintResponseHeader headerField;

            private bool successField;

            /// <remarks/>
            public iSprintResponseHeader header
            {
                get
                {
                    return this.headerField;
                }
                set
                {
                    this.headerField = value;
                }
            }

            /// <remarks/>
            public bool success
            {
                get
                {
                    return this.successField;
                }
                set
                {
                    this.successField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.iconplc.com/InitiateDDASiSprintFindings/xsd/v3/initiateDDASiSprintFind" +
            "ingsMsg")]
        public partial class iSprintResponseHeader
        {

            private string senderField;

            private System.DateTime timestampField;

            private string message_idField;

            private byte errorCodeField;

            private string errorMessageField;

            /// <remarks/>
            public string sender
            {
                get
                {
                    return this.senderField;
                }
                set
                {
                    this.senderField = value;
                }
            }

            /// <remarks/>
            public System.DateTime timestamp
            {
                get
                {
                    return this.timestampField;
                }
                set
                {
                    this.timestampField = value;
                }
            }

            /// <remarks/>
            public string message_id
            {
                get
                {
                    return this.message_idField;
                }
                set
                {
                    this.message_idField = value;
                }
            }

            /// <remarks/>
            public byte errorCode
            {
                get
                {
                    return this.errorCodeField;
                }
                set
                {
                    this.errorCodeField = value;
                }
            }

            /// <remarks/>
            public string errorMessage
            {
                get
                {
                    return this.errorMessageField;
                }
                set
                {
                    this.errorMessageField = value;
                }
            }
        }

        public partial class DDtoIsprintResponse
        {
            /// <remarks/>
            public bool Success { get; set; }
            public string Message { get; set; }
        }

    }
}
