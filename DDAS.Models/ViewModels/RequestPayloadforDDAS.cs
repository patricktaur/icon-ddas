using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.ViewModels
{
    public class RequestPayloadforDDAS
    {

        /// <remarks/>
        [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
        [System.SerializableAttribute()]
        [System.Diagnostics.DebuggerStepThroughAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
        public partial class ddRequest
        {

            private ddRequestProject projectField;

            private ddRequestInstitute instituteField;

            private ddRequestInvestigator[] investigatorsField;

            /// <remarks/>
            public ddRequestProject project
            {
                get
                {
                    return this.projectField;
                }
                set
                {
                    this.projectField = value;
                }
            }

            /// <remarks/>
            public ddRequestInstitute institute
            {
                get
                {
                    return this.instituteField;
                }
                set
                {
                    this.instituteField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlArrayItemAttribute("investigator", IsNullable = false)]
            public ddRequestInvestigator[] investigators
            {
                get
                {
                    return this.investigatorsField;
                }
                set
                {
                    this.investigatorsField = value;
                }
            }
        }

        /// <remarks/>
        [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
        [System.SerializableAttribute()]
        [System.Diagnostics.DebuggerStepThroughAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class ddRequestProject
        {

            private string projectNumberField;

            private string sponsorProtocolNumberField;

            /// <remarks/>
            public string projectNumber
            {
                get
                {
                    return this.projectNumberField;
                }
                set
                {
                    this.projectNumberField = value;
                }
            }

            /// <remarks/>
            public string sponsorProtocolNumber
            {
                get
                {
                    return this.sponsorProtocolNumberField;
                }
                set
                {
                    this.sponsorProtocolNumberField = value;
                }
            }
        }

        /// <remarks/>
        [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
        [System.SerializableAttribute()]
        [System.Diagnostics.DebuggerStepThroughAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class ddRequestInstitute
        {

            private string nameField;

            private string address1Field;

            private string address2Field;

            private string cityField;

            private string stateProvinceField;

            private string zipCodeField;

            private string countryField;

            /// <remarks/>
            public string name
            {
                get
                {
                    return this.nameField;
                }
                set
                {
                    this.nameField = value;
                }
            }

            /// <remarks/>
            public string address1
            {
                get
                {
                    return this.address1Field;
                }
                set
                {
                    this.address1Field = value;
                }
            }

            /// <remarks/>
            public string address2
            {
                get
                {
                    return this.address2Field;
                }
                set
                {
                    this.address2Field = value;
                }
            }

            /// <remarks/>
            public string city
            {
                get
                {
                    return this.cityField;
                }
                set
                {
                    this.cityField = value;
                }
            }

            /// <remarks/>
            public string stateProvince
            {
                get
                {
                    return this.stateProvinceField;
                }
                set
                {
                    this.stateProvinceField = value;
                }
            }

            /// <remarks/>
            public string zipCode
            {
                get
                {
                    return this.zipCodeField;
                }
                set
                {
                    this.zipCodeField = value;
                }
            }

            /// <remarks/>
            public string country
            {
                get
                {
                    return this.countryField;
                }
                set
                {
                    this.countryField = value;
                }
            }
        }

        /// <remarks/>
        [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
        [System.SerializableAttribute()]
        [System.Diagnostics.DebuggerStepThroughAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class ddRequestInvestigator
        {

            private ddRequestInvestigatorRole roleField;

            private string nameWithQualificationField;

            private string investigatorIdField;

            private string memberIdField;

            private string firstNameField;

            private string middleNameField;

            private string lastNameField;

            private string licenceNumberField;

            /// <remarks/>
            public ddRequestInvestigatorRole role
            {
                get
                {
                    return this.roleField;
                }
                set
                {
                    this.roleField = value;
                }
            }

            /// <remarks/>
            public string nameWithQualification
            {
                get
                {
                    return this.nameWithQualificationField;
                }
                set
                {
                    this.nameWithQualificationField = value;
                }
            }

            /// <remarks/>
            public string investigatorId
            {
                get
                {
                    return this.investigatorIdField;
                }
                set
                {
                    this.investigatorIdField = value;
                }
            }

            /// <remarks/>
            public string memberId
            {
                get
                {
                    return this.memberIdField;
                }
                set
                {
                    this.memberIdField = value;
                }
            }

            /// <remarks/>
            public string firstName
            {
                get
                {
                    return this.firstNameField;
                }
                set
                {
                    this.firstNameField = value;
                }
            }

            /// <remarks/>
            public string middleName
            {
                get
                {
                    return this.middleNameField;
                }
                set
                {
                    this.middleNameField = value;
                }
            }

            /// <remarks/>
            public string lastName
            {
                get
                {
                    return this.lastNameField;
                }
                set
                {
                    this.lastNameField = value;
                }
            }

            /// <remarks/>
            public string licenceNumber
            {
                get
                {
                    return this.licenceNumberField;
                }
                set
                {
                    this.licenceNumberField = value;
                }
            }
        }

        /// <remarks/>
        [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public enum ddRequestInvestigatorRole
        {

            /// <remarks/>
            SubI,

            /// <remarks/>
            PI,
        }


    }
}
