﻿using DDAS.Models;
using DDAS.Models.Entities;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models.Enums;
using DDAS.Models.Interfaces;
using DDAS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using Utilities;
using Utilities.WordTemplate;
using static DDAS.Models.ViewModels.RequestPayloadforDDAS;
using static DDAS.Models.ViewModels.RequestPayloadforiSprint;

namespace DDAS.Services.Search
{
    public class ComplianceFormService : ISearchService, IDisposable
    {
        private IUnitOfWork _UOW;
        private ISearchEngine _SearchEngine;
        private IConfig _config;
        //private CachedSiteScanData _cachedData;

        private const int _MatchCountLowerLimit = 2;
        private int _NumberOfRunningExtractionProcesses = 4;

        public ComplianceFormService(IUnitOfWork uow,
            ISearchEngine SearchEngine,
            IConfig Config)
        {
            _UOW = uow;
            _SearchEngine = SearchEngine;
            _config = Config;
            //_cachedData = new CachedSiteScanData(_UOW);
        }

         //Patrick: 7Jan2018 For use in ExtractDataService
        public IConfig Config
        {
            get
            {
                return _config;
            }
        }

        #region ComplianceFormCreationNUpdates

        //Patrick 27Nov2016 
        public ComplianceForm GetNewComplianceForm(string UserName, string InputSource)
        {
            ComplianceForm newForm = new ComplianceForm();
            newForm.AssignedTo = UserName;
            newForm.SearchStartedOn = DateTime.Now;
            newForm.InputSource = InputSource;

            var review = new Review();
            review.RecId = Guid.NewGuid();
            review.AssigendTo = UserName;
            review.AssignedBy = UserName;
            review.AssignedOn = DateTime.Now;
            review.ReviewerRole = ReviewerRoleEnum.Reviewer;
            newForm.Reviews.Add(review);

            AddMandatorySitesToComplianceForm(newForm);
            return newForm;
        }

        public string GetUserFullName(string UserName)
        {
            var User = _UOW.UserRepository.FindByUserName(UserName);

            return (User == null ? null : User.UserFullName);
        }

        #region ByPradeep
        //need to re-factor, no 
        public ExcelInputRow FillUpExcelInputRowObject(ExcelInputRow RowData,
            List<string> ExcelRow)
        {
            RowData.Role = ExcelRow[0];
            RowData.ProjectNumber = ExcelRow[1];
            RowData.ProjectNumber2 = ExcelRow[2];
            RowData.SponsorProtocolNumber = ExcelRow[3];
            RowData.SponsorProtocolNumber2 = ExcelRow[4];
            RowData.DisplayName = ExcelRow[5];
            RowData.InvestigatorID = ExcelRow[6];
            RowData.MemberID = ExcelRow[7];
            RowData.FirstName = ExcelRow[8];
            RowData.MiddleName = ExcelRow[9];
            RowData.LastName = ExcelRow[10];
            RowData.InstituteName = ExcelRow[11];
            RowData.AddressLine1 = ExcelRow[12];
            RowData.AddressLine2 = ExcelRow[13];
            RowData.City = ExcelRow[14];
            RowData.State = ExcelRow[15];
            RowData.PostalCode = ExcelRow[16];
            RowData.Country = ExcelRow[17];
            RowData.MedicalLicenseNumber = ExcelRow[18];

            return RowData;
        }

        public ExcelInput ReadDataFromExcelFile(string FilePathWithGUID)
        {
            var readExcelData = new ReadUploadedExcelFile();
            //using (var readExcelData = new ReadUploadedExcelFile())
            //{
            var Validations = new List<List<string>>();
                var ValidationMessages = new List<string>();
                var ComplianceFormDetails = new List<List<string>>();

                var excelInput = new ExcelInput();

                int RowIndex = 2;

                while (true)
                {
                    var RowData = new ExcelInputRow();

                    //reading headers and first row
                    var ExcelRow = readExcelData.ReadDataFromExcel(FilePathWithGUID, RowIndex);

                    if (_UOW.DefaultSiteRepository.GetAll().Count() == 0)
                        ExcelRow.Add("cannot find default sites. Add default sites before uploading");
                    //if headers are missing
                    if (ExcelRow.Where(x => x.Contains("cannot find")).Count() > 0)
                    {
                        ExcelRow.ForEach(row =>
                        {
                            RowData.ErrorMessages.Add(row);
                        });
                        RowData.ErrorMessages.Add("errors found");
                        excelInput.ExcelInputRows.Add(RowData);
                        //Validations.Add(ExcelRow);
                        break;
                    }

                    if (ExcelRow[0] == null || ExcelRow[0] == "") //empty file
                        break;

                    FillUpExcelInputRowObject(RowData, ExcelRow);

                    ValidationMessages = ValidateExcelInputs(RowData, RowIndex);

                    if (ValidationMessages.Count > 0)
                    {
                        ValidationMessages.ForEach(message =>
                        {
                            RowData.ErrorMessages.Add(message);
                        });
                        RowData.ErrorMessages.Add("errors found");
                        excelInput.ExcelInputRows.Add(RowData);
                        //Validations.Add(ValidationMessages);
                        break;
                    }

                    excelInput.ExcelInputRows.Add(RowData);

                    if (RowData.Role.ToLower() == "pi")
                    {
                        RowIndex += 1;
                        var SubInvestigator =
                            readExcelData.ReadDataFromExcel(FilePathWithGUID, RowIndex);

                        while (SubInvestigator[0].ToLower() == "sub i")
                        {
                            var SubInvRowData = new ExcelInputRow();

                            FillUpExcelInputRowObject(SubInvRowData, SubInvestigator);

                            ValidationMessages = ValidateExcelInputs(RowData, RowIndex);

                            if (ValidationMessages.Count > 0)
                            {
                                ExcelRow.ForEach(row =>
                                {
                                    RowData.ErrorMessages.Add(row);
                                });
                                RowData.ErrorMessages.Add("errors found");
                                //Validations.Add(ValidationMessages);
                                //break;
                            }
                            excelInput.ExcelInputRows.Add(SubInvRowData);
                            RowIndex += 1;
                            SubInvestigator = readExcelData.ReadDataFromExcel(FilePathWithGUID, RowIndex);
                        }
                    }
                }
            //}
            return excelInput;
        }

        public List<ComplianceForm> ReadUploadedFileData(ExcelInput DataFromExcelFile,
            string UserName,
            string FilePathWithGUID,
            string UploadedFileName)
        {
            var ComplianceForms = new List<ComplianceForm>();

            var InputRows = DataFromExcelFile.ExcelInputRows;

            for (int Index = 0; Index < InputRows.Count; Index++)
            {
                var form = GetNewComplianceForm(UserName, "Batch-Upload");

                form.UploadedFileName = UploadedFileName;
                form.GeneratedFileName =
                    Path.GetFileName(FilePathWithGUID);
                form.ProjectNumber = InputRows[Index].ProjectNumber.Trim();
                form.ProjectNumber2 = InputRows[Index].ProjectNumber2;
                form.SponsorProtocolNumber =
                    InputRows[Index].SponsorProtocolNumber;
                form.SponsorProtocolNumber2 =
                    InputRows[Index].SponsorProtocolNumber2;
                form.Institute = InputRows[Index].InstituteName;
                form.Address = InputRows[Index].Address;
                form.Country = InputRows[Index].Country;

                AddCountrySpecificSites(form);
                AddSponsorSpecificSites(form);

                int InvId = 1;
                if (InputRows[Index].Role.ToLower() == "pi")
                {
                    var Investigator = new InvestigatorSearched();
                    Investigator.Id = InvId;
                    InvId += 1;

                    Investigator.Name = InputRows[Index].DisplayName.Trim();
                    Investigator.FirstName = InputRows[Index].FirstName;
                    Investigator.MiddleName = InputRows[Index].MiddleName;
                    Investigator.LastName = InputRows[Index].LastName;
                    Investigator.Role = InputRows[Index].Role;
                    Investigator.MedicalLiceseNumber =
                        InputRows[Index].MedicalLicenseNumber;
                    Investigator.MemberId = InputRows[Index].MemberID;
                    Investigator.InvestigatorId = InputRows[Index].InvestigatorID;

                    form.InvestigatorDetails.Add(Investigator);

                    int tempIndex = Index + 1; //to add SI's
                    while (tempIndex < InputRows.Count &&
                        InputRows[tempIndex].Role.ToLower() == "sub i")
                    {
                        var Inv = new InvestigatorSearched();
                        Inv.Id = InvId;
                        InvId += 1;

                        Inv.Name = InputRows[tempIndex].DisplayName.Trim();
                        Inv.FirstName = InputRows[tempIndex].FirstName;
                        Inv.MiddleName = InputRows[tempIndex].MiddleName;
                        Inv.LastName = InputRows[tempIndex].LastName;
                        Inv.Role = InputRows[tempIndex].Role;
                        Inv.MedicalLiceseNumber =
                            InputRows[tempIndex].MedicalLicenseNumber;
                        Inv.MemberId = InputRows[tempIndex].MemberID;
                        Inv.InvestigatorId = InputRows[tempIndex].InvestigatorID;

                        form.InvestigatorDetails.Add(Inv);
                        tempIndex += 1;
                    }
                    Index = tempIndex - 1;
                }
                ComplianceForms.Add(form);
            }
            return ComplianceForms;
        }

        #endregion

        public ComplianceForm ImportIsprintData(ddRequest DR)
        {
            var form = GetNewComplianceForm("", "iSprint");

            if (!IsValidProjectNumber(DR.project.projectNumber))
            {
                throw new Exception("change the project number format to '1234/5678", new Exception("Data Validation Failed."));
            }

            form.ProjectNumber = DR.project.projectNumber;
            form.SponsorProtocolNumber = DR.project.sponsorProtocolNumber;
            form.Institute = DR.institute.name;
            form.Address = (
                DR.institute.address1 + " " + 
                DR.institute.address2 + " " + 
                DR.institute.city + " " + 
                DR.institute.stateProvince + " " + 
                DR.institute.zipCode).Replace("  ", " ");

            form.Country = DR.institute.country;

            AddCountrySpecificSites(form);
            AddSponsorSpecificSites(form);

            int InvId = 1;
            int PrincipleInvestigatorCount = 0;

            foreach (ddRequestInvestigator d in DR.investigators)
            {
                var Investigator = new InvestigatorSearched();
                Investigator.Id = InvId;
                InvId += 1;

                Investigator.Name = d.nameWithQualification.Trim();
                Investigator.FirstName = d.firstName == null ? "" : d.firstName.Trim();
                Investigator.MiddleName = d.middleName == null ? "" : d.middleName.Trim();
                Investigator.LastName = d.lastName == null ? "" : d.lastName.Trim();

                if (Investigator.SearchName == null ||
                    Investigator.SearchName == "" ||
                    Investigator.SearchName.Split(' ').Count() <= 1)
                {
                    throw new Exception("Provide atleast two name components(first/middle/last) to carry out the search");
                }

                Investigator.MedicalLiceseNumber = d.licenceNumber;
                Investigator.MemberId = d.memberId;
                Investigator.InvestigatorId = d.investigatorId;

                form.InvestigatorDetails.Add(Investigator);

                if (d.role.ToString().ToLower() == "pi")
                {
                    Investigator.Role = "PI";
                    PrincipleInvestigatorCount += 1;
                }
                else if (d.role.ToString().ToLower() == "subi")
                {
                    Investigator.Role = "Sub I";
                }
            }

            if (PrincipleInvestigatorCount == 0)
            {
                throw new Exception("Principle Investigator not Found. At least one PI must be present in the data.", new Exception("Data Validation Failed."));
            }

            if (PrincipleInvestigatorCount > 1)
            {
                throw new Exception("Principle Investigator cannot be more than one.", new Exception("Data Validation Failed."));
            }

            ScanUpdateComplianceForm(form);

            return form;
        }

        public iSprintResponseModel.DDtoIsprintResponse ExportDataToIsprint(Guid Recid)
        {
            var form = GetComplianceForm(Recid);
            return ExportDataToIsprint(form);
        }

        public iSprintResponseModel.DDtoIsprintResponse ExportDataToIsprint(ComplianceForm form)
        {
            //bool bRetVal = false;
            string sRetval = "";
            var resp = new iSprintResponseModel.DDtoIsprintResponse();

            Envelope oEnvelope = new Envelope();
            EnvelopeBody oBody = new EnvelopeBody();
            DueDiligenceiSprintRequest oDueDiligenceiSprintRequest = 
                new DueDiligenceiSprintRequest();
            CultureInfo ci = CultureInfo.InvariantCulture;

            //<<<Header
            DueDiligenceiSprintRequestHeader oHeader = 
                new DueDiligenceiSprintRequestHeader();
            oHeader.sender = "DDAS";
            oHeader.timestamp = DateTime.Now.ToUniversalTime();
            oHeader.message_id = form.RecId.ToString();

            oDueDiligenceiSprintRequest.header = oHeader;
            //>>>>

            //<<<<<<<<<< DDResults
            DueDiligenceiSprintRequestDDResults oDDresults = 
                new DueDiligenceiSprintRequestDDResults();

            //<<<<<<<<<< project

            project oProject = new project();

            oProject.projectNumber = form.ProjectNumber;
            oProject.sponsorProtocolNumber = form.SponsorProtocolNumber;

            // Project>>>>>>>>>>

            //Add Projects to DDResults
            oDDresults.project = oProject;


            //<<<<<<<<<<< institutions

            institutions oInstitutionsList = new institutions();
            institutionsChecksCompleted oChecksCompleted = 
                new institutionsChecksCompleted();
            institutionsChecksCompletedCheck[] oCheck = 
                new institutionsChecksCompletedCheck[1];

            institutionsChecksCompletedCheck oCheck1 = 
                new institutionsChecksCompletedCheck();
            oCheck1.name = "institution world check";
            oCheck1.date = InstituteWorldCheckCompletedOn(form.SiteSources);

            oCheck[0] = oCheck1;
            oChecksCompleted.check = oCheck;
            oInstitutionsList.checksCompleted = oChecksCompleted;
            oInstitutionsList.instituteComplianceIssue = 
                InstituteComplianceIssue(form.SiteSources);

            // institutions >>>>>>
            //Add institutions to DDResults
            oDDresults.institutions = oInstitutionsList;


            //<<<<<<<<<<<<<<<<< investigatorResults

            var oInvestigatorResultsList = new investigatorResults();

            investigatorResultsInvestigatorResult[] arrInvestigatorResult = 
                new investigatorResultsInvestigatorResult[form.InvestigatorDetails.Count];
            Int32 elem = 0;
            Int32 Index = 0;
            foreach (var InvestigatorDetail in form.InvestigatorDetails)
            {
                var InvestigatorResult = new investigatorResultsInvestigatorResult();

                InvestigatorResult.investigatorId = InvestigatorDetail.InvestigatorId.ToString();
                InvestigatorResult.memberId = InvestigatorDetail.MemberId.ToString();
                InvestigatorResult.firstName = InvestigatorDetail.FirstName.ToString(); //"Srinivas";
                InvestigatorResult.middleName = InvestigatorDetail.MiddleName.ToString();
                InvestigatorResult.lastName = InvestigatorDetail.LastName.ToString();
                InvestigatorResult.ddCompletedDate = InvestigatorDetail.ReviewCompletedOn;

                investigatorResultsInvestigatorResultChecksCompleted oInvestigatorChecksCompleted = 
                    new investigatorResultsInvestigatorResultChecksCompleted();
                investigatorResultsInvestigatorResultChecksCompletedCheck[] oInvestigatorCheck = 
                    new investigatorResultsInvestigatorResultChecksCompletedCheck[2];

                investigatorResultsInvestigatorResultChecksCompletedCheck oInvCheck1 = 
                    new investigatorResultsInvestigatorResultChecksCompletedCheck();

                oInvCheck1.name = "investigator world check";
                oInvCheck1.date = InvestigatorWorldCheckCompletedOn(form.SiteSources);
                oInvestigatorCheck[0] = oInvCheck1;

                investigatorResultsInvestigatorResultChecksCompletedCheck oInvCheck2 = 
                    new investigatorResultsInvestigatorResultChecksCompletedCheck();
                oInvCheck2.name = "foi";
                oInvCheck2.date = InvestigatorDetail.ReviewCompletedOn;
                oInvestigatorCheck[1] = oInvCheck2;

                oInvestigatorChecksCompleted.check = oInvestigatorCheck;
                InvestigatorResult.checksCompleted = oInvestigatorChecksCompleted;

                InvestigatorResult.dmc9002CheckDate = DMCExclusionCompletedOn(form.SiteSources);

                var DMCSite = form.SiteSources.Find(x =>
                x.SiteType == SiteTypeEnum.DMCExclusion);

                if (DMCSite != null)
                    InvestigatorResult.dmc9002Exclusion = DMCSite.IssuesIdentified.ToString();

                var InvestigatorFindings = form.Findings.Where(x => 
                    x.InvestigatorSearchedId == InvestigatorDetail.Id &&
                    x.IsAnIssue)
                    .ToList();

                investigatorResultsInvestigatorResultDdFindings oInvestigatorFindings = 
                    new investigatorResultsInvestigatorResultDdFindings();

                investigatorResultsInvestigatorResultDdFindingsFinding[] oInvFindings =
                    new investigatorResultsInvestigatorResultDdFindingsFinding[InvestigatorFindings.Count()];

                Index = 0;
                foreach(Finding finding in InvestigatorFindings)
                {
                    var Observation = "";
                    investigatorResultsInvestigatorResultDdFindingsFinding oInvestigatorFinding = 
                        new investigatorResultsInvestigatorResultDdFindingsFinding();

                    oInvestigatorFinding.date = finding.DateOfInspection;

                    var Site = form.SiteSources.Find(x => x.SiteEnum == finding.SiteEnum);
                    if (Site != null)
                    {
                        Observation = Site.SiteName;
                        Observation += "_" + finding.Observation;
                    }
                    else
                        Observation = finding.Observation;

                    //Pradeep 08Mar2018
                    //as per the mail from Moore Damien on 01Mar2018, setting up character limit to 32000
                    if (Observation.Length > 32000)
                    {
                        Observation = Observation.Remove(31997);
                        Observation += "...";
                    }

                    oInvestigatorFinding.comment = Observation;

                    oInvFindings[Index] = oInvestigatorFinding;
                    Index += 1;
                }
                oInvestigatorFindings.finding = oInvFindings;
                InvestigatorResult.ddFindings = oInvestigatorFindings;

                arrInvestigatorResult[elem] = InvestigatorResult;

                elem += 1;
            }

            oInvestigatorResultsList.investigatorResult = arrInvestigatorResult;

            //investigatorResults >>>>>>>>>>>>>>>>>

            //Add investigatorResults to DDResults
            oDDresults.investigatorResults = oInvestigatorResultsList;

            //Add DDResults to DueDiligenceiSprintRequest
            oDueDiligenceiSprintRequest.DDResults = oDDresults;

            //Add DueDiligenceiSprintRequest to Body
            oBody.DueDiligenceiSprintRequest = oDueDiligenceiSprintRequest;

            //Add Body to Envelope
            oEnvelope.Body = oBody;

            XmlSerializer xsSubmit = new XmlSerializer(typeof(Envelope));
            string xml = "";

            using (var sww = new Utf8StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    xsSubmit.Serialize(writer, oEnvelope);
                    xml = sww.ToString(); // Your XML
                }
            }

            //'Replace' tags to get required xml output
            xml = xml.Replace("<investigatorResult>", "");
            xml = xml.Replace("</investigatorResult>", "");
            xml = xml.Replace("investigatorResultsInvestigatorResult>", "investigatorResult>");

            xml = xml.Replace("<finding>", "");
            xml = xml.Replace("</finding>", "");
            xml = xml.Replace("<investigatorResultsInvestigatorResultDdFindingsFinding>", "<finding>");
            xml = xml.Replace("</investigatorResultsInvestigatorResultDdFindingsFinding>", "</finding>");

            xml = xml.Replace("<check>", "");
            xml = xml.Replace("</check>", "");
            
            xml = xml.Replace("<institutionsChecksCompletedCheck>", "<check>");
            xml = xml.Replace("</institutionsChecksCompletedCheck>", "</check>");

            xml = xml.Replace("<investigatorResultsInvestigatorResultChecksCompletedCheck>", "<check>");
            xml = xml.Replace("</investigatorResultsInvestigatorResultChecksCompletedCheck>", "</check>");

            var objLog = new LogWSISPRINT();

            objLog.CreatedOn = DateTime.Now;
            objLog.ComplianceFormId = form.RecId;
            objLog.RequestPayload = xml;

            resp = PostToIsprintWebService(xml, out sRetval);

            objLog.Response = sRetval;
            if (resp.Success)
            {
                objLog.Status = "ok";
            }
            else
            {
                objLog.Status = "failed";
            }


            _UOW.LogWSISPRINTRepository.Add(objLog);

            //To be removed. temp code
            sRetval = xml + "</br>Response ==> " + sRetval;

            resp.Message = sRetval;

            return resp;
        }

        public class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }

        public iSprintResponseModel.DDtoIsprintResponse PostToIsprintWebService(string RequestPayload, out string sRetVal)
        {
            var resp = new iSprintResponseModel.DDtoIsprintResponse();
            resp.Success = false;

            //HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://fmsoadev.iconplc.com/soa-infra/services/ClinOps_ClientServices/initiateDDASiSprintFindings/DDASiSprintFindings?WSDL");
            var iSprintURL = System.Configuration.ConfigurationManager.AppSettings["IsprintWS"];
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(iSprintURL);

            req.Method = "POST";
            req.ContentType = "text/xml";

            byte[] byteArray = Encoding.UTF8.GetBytes(RequestPayload);
            req.ContentLength = byteArray.Length;
            Stream dataStream = req.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            try
            {
                HttpWebResponse response = (HttpWebResponse)req.GetResponse();
                Stream dataStreamResponse = response.GetResponseStream();
                if (response.StatusCode == HttpStatusCode.OK) //on success
                {
                    StreamReader SR = new StreamReader(dataStreamResponse, Encoding.UTF8);
                    sRetVal = SR.ReadToEnd();
                    dataStreamResponse.Close();
                    SR.Close();
                    resp.Success = true;

                    //XmlSerializer serializer = new XmlSerializer(typeof(iSprintResponseModel.Envelope));
                    //StringReader rdr = new StringReader(sRetVal);
                    //var stud = (iSprintResponseModel.Envelope)serializer.Deserialize(rdr);

                    //XmlSerializer deserializer = new XmlSerializer(typeof(iSprintResponseModel.Envelope));
                    //TextReader textReader = new StreamReader(dataStreamResponseOut);
                    //var stud = (iSprintResponseModel.Envelope)deserializer.Deserialize(SR);

                    //sRetVal += " </br> iSprintResponse.success => " + stud.Body.iSprintResponse.success;
                    //sRetVal += " </br> iSprintResponse.header.errorMessage => " + stud.Body.iSprintResponse.header.errorMessage;
                    //sRetVal += " </br> Header.MessageID => " + stud.Header.MessageID;
                    //sRetVal += " </br> Header.ReplyTo.ReferenceParameters.trackingecid => " + stud.Header.ReplyTo.ReferenceParameters.trackingecid;
                    //sRetVal += " </br> Header.ReplyTo.ReferenceParameters.trackingFlowId => " + stud.Header.ReplyTo.ReferenceParameters.trackingFlowId;
                    //sRetVal += " </br> Header.ReplyTo.ReferenceParameters.trackingFlowEventId => " + stud.Header.ReplyTo.ReferenceParameters.trackingFlowEventId;
                }
                else
                {
                    sRetVal = "response.StatusCode:" + response.StatusCode;
                }


                response.Close();
                req.Abort();
            }
            catch (Exception ex)
            {
                resp.Message = ex.Message;
                sRetVal = "Request Error:" + ex.Message;

                req.Abort();
            }

            return resp;
        }

        //public String PostToIsprintWebService(string RequestPayload, ref Stream dataStreamResponse)
        //{
        //    string sRetval = "";
        //    //Stream dataStreamResponse = null;

        //    HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://fmsoadev.iconplc.com/soa-infra/services/ClinOps_ClientServices/initiateDDASiSprintFindings/DDASiSprintFindings?WSDL");

        //    req.Method = "POST";
        //    req.ContentType = "text/xml";

        //    byte[] byteArray = Encoding.UTF8.GetBytes(RequestPayload);
        //    req.ContentLength = byteArray.Length;
        //    Stream dataStream = req.GetRequestStream();
        //    dataStream.Write(byteArray, 0, byteArray.Length);
        //    dataStream.Close();

        //    try
        //    {
        //        HttpWebResponse response = (HttpWebResponse)req.GetResponse();
        //        //Stream dataStreamResponse = response.GetResponseStream();
        //        dataStreamResponse = response.GetResponseStream();
        //        StreamReader SR = new StreamReader(dataStreamResponse, Encoding.UTF8);
        //        sRetval = SR.ReadToEnd();
        //        response.Close();
        //        //dataStreamResponse.Close();
        //        SR.Close();
        //        req.Abort();
        //    }
        //    catch
        //    {
        //        req.Abort();
        //    }

        //    return sRetval;
        //    //return dataStreamResponse;
        //} 

        public void UpdateAssignedTo(Guid? RecId, string AssignedBy, string AssignedFrom, string AssignedTo)
        {

            var retValue = _UOW.ComplianceFormRepository.UpdateAssignedTo(RecId.Value, AssignedBy, AssignedFrom, AssignedTo);
            //Move to single update:
            if (retValue == true)
            {
                AddToAssignementHistory(RecId.Value, AssignedBy, AssignedTo);
            }
        }

        public void UpdateAssignedTo(string AssignedBy, AssignComplianceFormsTo AssignComplianceFormsTo)
        {
            string Errors="";
            string Comma = "";
            foreach (PrincipalInvestigator prInv in AssignComplianceFormsTo.PrincipalInvestigators)
            {
                try
                {
                    UpdateAssignedTo(prInv.RecId, AssignedBy, prInv.AssignedTo, AssignComplianceFormsTo.AssignedTo);
                }
                catch (Exception ex)
                {
                    //collect exceptions:
                    Errors = Comma + prInv.Name + " - " + prInv.ProjectNumber +  ex.Message;
                    Comma = ", ";
                }
            }
            if (Errors.Length > 0)
            {
                throw new Exception(Errors);
            }

        }

        //used by Excel File Upload method.
        public ComplianceForm ScanUpdateComplianceForm(ComplianceForm frm)
        {
            //Creates or Updates form
            //Remove Inv + Sites if marked for delete:
            //RemoveDeleteMarkedItemsFromFormCollections(frm);

            AddMissingSearchStatusRecords(frm);
            //Check and Search if required:
            AddMatchingRecords(frm);
            //AddLiveScanFindings(frm, log, ErrorScreenCaptureFolder, siteType);

            //return SaveComplianceForm(frm);

            RollUpSummary(frm);
            //UpdateRollUpSummary(frm.RecId.Value);
            //frm.ExtractionEstimatedCompletion = getEstimatedExtractionCompletion();

            if (frm.RecId != null)
                _UOW.ComplianceFormRepository.UpdateCollection(frm); //Update
            else
                _UOW.ComplianceFormRepository.Add(frm); //Insert

            return frm;
        }

        public bool AddAttachmentsToFindings(ComplianceForm form)
        {
            UpdateComplianceForm(form);
            return true;
        }

        //Used by Client Save button, updates - general section, Inviestigators and Sites collection.
        public ComplianceForm UpdateComplianceForm(ComplianceForm frm)
        {
            //Creates or Updates form
            //Remove Inv + Sites if marked for delete:
            RemoveDeleteMarkedItemsFromFormCollections(frm);

            AddMissingSearchStatusRecords(frm);

            return SaveComplianceForm(frm);
            //if (frm.RecId != null)
            //{
            //    _UOW.ComplianceFormRepository.UpdateComplianceForm(frm.RecId.Value, frm);
            //}
            //else
            //{
            //    _UOW.ComplianceFormRepository.Add(frm);
            //}
            //return frm;

        }

        public bool UpdateComplianceFormNIgnoreIfNotFound(ComplianceForm form)
        {
            //Check if Form exists.
            //Forms can get deleted by other operations
            //Therefore ignore if not found.
            var formToUpdate = _UOW.ComplianceFormRepository.FindById(form.RecId);

            if (formToUpdate == null)
            {
                return false;
            }
            else
            {
                _UOW.ComplianceFormRepository.UpdateCollection(form);
                return true;
            }
        }

        private ComplianceForm SaveComplianceForm(ComplianceForm frm)
        {
            //retain Assignedto //
            if (frm.RecId != null)
            {
                var formFromDB = _UOW.ComplianceFormRepository.FindById(frm.RecId);
                if (formFromDB == null)
                {
                    throw new Exception("Compliance Form not found in data base: " + frm.ProjectNumber);
                }
                //AssignedTo can change after the form is downloaded by client.  
                frm.AssignedTo = formFromDB.AssignedTo;

                //REcords are found only when the form is returned from the client after the extractor saved the comp form.
                var findingsFromLiveExtraction = formFromDB.Findings.Where(f => !frm.Findings.Any(f2 => (f2.Id == f.Id)));
                frm.Findings.AddRange(findingsFromLiveExtraction);
            }

            //Guid added to findings added by client.  Null Guid identifies records that are added by client.
            var findingsAddedByClient = frm.Findings.Where(f => f.Id == null).ToList();
            findingsAddedByClient.ForEach(f => f.Id = Guid.NewGuid());

            RollUpSummary(frm);


            //Patrick: 11May2018 - formWithMaxExtractionEstimatedDate is no longer required. Pending...
            //set frm.ExtractionEstimatedCompletion, will be overwritten when the form is added to the Queue
            if (frm.ExtractionPendingInvestigatorCount > 0 && frm.ExtractionEstimatedCompletion == null)
            {
                //To avoid calling _UOW.ComplianceFormRepository.GetAll()
                //it is likely that the code is not used.
                //However if it is used:
                //var formWithMaxExtractionEstimatedDate = _UOW.ComplianceFormRepository.GetAll().OrderByDescending(o => o.ExtractionEstimatedCompletion).FirstOrDefault();
                //get comp forms of recent 3 months, To Date Add one day to ensure today's forms are included.
                var compFormFilter = new ComplianceFormFilter();
                compFormFilter.SearchedOnFrom = DateTime.Now.AddDays(-90);
                compFormFilter.SearchedOnTo = DateTime.Now.AddDays(1);
                var compForms = _UOW.ComplianceFormRepository.FindComplianceForms(compFormFilter).OrderByDescending(x => x.SearchStartedOn).ToList();
                var formWithMaxExtractionEstimatedDate = compForms.OrderByDescending(o => o.ExtractionEstimatedCompletion).FirstOrDefault();


                if (formWithMaxExtractionEstimatedDate != null)
                {
                    var maxDate = formWithMaxExtractionEstimatedDate.ExtractionEstimatedCompletion;
                    var estimatedCompletionSecs = frm.ExtractionPendingInvestigatorCount * 3 * 15;
                    DateTime baseDate;
                    if (maxDate != null && maxDate > DateTime.Now)
                    {
                        baseDate = maxDate.Value;
                    }
                    else
                    {
                        baseDate = DateTime.Now;
                    }
                    var completionAt = baseDate.AddSeconds(estimatedCompletionSecs);
                    frm.ExtractionEstimatedCompletion = completionAt;
                }
            }

            if (frm.RecId == null)
            {
                _UOW.ComplianceFormRepository.Add(frm);
            }
            else
            {
                _UOW.ComplianceFormRepository.UpdateCollection(frm);
            }

            return frm;
        }

        private void AddToAssignementHistory(Guid ComplianceFormId, string AssignedBy, string AssignedTo)
        {
            var ComplianceForm = _UOW.ComplianceFormRepository.FindById(ComplianceFormId);

            if (ComplianceForm == null)
                throw new Exception("Compliance form could not be found");

            var AssignmentHistory = new AssignmentHistory();
            AssignmentHistory.ComplianceFormId = ComplianceFormId;
            AssignmentHistory.PreviouslyAssignedTo =
                ComplianceForm.AssignedTo;
            AssignmentHistory.AssignedBy = AssignedBy;
            AssignmentHistory.AssignedTo = AssignedTo;
            AssignmentHistory.AssignedOn = DateTime.Now;

            _UOW.AssignmentHistoryRepository.Add(AssignmentHistory);
        }

        public void UpdateExtractionQuePosition(Guid formId, int Position, DateTime ExtractionStartedAt, DateTime ExtractionEstimatedCompletion)
        {
            var form = _UOW.ComplianceFormRepository.FindById(formId);
            form.ExtractionQuePosition = Position;
            form.ExtractionQueStart = ExtractionStartedAt;
            form.ExtractionEstimatedCompletion = ExtractionEstimatedCompletion;
            _UOW.ComplianceFormRepository.UpdateCollection(form);
        }

        //Patrick - 11May2018 - redundant code ?
        private DateTime getEstimatedExtractionCompletion()
        {
            throw new Exception("redundant code");

            //List<ComplianceForm> forms = _UOW.ComplianceFormRepository.GetAll();


            //var formsToScanCount = forms.Where(f => f.InvestigatorDetails.Any(i => i.SitesSearched.Any(
            //  s => s.ExtractionMode == "Live"
            //  && s.ExtractedOn == null
            //  && !(s.StatusEnum == ComplianceFormStatusEnum.ReviewCompletedIssuesIdentified || s.StatusEnum == ComplianceFormStatusEnum.ReviewCompletedIssuesNotIdentified)
            //  ))).ToList().Count();

            //int totCount = 0;
            //var formsForLiveScan = forms.Where(f => f.InvestigatorDetails.Any(i => i.SitesSearched.Any(
            //   s => s.ExtractionMode == "Live"
            //   && s.ExtractedOn == null
            //   && !(s.StatusEnum == ComplianceFormStatusEnum.ReviewCompletedIssuesIdentified || s.StatusEnum == ComplianceFormStatusEnum.ReviewCompletedIssuesNotIdentified)
            //   ))).ToList();
            //foreach (ComplianceForm form in formsForLiveScan)
            //{
            //    foreach (InvestigatorSearched inv in form.InvestigatorDetails.Where(i => i.ExtractionPendingSiteCount > 0))
            //    {
            //        totCount += inv.SitesSearched.Count(
            //           s => s.ExtractionMode == "Live"
            //        && s.ExtractedOn == null
            //         && !(s.StatusEnum == ComplianceFormStatusEnum.ReviewCompletedIssuesIdentified || s.StatusEnum == ComplianceFormStatusEnum.ReviewCompletedIssuesNotIdentified));
            //    }
            //}

            //var estimatedCompletionSecs = (totCount * 30) / _NumberOfRunningExtractionProcesses;
            //if (estimatedCompletionSecs < 120)
            //{
            //    estimatedCompletionSecs = 120;
            //}
            //var completionAt = DateTime.Now.AddSeconds(estimatedCompletionSecs);
            //return completionAt;

        }
        /*

        */

        #region Client side update

        // Called by ComplianceForm Save
        public ComplianceForm UpdateCompFormGeneralNInvestigatorsNOptionalSites(ComplianceForm form)
        {
            var ScanData = new SiteScanData(_UOW);

            if (form.RecId == null)
            {
                AddCountrySpecificSites(form);
                AddSponsorSpecificSites(form);

                AddMissingSearchStatusRecords(form);
                AddMatchingRecords(form);

                RollUpSummary(form);
                _UOW.ComplianceFormRepository.Add(form);
                return form;
            }
            else
            {
                var dbForm = _UOW.ComplianceFormRepository.FindById(form.RecId);
                if (dbForm != null)
                {
                    dbForm.UpdatedOn = DateTime.Now;
                    dbForm.ProjectNumber = form.ProjectNumber;
                    dbForm.ProjectNumber2 = form.ProjectNumber2;
                    dbForm.SponsorProtocolNumber = form.SponsorProtocolNumber;
                    dbForm.SponsorProtocolNumber2 = form.SponsorProtocolNumber2;
                    dbForm.Institute = form.Institute;
                    dbForm.Address = form.Address;
                    dbForm.Country = form.Country;

                    dbForm.InvestigatorDetails.Clear();
                    dbForm.InvestigatorDetails.AddRange(form.InvestigatorDetails);

                    //Remove Optional Sites.
                    //Remove Optional sites not found in client collection
                    dbForm.SiteSources.Clear();
                    dbForm.SiteSources.AddRange(form.SiteSources);

                    //Get SiteDataId for newly added DB Sites:
                    foreach (var site in dbForm.SiteSources)
                    {
                        if (site.SiteDataId == null && site.ExtractionMode.ToLower() == "db")
                        {
                            var sourceSite = _UOW.SiteSourceRepository.FindById(site.SiteId);
                            var siteScan = ScanData.GetSiteScanData(sourceSite.SiteEnum);
                            if (siteScan != null)
                            {
                                site.SiteDataId = siteScan.DataId;
                            }
                        }
                    }

                    dbForm.Findings.Clear();
                    dbForm.Findings.AddRange(form.Findings);

                    dbForm.Reviews.Clear();
                    dbForm.Reviews.AddRange(form.Reviews);

                    //Correct DisplayPosition etc
                    AddMissingSearchStatusRecords(dbForm);
                    RemoveOrphanedSearchStatusRecords(dbForm);
                    //RemoveOrphanedFindings(dbForm);

                    // DisplayPosition, RowNumberInSource nos need adjustment when a site is deleted.

                    AdjustDisplayPositionOfSiteSources(dbForm);
                    CorrectDisplayPositionOfSearchStatusRecords(dbForm);
                    CorrectSiteDisplayPositionInFindings(dbForm);

                    //Check and Search if required:
                    if (dbForm.ExtractionPendingInvestigatorCount > 0)
                    {
                        //dbForm.ExtractionEstimatedCompletion = getEstimatedExtractionCompletion();
                    }
                    AddMatchingRecords(dbForm);

                    RollUpSummary(dbForm);
                    _UOW.ComplianceFormRepository.UpdateCollection(dbForm);
                    return dbForm;
                }
                else
                {
                    return null;
                }
            }

        }

        // Called by Findings.
        public bool UpdateFindings(UpdateFindigs updateFindings)
        {
            //Called by client - Findings page.
            //Retrieves form from db and replaces view related values (ReviewCompleted, corresponding Findings) 

            //get Comp form from db.
            var dbForm = _UOW.ComplianceFormRepository.FindById(updateFindings.FormId);

            if (dbForm != null)
            {
                dbForm.UpdatedOn = DateTime.Now;
                //Set  Review Completed value:
                foreach (InvestigatorSearched Investigator in dbForm.InvestigatorDetails)
                {
                    if (Investigator.Id == updateFindings.InvestigatorSearchedId)
                    {
                        foreach (SiteSearchStatus s in Investigator.SitesSearched)
                        {
                            if (s.SiteSourceId == updateFindings.SiteSourceId)
                            {
                                s.ReviewCompleted = updateFindings.ReviewCompleted;
                            }
                            if (!s.ReviewCompleted)
                                Investigator.ReviewCompletedOn = null;
                        }
                    }
                }

                //Findings
                //add Guid for new records:
                foreach (Finding finding in updateFindings.Findings)
                {
                    if (finding.Id == null)
                    {
                        finding.Id = Guid.NewGuid();
                    }

                    foreach (Comment comment in finding.Comments)
                    {
                        if (comment != null &&
                            comment.CategoryEnum != CommentCategoryEnum.Select &&
                            comment.AddedOn == null)
                            comment.AddedOn = DateTime.Now;
                        if (comment != null &&
                            comment.CorrectedOn == null &&
                            (/*comment.ReviewerCategoryEnum == CommentCategoryEnum.CorrectionCompleted ||*/
                            comment.ReviewerCategoryEnum == CommentCategoryEnum.Accepted))
                        {
                            comment.CorrectedOn = DateTime.Now;
                        }
                    }
                }

                //REvised on 15May2017
                dbForm.Findings.RemoveAll(
                    x => x.InvestigatorSearchedId == updateFindings.InvestigatorSearchedId
                    && x.SiteSourceId == updateFindings.SiteSourceId);
                dbForm.Findings.AddRange(updateFindings.Findings);

                ////***** commented, replaced by above code
                ////Remove manually added Findings (IsMatched = false) from db and add again from the client
                //dbForm.Findings.RemoveAll(
                //    x => x.InvestigatorSearchedId == updateFindings.InvestigatorSearchedId
                //    && x.SiteSourceId == updateFindings.SiteSourceId
                //    && (x.IsMatchedRecord == false || x.MatchCount ==1));

                ////Add all IsMatchedRecord = false OR MatchCount == 1 records from client
                ////MatchCount == 1  are added to comp Form by client.
                //dbForm.Findings.AddRange(updateFindings.Findings.Where(x => x.IsMatchedRecord == false || x.MatchCount == 1));

                //var matchedRecords = updateFindings.Findings.Where(x => x.InvestigatorSearchedId == updateFindings.InvestigatorSearchedId
                //   && x.SiteSourceId == updateFindings.SiteSourceId
                //   && (x.IsMatchedRecord == true && x.MatchCount > 1));

                ////Replace existing generated records (IsMatchedRecord = true) records with records received from client.
                //foreach (var rec in matchedRecords)
                //{
                //    var findingInForm = dbForm.Findings.Find(x => x.Id == rec.Id);
                //    if (findingInForm != null)
                //    {
                //        findingInForm.Observation = rec.Observation;
                //        findingInForm.IsAnIssue = rec.IsAnIssue;
                //        findingInForm.Selected = rec.Selected;
                //     }
                //}

                RollUpSummary(dbForm);

                _UOW.ComplianceFormRepository.UpdateCollection(dbForm);
                return true;
            }
            else
            {
                return false;
            }



            //_UOW.ComplianceFormRepository.UpdateInvestigator(updateFindings.FormId, updateFindings.InvestigatorSearched);
            //_UOW.ComplianceFormRepository.UpdateFindings(updateFindings);


            // return true;
        }

        public bool UpdateInstituteFindings(UpdateInstituteFindings InstitueFindings)
        {
            //Called by client - Findings page.
            //Retrieves form from db and replaces view related values (ReviewCompleted, corresponding Findings) 

            //get Comp form from db.
            var dbForm = _UOW.ComplianceFormRepository.FindById(InstitueFindings.FormId);

            if (dbForm != null)
            {
                dbForm.UpdatedOn = DateTime.Now;

                dbForm.Findings.RemoveAll(x => x.SiteSourceId == InstitueFindings.SiteSourceId);

                //Findings
                //add Guid for new records:
                foreach (Finding finding in InstitueFindings.Findings)
                {
                    if (finding.Id == null)
                    {
                        finding.Id = Guid.NewGuid();
                    }
                }

                dbForm.Findings.AddRange(InstitueFindings.Findings);

                RollUpSummary(dbForm);

                _UOW.ComplianceFormRepository.UpdateCollection(dbForm);
                return true;
            }
            else
            {
                return false;
            }

        }

        public bool UpdateQC(ComplianceForm Form)
        {
            foreach (Finding finding in Form.Findings)
            {
                if (finding.Id == null)
                {
                    finding.Id = Guid.NewGuid();
                }

                foreach (Comment comment in finding.Comments)
                {
                    if (comment != null &&
                        comment.CategoryEnum != CommentCategoryEnum.NotApplicable &&
                        comment.AddedOn == null)
                        comment.AddedOn = DateTime.Now;
                    if (comment != null &&
                        comment.CorrectedOn == null &&
                        (/*comment.ReviewerCategoryEnum == CommentCategoryEnum.CorrectionCompleted ||*/
                        comment.ReviewerCategoryEnum == CommentCategoryEnum.Accepted))
                    {
                        comment.CorrectedOn = DateTime.Now;
                    }
                }
            }

            Form.QCGeneralComments.ForEach(x =>
            {
                if (x.AddedOn == null)
                    x.AddedOn = DateTime.Now;
            });

            Form.QCAttachmentComments.ForEach(x =>
            {
                if (x.AddedOn == null)
                    x.AddedOn = DateTime.Now;
            });

            RollUpSummary(Form);
            _UOW.ComplianceFormRepository.UpdateCollection(Form);

            return true;
        }
        #endregion

        private void AddCountrySpecificSites(ComplianceForm compForm)
        {
            //var test = _UOW.CountryRepository.GetAll();
            //var test1 = test.Where(x => x.CountryName == "");
            //var test2 = test1.ToList();
            if (compForm.Country == null ||
                compForm.Country.Trim().Length == 0)
            {
                return;
            }

            if (_UOW.CountryRepository.GetAll().Count() == 0)
                return;

            var Countries = _UOW.CountryRepository.GetAll().Where(country =>
            country.CountryName.Trim().ToLower() == compForm.Country.Trim().ToLower()); //.ToList();

            var lastDisplayPosition = compForm.SiteSources.Max(x => x.DisplayPosition);

            foreach (Country country in Countries)
            {
                lastDisplayPosition += 1;
                AddToComplianceFormSiteSource(compForm, country, lastDisplayPosition);

                //var SiteToAdd = _UOW.SiteSourceRepository.FindById(country.SiteId);
                //if (SiteToAdd != null)
                //{
                //    var siteSource = new SiteSource();
                //    lastDisplayPosition += 1;
                //    siteSource.DisplayPosition = lastDisplayPosition;
                //    siteSource.SiteName = SiteToAdd.SiteName;
                //    siteSource.SiteShortName = SiteToAdd.SiteShortName;
                //    siteSource.SiteId = SiteToAdd.RecId;
                //    siteSource.SiteEnum = SiteToAdd.SiteEnum;
                //    siteSource.SiteUrl = SiteToAdd.SiteUrl;
                //    siteSource.IsMandatory = SiteToAdd.Mandatory;
                //    siteSource.ExtractionMode = SiteToAdd.ExtractionMode;
                //    //siteSource.ExcludePI = SiteToAdd.ExcludePI;
                //    //siteSource.ExcludeSI = SiteToAdd.ExcludeSI;
                //    compForm.SiteSources.Add(siteSource);
                //}
                //Not found, continue

            }
        }

        private void AddSponsorSpecificSites(ComplianceForm compForm)
        {
            var lastDisplayPosition = compForm.SiteSources.Max(x => x.DisplayPosition);

            if (_UOW.SponsorProtocolRepository.GetAll().Count() == 0)
                return;

            var SponsorProtocols = _UOW.SponsorProtocolRepository.GetAll().Where(
               sponsor => sponsor.SponsorProtocolNumber ==
               compForm.ProjectNumber.Substring(0, 4)); //.ToList();

            foreach (SponsorProtocol sponsorProtocol in SponsorProtocols)
            {
                lastDisplayPosition += 1;
                AddToComplianceFormSiteSource(
                    compForm, sponsorProtocol, lastDisplayPosition);

                //var SiteToAdd = _UOW.SiteSourceRepository.FindById(sponsorProtocol.SiteId);

                //var siteSource = new SiteSource();

                //lastDisplayPosition += 1;
                //siteSource.DisplayPosition = lastDisplayPosition;
                //siteSource.SiteName = SiteToAdd.SiteName;
                //siteSource.SiteShortName = SiteToAdd.SiteShortName;
                //siteSource.SiteId = SiteToAdd.RecId;
                //siteSource.SiteEnum = SiteToAdd.SiteEnum;
                //siteSource.SiteUrl = SiteToAdd.SiteUrl;
                //siteSource.IsMandatory = SiteToAdd.Mandatory;
                //siteSource.ExtractionMode = SiteToAdd.ExtractionMode;
                //siteSource.ExcludePI = SiteToAdd.ExcludePI;
                //siteSource.ExcludeSI = SiteToAdd.ExcludeSI;

                //compForm.SiteSources.Add(siteSource);
            }
        }

        //Patrick 27Nov2016 - check with Pradeep if alt code is available?
        private void AddMandatorySitesToComplianceForm(ComplianceForm compForm)
        {
            //var siteSources = _UOW.SiteSourceRepository.GetAll();

            int SrNo = 0;

            var Sites = _UOW.DefaultSiteRepository.GetAll();

            if (Sites.Count() == 0)
                return;

            var MandatorySites = Sites
                .Where(x => x.IsMandatory == true)
                .OrderBy(x => x.OrderNo).ToList();

            foreach (DefaultSite site in MandatorySites)
            {
                SrNo += 1;
                AddToComplianceFormSiteSource(compForm, site, SrNo);
            }

            var OptionalSites = _UOW.DefaultSiteRepository.GetAll()
                .Where(x => x.IsMandatory == false)
                .OrderBy(x => x.OrderNo).ToList();

            foreach (DefaultSite site in OptionalSites)
            {
                SrNo += 1;
                AddToComplianceFormSiteSource(compForm, site, SrNo);
            }
        }

        private void AddToComplianceFormSiteSource(ComplianceForm compForm, ComplianceFormBaseSite siteToAdd, int SrNo)
        {
            var ScanData = new SiteScanData(_UOW);
            var sourceSite = _UOW.SiteSourceRepository.FindById(siteToAdd.SiteId);
            if (sourceSite == null)
            {
                throw new Exception("Site Source for position " + SrNo + " not found");
            }
            var siteScan = new SiteScan();
            var siteSourceToAdd = new SiteSource();

            siteScan = null;
            if (sourceSite.ExtractionMode.ToLower() == "db")
                //Patrick-Pradeep 02Dec2016 -  Exception is raised in GetSiteScanData therefore will not return null
                siteScan = ScanData.GetSiteScanData(sourceSite.SiteEnum);

            if (siteScan != null)
            {
                siteSourceToAdd.DataExtractedOn = siteScan.DataExtractedOn;
                siteSourceToAdd.SiteSourceUpdatedOn = siteScan.SiteLastUpdatedOn;
                //Patrick Is this required?
                siteSourceToAdd.SiteDataId = siteScan.DataId;
            }
            //else if(siteScan == null && sourceSite.ExtractionMode.ToLower() == "db")
            //{
            ////    siteScan can be null when there's no repository
            ////    created for a site due to extraction error
            ////    which has not been handled... pending
            //}

            siteSourceToAdd.CreatedOn = DateTime.Now;
            //The Id and DisplayPosition are identical when form is created.
            //DisplayPosition may change at the client side.
            siteSourceToAdd.Id = SrNo;
            siteSourceToAdd.DisplayPosition = SrNo;
            siteSourceToAdd.SiteId = siteToAdd.SiteId; //RecId of SiteSourceRepository
            siteSourceToAdd.SiteEnum = sourceSite.SiteEnum;
            siteSourceToAdd.SiteUrl = sourceSite.SiteUrl;
            siteSourceToAdd.SiteName = siteToAdd.Name; // siteToAdd.SiteName; //Name derived from Default / Country / Sponsor list. 
            siteSourceToAdd.SiteShortName = sourceSite.SiteShortName;
            siteSourceToAdd.IsMandatory = siteToAdd.IsMandatory;
            siteSourceToAdd.ExtractionMode = sourceSite.ExtractionMode;
            siteSourceToAdd.SearchAppliesTo = siteToAdd.SearchAppliesTo;
            siteSourceToAdd.SearchAppliesToText = siteToAdd.SearchAppliesTo.ToString().Replace("_", " ");
            siteSourceToAdd.SiteType = siteToAdd.SiteType;
            //siteSourceToAdd.ExcludePI = siteToAdd.ExcludePI;
            //siteSourceToAdd.ExcludeSI = siteToAdd.ExcludeSI;

            siteSourceToAdd.Deleted = false;

            compForm.SiteSources.Add(siteSourceToAdd);
        }

        //Patrick 17Oct2023
        //reducing the loops in the method
        //void UpdateMatchStatus(
        //    IEnumerable<SiteDataItemBase> items,
        //    string InvestigatorName,
        //    int MatchCount = 1)
        //{
        //    InvestigatorName = RemoveExtraCharacters(InvestigatorName);
        //    string[] Name = InvestigatorName.Split(' ');
        //    ////-----------
        //    ////21April2020 - while debugging search result mismatch
        //    ////--Introducing the following code improved the performance
        //    ////-- However the search results did not match with results generated by applicaitn running on ICON prod server
        //    ////Filter:
        //    //var filteredItems = new List<SiteDataItemBase>();
        //    //foreach (var namePart in Name)
        //    //{
        //    //    //var selectedItems = items.Where(p => p.FullName.Contains(namePart)).ToList();
        //    //    var selectedItems = items.Where(p => p.FullName.ToLower().Contains(namePart.Trim().ToLower())).ToList();

        //    //    filteredItems.AddRange(selectedItems);
        //    //}

        //    //var distinctItems = filteredItems.GroupBy(x => x.FullName).Select(y => y.First()).ToList();


        //    ////foreach (SiteDataItemBase item in distinctItems)
        //    ////-----------------
        //    foreach (SiteDataItemBase item in items)
        //    {
        //        string NameComponentSearched = null;
        //        if (item.FullName != null)
        //        {

        //            string FullName = RemoveExtraCharacters(item.FullName);
        //            int Count = 0;
        //            string[] FullNameDB = FullName.Split(' ');

        //            for (int Index = 0; Index < Name.Length; Index++)
        //            {
        //                var temp = Name[Index];
        //                if (temp != null)
        //                {
        //                    if (temp != "")
        //                    {
        //                        for (int Counter = 0; Counter < FullNameDB.Length; Counter++)
        //                        {
        //                            FullNameDB[Counter] = RemoveExtraCharacters(FullNameDB[Counter]);

        //                            bool FullNameComponentIsEqualToNameComponentAndIsNotNull =
        //                            (FullNameDB[Counter] != null &&
        //                            FullNameDB[Counter].ToLower().Equals(Name[Index].ToLower()));

        //                            bool FullNameComponentStartWith = (FullNameDB[Counter].ToLower().
        //                            StartsWith(Name[Index].ToLower()));

        //                            bool IsNameComponentRepeated = (NameComponentSearched != null &&
        //                                Name[Index].ToLower().Equals(NameComponentSearched.ToLower()));

        //                            if (FullNameComponentIsEqualToNameComponentAndIsNotNull &&
        //                                !IsNameComponentRepeated)
        //                            {
        //                                Count += 1;
        //                                NameComponentSearched = Name[Index];
        //                                break;
        //                            }
        //                        }
        //                        NameComponentSearched = Name[Index];
        //                    }
        //                }
        //            }
        //            if (Count > MatchCount)
        //                item.MatchCount = Count;
        //        }
        //    }
        //}


        void UpdateMatchStatus(
            IEnumerable<SiteDataItemBase> items,
            string InvestigatorName,
            int MatchCount = 1)
        {
            var InvestigatorNameParts = GetNameParts(InvestigatorName);
            var testItems = items.Where(item => InvestigatorNameParts.Any(part => item.FullName.ToLower().Contains(part))).ToList();
            foreach (var item in items.Where(item => InvestigatorNameParts.Any(part => item.FullName.ToLower().Contains(part))))
            {
                var FullNameParts = GetNameParts(item.FullName);
                int exactMatchCount = InvestigatorNameParts.Intersect(FullNameParts).Count();
                if (exactMatchCount > MatchCount)
                    item.MatchCount = exactMatchCount;
            }
        }

        string[] GetNameParts(string Name)
        {
            string nonNameCharsPattern = "[.,;]"; // Add other characters inside the square brackets if needed
            string[] NamePart = Name.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return  NamePart
                .Select(part => Regex.Replace(part.ToLowerInvariant(), nonNameCharsPattern, string.Empty))
                .ToArray();
        }


        void AddSingleComponentMatches(
            IEnumerable<SiteDataItemBase> items,
            string InvestigatorName)
        {
            InvestigatorName = RemoveExtraCharacters(InvestigatorName);

            var NameComponents = InvestigatorName.Split(' ');

            int Count = 0;

            for (int Index = 0; Index < NameComponents.Length; Index++)
            {
                foreach (SiteDataItemBase item in items)
                {
                    if (item.FullName.ToLower().Contains(NameComponents[Index].ToLower()))
                    {
                        Count += 1;
                    }
                }
            }
        }

        //Patrick: further refactoring may not require this method.
        public List<MatchedRecord> ConvertToMatchedRecords(IEnumerable<SiteDataItemBase> records)
        {
            List<MatchedRecord> MatchedRecords = new List<MatchedRecord>();

            foreach (SiteDataItemBase record in records)
            {
                var MatchedRecord = new MatchedRecord();
                MatchedRecord.RowNumber = record.RowNumber;
                MatchedRecord.MatchCount = record.MatchCount;
                MatchedRecord.RecordDetails = record.RecordDetails;
                MatchedRecord.Links = record.Links;
                if (record.DateOfInspection.HasValue)
                    MatchedRecord.DateOfInspection = record.DateOfInspection;

                MatchedRecords.Add(MatchedRecord);
            }
            return MatchedRecords;
        }

        public ComplianceForm RollUpSummary(ComplianceForm form)  //previously UpdateFindings
        {
            //int SiteCount = form.SiteSources.Count;
            int FullMatchesFoundInvestigatorCount = 0;
            int PartialMatchesFoundInvestigatorCount = 0;
            int SingleMatchFoundInvestigatorCount = 0;

            int IssuesFoundInvestigatorCount = 0;
            int ReviewCompletedInvestigatorCount = 0;
            int ExtractedInvestigatorCount = 0;
            int ExtractionErrorInvestigatorCount = 0;
            int ExtractionPendingInvestigatorCount = 0;

            //Pradeep 20Dec2016
            form.PartialMatchesFoundInvestigatorCount = 0;
            form.FullMatchesFoundInvestigatorCount = 0;
            form.SingleMatchFoundInvestigatorCount = 0;
            form.IssuesFoundInvestigatorCount = 0;
            form.ReviewCompletedInvestigatorCount = 0;
            form.ExtractedInvestigatorCount = 0;

            var AllSites = form.SiteSources;
            AllSites.ToList().ForEach(x => x.IssuesIdentified = false);
            foreach (InvestigatorSearched Investigator in form.InvestigatorDetails)
            {
                var sitesSearched = Investigator.SitesSearched.Where(x => x.Exclude == false);
                int sitesSearchedCount = sitesSearched.ToList().Count;

                Investigator.TotalIssuesFound = 0;

                int PartialMatchSiteCount = 0;
                int FullMatchSiteCount = 0;
                int SingleMatchSiteCount = 0;
                int IssuesFoundSiteCount = 0;
                int ReviewCompletedSiteCount = 0;
                int ExtractionPendingSiteCount = 0;

                //Investigator.ReviewCompletedOn = null;

                //Pradeep 20Dec2016
                Investigator.Sites_PartialMatchCount = 0;
                Investigator.Sites_FullMatchCount = 0;
                Investigator.Sites_SingleMatchCount = 0;
                Investigator.IssuesFoundSiteCount = 0;
                Investigator.ReviewCompletedSiteCount = 0;

                // foreach (SiteSearchStatus searchStatus in Investigator.SitesSearched.Where(x => x.Exclude == false))
                foreach (SiteSearchStatus searchStatus in sitesSearched)
                {
                    if (
                        //searchStatus.ExtractionMode.ToLower() == "db" 
                        //|| searchStatus.ExtractionMode.ToLower() == "live") && searchStatus.ExtractedOn == null
                        searchStatus.ExtractedOn == null &&
                        searchStatus.ExtractionMode.ToLower() == "live" &&
                        !(searchStatus.StatusEnum ==
                        ComplianceFormStatusEnum.ReviewCompletedIssuesIdentified ||
                        searchStatus.StatusEnum ==
                        ComplianceFormStatusEnum.ReviewCompletedIssuesNotIdentified)
                        )
                    {
                        searchStatus.ExtractionPending = true;
                        ExtractionPendingSiteCount += 1;
                    }
                    searchStatus.IssuesFound = 0; //Pradeep 20Dec2016

                    var ListOfFindings = form.Findings;

                    //var Findings = ListOfFindings.Where(
                    //    x => x.SiteEnum == searchStatus.siteEnum).ToList();

                    var Findings = ListOfFindings.Where(
                        x => x.SiteSourceId == searchStatus.SiteSourceId).ToList();

                    int IssuesFound = 0;
                    int InvId = 0;
                    foreach (Finding Finding in Findings)
                    {
                        if (Finding != null && Finding.IsAnIssue &&
                            Finding.InvestigatorSearchedId == Investigator.Id)
                        {
                            InvId = Finding.InvestigatorSearchedId.Value;
                            IssuesFound += 1;
                        }
                    }
                    searchStatus.IssuesFound = IssuesFound;
                    Investigator.TotalIssuesFound += IssuesFound;

                    //Commented Patrick, 29Apr2017, replaced by code at line: 1177
                    // //var Site = form.SiteSources.Find
                    // //    (x => x.SiteEnum == searchStatus.siteEnum);
                    // var Site = form.SiteSources.Find
                    //     (x => x.SiteId == searchStatus.SiteId);

                    // if (IssuesFound > 0 && Investigator.Id == InvId)
                    //     Site.IssuesIdentified = true;
                    ////else
                    // //    Site.IssuesIdentified = false;

                    //Rollup summary:
                    if (searchStatus.PartialMatchCount > 0)
                    {
                        PartialMatchSiteCount += 1;
                    }
                    if (searchStatus.FullMatchCount > 0)
                    {
                        FullMatchSiteCount += 1;
                    }
                    if (searchStatus.SingleMatchCount > 0)
                    {
                        SingleMatchSiteCount += 1;
                    }
                    if (searchStatus.IssuesFound > 0)
                    {
                        IssuesFoundSiteCount += 1;
                    }
                    if (searchStatus.ReviewCompleted)
                    {
                        ReviewCompletedSiteCount += 1;
                        //Investigator.ReviewCompletedOn = DateTime.Now;
                    }
                }

                if (sitesSearched.Where(x => x.ReviewCompleted).ToList().Count
                    == sitesSearchedCount && Investigator.ReviewCompletedOn == null)
                    Investigator.ReviewCompletedOn = DateTime.Now;

                Investigator.Sites_PartialMatchCount = PartialMatchSiteCount;
                Investigator.Sites_FullMatchCount = FullMatchSiteCount;
                Investigator.Sites_SingleMatchCount = SingleMatchSiteCount;
                Investigator.IssuesFoundSiteCount = IssuesFoundSiteCount;
                Investigator.ReviewCompletedSiteCount = ReviewCompletedSiteCount;
                Investigator.ExtractionPendingSiteCount = ExtractionPendingSiteCount;

                if (Investigator.Sites_PartialMatchCount > 0)
                {
                    PartialMatchesFoundInvestigatorCount += 1;
                }

                if (Investigator.Sites_FullMatchCount > 0)
                {
                    FullMatchesFoundInvestigatorCount += 1;
                }

                if (Investigator.Sites_SingleMatchCount > 0)
                {
                    SingleMatchFoundInvestigatorCount += 1;
                }

                if (Investigator.IssuesFoundSiteCount > 0)
                {
                    IssuesFoundInvestigatorCount += 1;
                }

                if (Investigator.ReviewCompletedSiteCount == sitesSearchedCount)  //SiteCount)
                {
                    ReviewCompletedInvestigatorCount += 1;
                }
                if (Investigator.AddedOn != null)
                {
                    ExtractedInvestigatorCount += 1;
                }

                if (Investigator.ExtractionErrorSiteCount > 0)
                {
                    ExtractionErrorInvestigatorCount += 1;
                }

                if (Investigator.ExtractionPendingSiteCount > 0)
                {
                    ExtractionPendingInvestigatorCount += 1;
                }
            }

            //set DisplayPosition value so that the sorting code is simple on client side angular.
            int pos = 0;

            foreach (var fnd in form.Findings
                .Where(f => f.Selected == true)
                .OrderBy(f => f.InvestigatorSearchedId)
                .ThenBy(f => f.SiteDisplayPosition)
                .ThenByDescending(f => f.DateOfInspection)
                )
            {
                pos += 1;
                fnd.DisplayPosition = pos;
            }

            //Set Isssues find for all sites;
            //.Where(x => x.SearchAppliesTo == SearchAppliesToEnum.Institute)
            foreach (var site in form.SiteSources)
            {
                var InstFindginsCount = form.Findings.Where(
                    x => x.SiteSourceId == site.Id // .DisplayPosition
                    && x.IsAnIssue == true
                    ).Count();
                if (InstFindginsCount > 0)
                {
                    site.IssuesIdentified = true;
                }
                else
                {
                    site.IssuesIdentified = false;
                }
            }
            //Institute sites will not be added under SiteSearchStatus
            var InstituteSiteSources = form.SiteSources.Where(x =>
            x.SearchAppliesTo == SearchAppliesToEnum.Institute)
            .ToList();

            if (InstituteSiteSources.Any(x => x.IssuesIdentified))
                IssuesFoundInvestigatorCount += 1;

            form.PartialMatchesFoundInvestigatorCount = PartialMatchesFoundInvestigatorCount;
            form.FullMatchesFoundInvestigatorCount = FullMatchesFoundInvestigatorCount;
            form.SingleMatchFoundInvestigatorCount = SingleMatchFoundInvestigatorCount;
            form.IssuesFoundInvestigatorCount = IssuesFoundInvestigatorCount;
            form.ReviewCompletedInvestigatorCount = ReviewCompletedInvestigatorCount;
            form.ExtractedInvestigatorCount = ExtractedInvestigatorCount;
            form.ExtractionErrorInvestigatorCount = ExtractionErrorInvestigatorCount;
            form.ExtractionPendingInvestigatorCount = ExtractionPendingInvestigatorCount;

            if (form.Reviews.FirstOrDefault() == null)
                throw new Exception("Review cannot be empty");

            UpdateReviewStatus(form.Reviews.First(),
                form.IsReviewCompleted);

            return form;
        }

        private void UpdateReviewStatus(Review review, bool IsReviewCompleted)
        {
            if (!IsReviewCompleted)
            {
                if (review.Status == ReviewStatusEnum.ReviewInProgress)
                {
                    //do nothing. status is already ReviewInProgress
                }
                else
                    review.Status = ReviewStatusEnum.ReviewInProgress;
            }
            else
            {
                if (review.Status == ReviewStatusEnum.ReviewCompleted)
                {
                    //do nothing. status is already ReviewInProgress
                }
                else
                    review.Status = ReviewStatusEnum.ReviewCompleted;
            }
        }

        public bool UpdateRollUpSummary(Guid formId)
        {
            var form = _UOW.ComplianceFormRepository.FindById(formId);
            RollUpSummary(form);
            //_UOW.ComplianceFormRepository.UpdateComplianceForm(formId, form);
            _UOW.ComplianceFormRepository.UpdateCollection(form);
            return true;
        }

        public void AddMatchingRecords(ComplianceForm frm)
        {
            int InvestigatorId = 1;
            frm.ExtractedOn = DateTime.Now; //last extracted on
            foreach (InvestigatorSearched inv in frm.InvestigatorDetails)
            {
                var InvestigatorName = inv.SearchName;

                var ComponentsInInvestigatorName =
                    InvestigatorName.Split(' ').Count();

                //inv.AddedOn = DateTime.Now;
                inv.HasExtractionError = true; // until set to false.
                inv.IssuesFoundSiteCount = 0;
                inv.ReviewCompletedCount = 0;

                bool HasExtractionError = false; //for rollup value for Investigator
                int ExtractionErrorSiteCount = 0;
                //foreach (SiteSource siteSource in frm.SiteSources)
                foreach (SiteSource siteSource in frm.SiteSources.Where(
                    x => x.ExtractionMode.ToLower() == "db"
                    && x.SearchAppliesTo != SearchAppliesToEnum.Institute))
                {
                    SiteSearchStatus searchStatus = null;

                    //if (inv.SitesSearched != null)
                    //    searchStatus =
                    //        inv.SitesSearched.Find(x => x.siteEnum == siteSource.SiteEnum);

                    if (inv.SitesSearched != null)
                        searchStatus =
                            inv.SitesSearched.Find(x => x.DisplayPosition == siteSource.DisplayPosition);

                    bool searchRequired = false;

                    if (searchStatus == null)
                    {
                        //SearchStatus records must be added to each Investigator before calling AddMatchingRecords
                        throw new Exception("Coding Error: Search Status is not added to Project-Investigator:"
                            + frm.ProjectNumber + "-" + inv.Name);
                    }

                    if (!(searchStatus.StatusEnum == ComplianceFormStatusEnum.ReviewCompletedIssuesIdentified || searchStatus.StatusEnum == ComplianceFormStatusEnum.ReviewCompletedIssuesNotIdentified))
                    {
                        if (searchStatus.HasExtractionError == true || searchStatus.ExtractedOn == null)
                        {
                            searchRequired = true;
                        }
                    }

                    //10Feb2017-todo: siteSource.ExtractionMode.ToLower() = "db") //get db Sites only, live sites are extracted through windows service
                    if (searchRequired == true)
                    {
                        try
                        {
                            inv.AddedOn = DateTime.Now;
                            //clear previously added matching records.
                            //frm.Findings.RemoveAll(x => (x.InvestigatorSearchedId == inv.Id) && (x.SiteEnum == searchStatus.siteEnum) && x.IsMatchedRecord == true);
                            frm.Findings.RemoveAll(x => (x.InvestigatorSearchedId == inv.Id) && (x.DisplayPosition == searchStatus.DisplayPosition) && x.IsMatchedRecord == true);

                            DateTime? SiteLastUpdatedOn = null;

                            if (siteSource.SiteDataId == null)
                                throw new Exception(
                                    "SiteDataId is null for: " +
                                    siteSource.SiteEnum);

                            var MatchedRecords = GetMatchedRecords(
                                siteSource, InvestigatorName,
                                ComponentsInInvestigatorName, out SiteLastUpdatedOn);

                            siteSource.SiteSourceUpdatedOn = SiteLastUpdatedOn;

                            GetFullAndPartialMatchCount(
                                MatchedRecords,
                                searchStatus,
                                ComponentsInInvestigatorName);

                            inv.Sites_PartialMatchCount +=
                                searchStatus.PartialMatchCount;
                            inv.Sites_FullMatchCount +=
                                searchStatus.FullMatchCount;

                            var SingleMatchCount = GetSingleComponentMatches(
                                siteSource.SiteEnum,
                                siteSource.SiteDataId,
                                InvestigatorName.Split(' '));

                            if (SingleMatchCount >= 0)
                                searchStatus.SingleMatchCount = SingleMatchCount;

                            //inv.Id = InvestigatorId;

                            //Pradeep 13Apr2017 Major change. 
                            //Do not add findings at this level

                            //To-Do: convert matchedRecords to Findings
                            foreach (MatchedRecord rec in MatchedRecords)
                            {
                                var finding = new Finding();
                                finding.Id = Guid.NewGuid();
                                finding.InvestigatorSearchedId = inv.Id;
                                finding.SiteSourceId = siteSource.Id; // siteSource.DisplayPosition;
                                finding.SiteDisplayPosition = siteSource.DisplayPosition;
                                //required??
                                finding.SiteId = siteSource.SiteId;
                                finding.SiteEnum = siteSource.SiteEnum;
                                finding.IsFullMatch = rec.IsFullMatch;
                                finding.MatchCount = rec.MatchCount;
                                finding.RecordDetails = rec.RecordDetails;
                                finding.RowNumberInSource = rec.RowNumber;
                                finding.IsMatchedRecord = true;
                                if (rec.DateOfInspection.HasValue)
                                {
                                    finding.DateOfInspection = rec.DateOfInspection;
                                }
                                finding.InvestigatorName = inv.Name;
                                finding.Links = rec.Links;

                                frm.Findings.Add(finding);
                            }

                            //Review:
                            //siteSource.SiteSourceUpdatedOn' is the date of update at the time of creation of CompForm
                            //
                            //replace  '= siteSource.SiteSourceUpdatedOn' SiteSourceUpdatedOn at the time of data extraction
                            searchStatus.SiteSourceUpdatedOn = siteSource.SiteSourceUpdatedOn;
                            searchStatus.HasExtractionError = false;
                            searchStatus.ExtractionErrorMessage = "";

                            searchStatus.ExtractedOn = siteSource.DataExtractedOn;
                            //searchStatus.SiteDataId = siteSource.SiteDataId.ToString();
                            if (MatchedRecords.Count == 0 && SingleMatchCount == 0)
                            {
                                searchStatus.ReviewCompleted = true;
                            }
                            inv.SearchCompletedOn = DateTime.Now;
                            //ListOfSiteSearchStatus.Add(searchStatus);
                        }
                        catch (Exception ex)
                        {
                            HasExtractionError = true;  //for rollup to investigator
                            ExtractionErrorSiteCount += 1;
                            searchStatus.HasExtractionError = true;
                            searchStatus.ExtractionErrorMessage =
                                "search not successful - " + ex.Message;
                            // Log -- ex.Message + ex.InnerException.Message
                        }
                        finally
                        {
                            frm.UpdatedOn = DateTime.Now;
                        }
                    }
                }
                inv.ExtractionErrorSiteCount = ExtractionErrorSiteCount;
                inv.HasExtractionError = HasExtractionError;
                //inv.SitesSearched = ListOfSiteSearchStatus;
                InvestigatorId += 1;
            }
            AddOrUpdateReviewStatus(frm); //pradeep 27Dec2017
        }

        private void AddOrUpdateReviewStatus(ComplianceForm form)
        {
            if (form.Reviews.Count == 0)
            {
                var review = new Review();
                review.RecId = Guid.NewGuid();
                review.AssigendTo = form.AssignedTo;
                review.AssignedBy = form.AssignedTo;
                review.AssignedOn = DateTime.Now;
                review.ReviewerRole = ReviewerRoleEnum.Reviewer;
                review.Status = ReviewStatusEnum.SearchCompleted;
                form.Reviews.Add(review);
            }
            else
            {
                //...
            }
        }

        public void AddLiveScanFindings(ComplianceForm frm)
        {
            throw new NotImplementedException();

            ////Recheck code when uncommenting:

            //string siteType = "live";

            ////int InvestigatorId = 1;
            //frm.ExtractedOn = DateTime.Now; //last extracted on
            //foreach (InvestigatorSearched inv in frm.InvestigatorDetails)
            //{
            //    //var ListOfSiteSearchStatus = new List<SiteSearchStatus>();

            //    var InvestigatorName = RemoveExtraCharacters(inv.Name);

            //    var ComponentsInInvestigatorName =
            //        InvestigatorName.Trim().Split(' ').Count();

            //    //inv.ExtractedOn = DateTime.Now;
            //    //inv.HasExtractionError = true; // until set to false.
            //    //inv.IssuesFoundSiteCount = 0;
            //    //inv.ReviewCompletedCount = 0;

            //    //bool HasExtractionError = false; //for rollup value for Investigator
            //    //int ExtractionErrorSiteCount = 0;
            //    foreach (SiteSource siteSource in frm.SiteSources)
            //    {
            //        SiteSearchStatus searchStatus = null;

            //        if (inv.SitesSearched != null)
            //            searchStatus =
            //                inv.SitesSearched.Find(x => x.siteEnum == siteSource.SiteEnum);

            //        bool searchRequired = false;

            //        if (searchStatus == null)
            //        {
            //            //SearchStatus records must be added to each Investigator before calling AddMatchingRecords
            //            throw new Exception("Coding Error: Search Status is not added to Project-Investigator:"
            //                + frm.ProjectNumber + "-" + inv.Name);
            //        }

            //        if (!(searchStatus.StatusEnum == ComplianceFormStatusEnum.ReviewCompletedIssuesIdentified || searchStatus.StatusEnum == ComplianceFormStatusEnum.ReviewCompletedIssuesNotIdentified))
            //        {
            //            if (searchStatus.HasExtractionError == true || searchStatus.ExtractedOn == null)
            //            {
            //                searchRequired = true;
            //            }
            //        }

            //        //10Feb2017-todo: siteSource.ExtractionMode.ToLower() = "db") //get db Sites only, live sites are extracted through windows service
            //        if (searchRequired == true &&
            //            siteSource.ExtractionMode.ToLower() == siteType)
            //        {
            //            try
            //            {
            //                //clear previously added matching records.
            //                frm.Findings.RemoveAll(x => (x.InvestigatorSearchedId == inv.Id) && (x.SiteEnum == searchStatus.siteEnum) && x.IsMatchedRecord == true);

            //                //new:
            //                DateTime? SiteLastUpdatedOn1 = null;

            //                    var findings = getFindings(siteSource, InvestigatorName, inv.Id, log,
            //                   ErrorScreenCaptureFolder, ComponentsInInvestigatorName,
            //                   out SiteLastUpdatedOn1);

            //                if (findings.Count > 0)
            //                {
            //                    _UOW.ComplianceFormRepository.AddFindings(frm.RecId.Value, findings);
            //                }


            //                DateTime? SiteLastUpdatedOn = null;


            //                siteSource.SiteSourceUpdatedOn = SiteLastUpdatedOn;

            //                //searchStatus - full / partial match count
            //                //GetFullAndPartialMatchCount(MatchedRecords, searchStatus, ComponentsInInvestigatorName);
            //                //record.MatchCount >= ComponentsInInvestigatorName
            //                searchStatus.FullMatchCount = findings.Where(x => x.MatchCount >= ComponentsInInvestigatorName).Count();
            //                searchStatus.PartialMatchCount = findings.Count - searchStatus.FullMatchCount;



            //                searchStatus.SiteSourceUpdatedOn = siteSource.SiteSourceUpdatedOn;
            //                searchStatus.HasExtractionError = false;
            //                searchStatus.ExtractionErrorMessage = "";
            //                searchStatus.ExtractionPending = false;
            //                searchStatus.ExtractedOn = DateTime.Now;

            //                if (findings.Count == 0)
            //                {
            //                    searchStatus.ReviewCompleted = true;
            //                }

            //            }
            //            catch (Exception ex)
            //            {
            //                //HasExtractionError = true;  //for rollup to investigator
            //                //ExtractionErrorSiteCount += 1;
            //                searchStatus.ExtractionPending = true;
            //                searchStatus.ExtractedOn = null;

            //                searchStatus.HasExtractionError = true;
            //                searchStatus.ExtractionErrorMessage =
            //                    "Data Extraction not successful - " + ex.Message;
            //                log.WriteLog("Data extraction failed. Details: " + ex.Message);
            //                // Log -- ex.Message + ex.InnerException.Message
            //            }
            //            finally
            //            {

            //            }
            //            UpdateSearchStatus(frm.RecId.Value, inv.Id, searchStatus);
            //        }
            //    }

            //    //handled
            //    //inv.ExtractionErrorSiteCount = ExtractionErrorSiteCount;
            //    //inv.HasExtractionError = HasExtractionError;
            //    //inv.SitesSearched = ListOfSiteSearchStatus;
            //    //InvestigatorId += 1;

            //}
        }

        private List<Finding> getFindings(
            SiteSource siteSource,
            string InvestigatorName,
            int InvestigatorId,
            int ComponentsInInvestigatorName,
            out DateTime? SiteLastUpdatedOn)
        {
            throw new NotImplementedException();

            //Recheck code when uncommenting:

            //var retFindings = new List<Finding>(); 
            //var MatchedRecords = GetMatchedRecords(siteSource, InvestigatorName, log,
            //                   ErrorScreenCaptureFolder, ComponentsInInvestigatorName,
            //                   out SiteLastUpdatedOn);

            ////To-Do: convert matchedRecords to Findings
            //foreach (MatchedRecord rec in MatchedRecords)
            //{
            //    var finding = new Finding();
            //    finding.Id = Guid.NewGuid();
            //    finding.MatchCount = rec.MatchCount;
            //    finding.InvestigatorSearchedId = InvestigatorId;
            //    //finding.SiteSourceId = siteSource.DisplayPosition;
            //    finding.SiteSourceId = siteSource.Id;
            //    finding.SiteDisplayPosition = siteSource.DisplayPosition;
            //    finding.SiteEnum = siteSource.SiteEnum;

            //    finding.RecordDetails = rec.RecordDetails;
            //    finding.RowNumberInSource = rec.RowNumber;

            //    finding.IsMatchedRecord = true;
            //    if (rec.DateOfInspection.HasValue)
            //    {
            //        finding.DateOfInspection = rec.DateOfInspection;
            //    }

            //    finding.InvestigatorName = InvestigatorName;
            //    finding.Links = rec.Links;

            //    retFindings.Add(finding);
            //}
            //return retFindings;
        }

        private bool UpdateSearchStatus(Guid formId, int InvestigatorId, SiteSearchStatus siteStatus)
        {
            throw new NotImplementedException();

            ////Recheck code when uncommenting:

            //var dbForm = _UOW.ComplianceFormRepository.FindById(formId);

            //foreach (InvestigatorSearched inv in dbForm.InvestigatorDetails)
            //{
            //    if (inv.Id == InvestigatyorId)
            //    {
            //        foreach (SiteSearchStatus ss in inv.SitesSearched)
            //        {
            //            if (ss.siteEnum == siteStatus.siteEnum)
            //            {
            //                //replace values
            //                ss.DisplayPosition = siteStatus.DisplayPosition;
            //                ss.ExtractedOn = siteStatus.ExtractedOn;
            //                ss.HasExtractionError = siteStatus.HasExtractionError;
            //                ss.ExtractionErrorMessage = siteStatus.ExtractionErrorMessage;
            //                //ss.ExtractionPending ??
            //                ss.FullMatchCount = siteStatus.FullMatchCount;
            //                //ss.HasExtractionError
            //                //ss.IssuesFound
            //                ss.PartialMatchCount = siteStatus.PartialMatchCount;
            //                ss.ReviewCompleted = siteStatus.ReviewCompleted;
            //                ss.SiteSourceUpdatedOn = siteStatus.SiteSourceUpdatedOn;
            //                ss.ExtractionPending = siteStatus.ExtractionPending;
            //                _UOW.ComplianceFormRepository.UpdateComplianceForm(formId, dbForm);
            //                break;
            //            }
            //        }
            //    }


            //}

            //return true;
        }

        #region Save related

        private void RemoveDeleteMarkedItemsFromFormCollections(ComplianceForm frm)
        {

            frm.InvestigatorDetails.RemoveAll(x => x.Deleted == true);

            //Remove Delete Site's SiteSearchStatus from each remaining Investigator
            var sitesRemoved = frm.SiteSources.Where(x => x.Deleted == true);
            foreach (SiteSource site in sitesRemoved)
            {
                foreach (InvestigatorSearched inv in frm.InvestigatorDetails)
                {
                    inv.SitesSearched.RemoveAll(x => x.siteEnum == site.SiteEnum);
                }
            }
            frm.SiteSources.RemoveAll(x => x.Deleted == true);

            //findings -  Delete Source = site and selected = false, manual is set on the client side
            frm.Findings.RemoveAll(x => x.IsMatchedRecord == false && x.Selected == false);
        }

        private void AddMissingSearchStatusRecords(ComplianceForm frm)
        {
            //Adds SearchStatus Records if not found for an investigator.
            //This will happen when new investigator is added or new sites are added from the client side

            foreach (InvestigatorSearched inv in frm.InvestigatorDetails)
            {
                foreach (SiteSource site in frm.SiteSources.Where(x => x.SearchAppliesTo != SearchAppliesToEnum.Institute))
                {
                    //SiteSearchStatus searchStatus = inv.SitesSearched.Find(x => x.siteEnum == site.SiteEnum);
                    //SiteSearchStatus searchStatus = inv.SitesSearched.Find(x => x.SiteId== site.SiteId);
                    //SiteSearchStatus searchStatus = inv.SitesSearched.Find(x => x.DisplayPosition == site.DisplayPosition);
                    SiteSearchStatus searchStatus = inv.SitesSearched.Find(x => x.SiteSourceId == site.Id);
                    if (searchStatus == null)
                    {
                        searchStatus = new SiteSearchStatus();
                        searchStatus.SiteSourceId = site.Id;
                        searchStatus.SiteId = site.SiteId;
                        searchStatus.siteEnum = site.SiteEnum;
                        searchStatus.SiteName = site.SiteName;
                        searchStatus.SiteUrl = site.SiteUrl;
                        searchStatus.DisplayPosition = site.DisplayPosition;
                        searchStatus.ExtractionMode = site.ExtractionMode;

                        if (site.SearchAppliesTo == SearchAppliesToEnum.PIs && inv.Role.ToLower() == "sub i")
                        //if (site.ExcludeSI == true && inv.Role.ToLower() == "sub")
                        {
                            searchStatus.Exclude = true;
                            //searchStatus.ReviewCompleted = true;
                        }

                        //if (site.ExtractionMode.ToLower() == "manual" && //requirement of ICON 23-Mar-2017
                        //    inv.Role.ToLower() == "sub i") //requirement of ICON 04-Oct-2017
                        //{
                        //    searchStatus.ReviewCompleted = true;
                        //}

                        inv.SitesSearched.Add(searchStatus);
                    }
                }
            }
        }

        //When Site is removed, (Optional site), the corresponding SearchStatus of all Investigators become orphanced.
        private void RemoveOrphanedSearchStatusRecords(ComplianceForm frm)
        {
            foreach (InvestigatorSearched inv in frm.InvestigatorDetails)
            {
                foreach (SiteSearchStatus siteSearchStatus in inv.SitesSearched)
                {
                    //var siteSource = frm.SiteSources.Find(x => x.SiteEnum == siteSearchStatus.siteEnum);
                    var siteSource = frm.SiteSources.Find(x => x.Id == siteSearchStatus.SiteSourceId);
                    if (siteSource == null) // Site Source not available, remove 
                    {
                        siteSearchStatus.DisplayPosition = -1;
                    }
                }
                inv.SitesSearched.RemoveAll(x => x.DisplayPosition == -1);
            }
        }

        private void AdjustDisplayPositionOfSiteSources(ComplianceForm frm)
        {
            //When intermediate sites are deleted.
            var SrNo = 0;
            foreach (SiteSource site in frm.SiteSources)
            {
                SrNo += 1;
                site.DisplayPosition = SrNo;
            }
        }

        private void CorrectDisplayPositionOfSearchStatusRecords(ComplianceForm frm)
        {
            foreach (InvestigatorSearched inv in frm.InvestigatorDetails)
            {
                foreach (SiteSearchStatus siteSearchStatus in inv.SitesSearched)
                {
                    //siteSearchStatus.DisplayPosition = frm.SiteSources.Find(x => x.SiteEnum == siteSearchStatus.siteEnum).DisplayPosition;

                    siteSearchStatus.DisplayPosition = frm.SiteSources.Find(x => x.Id == siteSearchStatus.SiteSourceId).DisplayPosition;

                }
            }
        }

        private void CorrectSiteDisplayPositionInFindings(ComplianceForm frm)
        {
            //foreach (Finding finding in frm.Findings.Where(x => (x.SiteId != null)))
            foreach (Finding finding in frm.Findings)
            {
                finding.SiteDisplayPosition = frm.SiteSources.Find(x => x.Id == finding.SiteSourceId).DisplayPosition;
            }

        }

        //When Site is removed, (Optional site), the Findings for those sites become orphanced.
        private void RemoveOrphanedFindings(ComplianceForm frm)
        {
            //list.RemoveAll( item => !list2.Contains(item));


            foreach (Finding finding in frm.Findings)
            {
                var siteSource = frm.SiteSources.Find(x => x.SiteEnum == finding.SiteEnum);
                if (siteSource == null) // Site Source not available, remove 
                {
                    if (siteSource == null) // Site Source not available, remove 
                    {
                        finding.MatchCount = -1;  //used as a flag to delete records
                    }
                }
            }
            frm.Findings.RemoveAll(x => x.MatchCount == -1);
        }
        #endregion

        private List<MatchedRecord> GetMatchedRecords(
            SiteSource site, string NameToSearch,
            int ComponentsInInvestigatorName, out DateTime? SiteLastUpdatedOn)
        {
            switch (site.SiteEnum)
            {
                case SiteEnum.FDADebarPage:
                    return GetFDADebarPageMatchedRecords(
                        site.SiteDataId,
                        NameToSearch,
                        ComponentsInInvestigatorName,
                        out SiteLastUpdatedOn);

                case SiteEnum.ClinicalInvestigatorInspectionPage:
                    return GetClinicalInvestigatorPageMatchedRecords(
                        site.SiteDataId,
                        NameToSearch,
                        ComponentsInInvestigatorName,
                        out SiteLastUpdatedOn);

                case SiteEnum.FDAWarningLettersPage:
                    return GetFDAWarningLettersPageMatchedRecords(
                        site.SiteDataId, NameToSearch,
                        ComponentsInInvestigatorName,
                        out SiteLastUpdatedOn);

                case SiteEnum.ERRProposalToDebarPage:
                    return GetERRProposalToDebarPageMatchedRecords(
                        site.SiteDataId,
                        NameToSearch,
                        ComponentsInInvestigatorName,
                        out SiteLastUpdatedOn);

                case SiteEnum.AdequateAssuranceListPage:
                    return GetAdequateAssurancePageMatchedRecords(
                        site.SiteDataId,
                        NameToSearch,
                        ComponentsInInvestigatorName,
                        out SiteLastUpdatedOn);

                case SiteEnum.ClinicalInvestigatorDisqualificationPage:
                    return GetClinicalInvestigatorDisqualificationPageMatchedRecords(
                        site.SiteDataId,
                        NameToSearch,
                        ComponentsInInvestigatorName,
                        out SiteLastUpdatedOn);

                case SiteEnum.CBERClinicalInvestigatorInspectionPage:
                    return GetCBERClinicalInvestigatorPageMatchedRecords(
                        site.SiteDataId,
                        NameToSearch,
                        ComponentsInInvestigatorName,
                        out SiteLastUpdatedOn);

                case SiteEnum.PHSAdministrativeActionListingPage:
                    return GetPHSAdministrativeActionPageMatchedRecords(
                        site.SiteDataId,
                        NameToSearch,
                        ComponentsInInvestigatorName,
                        out SiteLastUpdatedOn);

                case SiteEnum.ExclusionDatabaseSearchPage:
                    return GetExclusionDatabasePageMatchedRecords(
                        site.SiteDataId,
                        NameToSearch,
                        ComponentsInInvestigatorName,
                        out SiteLastUpdatedOn);

                case SiteEnum.CorporateIntegrityAgreementsListPage:
                    return GetCIAPageMatchedRecords(
                        site.SiteDataId,
                        NameToSearch,
                        ComponentsInInvestigatorName,
                        out SiteLastUpdatedOn);

                case SiteEnum.SystemForAwardManagementPage:
                    return GetSAMPageMatchedRecords(
                        site.SiteDataId,
                        NameToSearch,
                        ComponentsInInvestigatorName,
                        out SiteLastUpdatedOn);

                case SiteEnum.SpeciallyDesignedNationalsListPage:
                    return GetSDNPageMatchedRecords(
                        site.SiteDataId,
                        NameToSearch,
                        ComponentsInInvestigatorName,
                        out SiteLastUpdatedOn);

                default: throw new Exception("Invalid Enum");
            }
        }

        private void GetFullAndPartialMatchCount(List<MatchedRecord> Records,
            SiteSearchStatus SearchStatus, int ComponentsInInvestigatorName)
        {
            SearchStatus.FullMatchCount = 0;
            SearchStatus.PartialMatchCount = 0;
            foreach (MatchedRecord record in Records)
            {
                if (record.MatchCount >= ComponentsInInvestigatorName)
                {
                    SearchStatus.FullMatchCount += 1;
                    record.IsFullMatch = true;
                }
                else
                {
                    SearchStatus.PartialMatchCount += 1;
                }
            }
        }

        #endregion

        #region ComplianceFormQueries

        public ComplianceForm GetComplianceForm(Guid ComplianceFormId)
        {
            var form = _UOW.ComplianceFormRepository.FindById(ComplianceFormId);
            return form;
        }


        public List<PrincipalInvestigator> getAllPrincipalInvestigators()
        {
            var retList = new List<PrincipalInvestigator>();

            //To avoid calling _UOW.ComplianceFormRepository.GetAll()
            //var compForms = _UOW.ComplianceFormRepository.GetAll().OrderByDescending(x => x.SearchStartedOn).ToList();
            //get comp forms of recent 3 months, To Date Add one day to ensure today's forms are included.
            var compFormFilter = new ComplianceFormFilter();
            compFormFilter.SearchedOnFrom = DateTime.Now.AddDays(-90);
            compFormFilter.SearchedOnTo = DateTime.Now.AddDays(1);
            var compForms = _UOW.ComplianceFormRepository.FindComplianceForms(compFormFilter).OrderByDescending(x => x.SearchStartedOn).ToList();



            return getPrincipalInvestigators(compForms);

            //foreach (ComplianceForm compForm in compForms)
            //{
            //    var item = getPrincipalInvestigators(compForm);
            //    retList.Add(item);
            //}
            //return retList;
        }

        public List<PrincipalInvestigator> getPrincipalInvestigators(string AssignedTo, bool Active)
        {
            var retList = new List<PrincipalInvestigator>();
            List<ComplianceForm> compForms;
            if (AssignedTo != null && AssignedTo.Length > 0)
            {
                //compForms = _UOW.ComplianceFormRepository.GetAll().Where(x => x.AssignedTo == AssignedTo).OrderByDescending(x => x.SearchStartedOn).ToList();
                compForms = _UOW.ComplianceFormRepository.FindComplianceForms(AssignedTo).OrderByDescending(x => x.SearchStartedOn).ToList();
            }
            else
            {
                //To avoid calling _UOW.ComplianceFormRepository.GetAll()
                //compForms = _UOW.ComplianceFormRepository.GetAll().OrderByDescending(x => x.SearchStartedOn).ToList();
                //get comp forms of recent 3 months, To Date Add one day to ensure today's forms are included.
                var compFormFilter = new ComplianceFormFilter();
                compFormFilter.SearchedOnFrom = DateTime.Now.AddDays(-90);
                compFormFilter.SearchedOnTo = DateTime.Now.AddDays(1);
                compForms = _UOW.ComplianceFormRepository.FindComplianceForms(compFormFilter).OrderByDescending(x => x.SearchStartedOn).ToList();


            }

            //foreach (ComplianceForm compForm in compForms.Where(x => x.Active == Active))
            //{
            //    var item = getPrincipalInvestigators(compForm);
            //    retList.Add(item);
            //}
            //return retList;

            return getPrincipalInvestigators(compForms);
        }

        public List<PrincipalInvestigator> getPrincipalInvestigators(string AssignedTo, bool Active = true, bool ReviewCompleted = false)
        {
            var retList = new List<PrincipalInvestigator>();

            retList = getPrincipalInvestigators(AssignedTo, Active);

            return retList.Where(x => x.ReviewCompleted == ReviewCompleted).ToList();

            
        }

        public List<PrincipalInvestigator> getPrincipalInvestigators(string AssignedTo, ReviewStatusEnum ReviewStatus)
        {
            var forms = _UOW.ComplianceFormRepository.FindComplianceForms(AssignedTo, ReviewStatus);
            return getPrincipalInvestigators(forms)
                .OrderByDescending(x => x.SearchStartedOn)
                .ToList();
        }

        //Patrick 11May2018 - Not used:
        //public List<PrincipalInvestigator> getPrincipalInvestigatorsByFilters(string AssignedTo, string PricipalInvestigatorName = "")
        //{
        //    var retList = new List<PrincipalInvestigator>();

        //    List<ComplianceForm> compForms;

        //    if (AssignedTo.Length > 0)
        //    {
        //        compForms = _UOW.ComplianceFormRepository.GetAll().Where(x => x.AssignedTo == AssignedTo).ToList();
        //    }
        //    else
        //    {
        //        compForms = _UOW.ComplianceFormRepository.GetAll();
        //    }

        //    //Principal Investigator
        //    List<ComplianceForm> compForms1;
        //    compForms1 = compForms.OrderByDescending(x => x.SearchStartedOn).ToList();
        //    if (PricipalInvestigatorName.Length > 0)
        //    {
        //        compForms1 = compForms.Where(x => x.InvestigatorDetails.Any(y => (y.Name.Contains(PricipalInvestigatorName) && y.Role == "PI"))).ToList();
        //    }
        //    else
        //    {
        //        compForms = compForms1;
        //    }


        //    foreach (ComplianceForm compForm in compForms1)
        //    {
        //        var item = getPrincipalInvestigators(compForm);
        //        retList.Add(item);
        //    }
        //    return retList;
        //}

        public List<PrincipalInvestigator> GetUnAssignedComplianceForms()
        {
            //var Forms = _UOW.ComplianceFormRepository.GetAll();

            //if (Forms.Count == 0)
            //    return null;

            //Forms = Forms.Where(x =>
            //    x.AssignedTo == null ||
            //    x.AssignedTo.Length == 0)
            //    .ToList();

            var Forms = _UOW.ComplianceFormRepository.FindComplianceForms("");


            var PIList = new List<PrincipalInvestigator>();

            foreach (ComplianceForm Form in Forms)
            {
                var PI = getPrincipalInvestigators(Form);
                PIList.Add(PI);
            }
            return PIList;
        }

        public List<PrincipalInvestigator> getPrincipalInvestigators(List<ComplianceForm> Forms)
        {
            var PIList = new List<PrincipalInvestigator>();

            foreach (ComplianceForm Form in Forms)
            {
                var PI = getPrincipalInvestigators(Form);
                PIList.Add(PI);
            }
            return PIList;
        }

        private PrincipalInvestigator getPrincipalInvestigators(ComplianceForm compForm)
        {
            var item = new PrincipalInvestigator();
            item.InputSource = compForm.InputSource;
            item.Address = compForm.Address;
            item.Country = compForm.Country;
            item.ProjectNumber = compForm.ProjectNumber;
            item.ProjectNumber2 = compForm.ProjectNumber2;
            item.Institute = compForm.Institute;
            item.SponsorProtocolNumber = compForm.SponsorProtocolNumber;
            item.SponsorProtocolNumber2 = compForm.SponsorProtocolNumber2;
            item.RecId = compForm.RecId;
            item.Active = compForm.Active;
            item.SearchStartedOn = compForm.SearchStartedOn;
            item.CurrentReviewStatus = compForm.CurrentReviewStatus;
            item.Reviewer = compForm.Reviewer;
            item.QCVerifier = compForm.QCVerifier;
            item.ExportedToiSprintOn = compForm.ExportedToiSprintOn;
            item.ReviewCompletedOn = compForm.ReviewCompletedOn;
            if (compForm.InvestigatorDetails.Count > 0)
            {
                item.Name = compForm.InvestigatorDetails.FirstOrDefault().Name;
            }
            item.AssignedTo = compForm.AssignedTo;
            item.AssignedToFullName = GetUserFullName(compForm.AssignedTo);
            item.Status = compForm.Status;
            item.StatusEnum = compForm.StatusEnum;
            item.ExtractionErrorInvestigatorCount = compForm.ExtractionErrorInvestigatorCount;
            item.ExtractionPendingInvestigatorCount = compForm.ExtractionPendingInvestigatorCount;
            item.EstimatedExtractionCompletionWithin = compForm.EstimatedExtractionCompletionWithin;

            foreach (InvestigatorSearched Investigator in compForm.InvestigatorDetails)
            {
                if (Investigator.Role.ToLower() == "sub i")
                {
                    var SubInv = new SubInvestigator();
                    SubInv.Name = Investigator.Name;
                    SubInv.Status = Investigator.Status;
                    SubInv.StatusEnum = Investigator.StatusEnum;
                    item.SubInvestigators.Add(SubInv);
                }
            }
            CanUndoQC(item, compForm);
            return item;
        }

        private void CanUndoQC(PrincipalInvestigator Investigator, ComplianceForm Form)
        {
            //required for Undo action in completed icsf page
            var ReviewCompleted = Form.Reviews.Find(x =>
                x.Status == ReviewStatusEnum.ReviewCompleted);

            var QCReview = Form.Reviews.Find(x =>
                x.Status == ReviewStatusEnum.QCCompleted);

            var Completed = Form.Reviews.Find(x =>
                x.Status == ReviewStatusEnum.Completed);

            if (QCReview == null &&
                Completed != null &&
                Form.QCVerifier != null) //For QCVerifier
                Investigator.UndoQCSubmit = true;
            else if (QCReview != null && Completed != null) //For Reviewer
                Investigator.UndoQCResponse = true;
            else if (ReviewCompleted == null && Completed != null)
                Investigator.UndoCompleted = true; //For reviewer, to undo completed to review completed
        }

        public List<PrincipalInvestigator> GetComplianceFormsFromFiltersWithReviewDates(
            ComplianceFormFilter CompFormFilter)
        {

            var compForms = GetComplianceFormsFromFilters(CompFormFilter);
            //ReviewCompletedOnFrom is a computed value
            var compForms1 = compForms;
            if (CompFormFilter.ReviewCompletedOnFrom != null)
            {
                DateTime startDate;
                startDate = CompFormFilter.ReviewCompletedOnFrom.Value.Date;
                compForms1 = compForms.Where(x => x.ReviewCompletedOn >= startDate).ToList();
            }
            var compForms2 = compForms1;
            if (CompFormFilter.ReviewCompletedOnTo != null)
            {
                DateTime endDate;
                endDate = CompFormFilter.ReviewCompletedOnTo.Value.Date.AddDays(1);
                compForms2 = compForms1.Where(x => x.ReviewCompletedOn < endDate).ToList();
            }
            return compForms2;

            
        }
        public List<PrincipalInvestigator> GetComplianceFormsFromFilters(
            ComplianceFormFilter CompFormFilter)
        {
            if (CompFormFilter == null)
            {
                throw new Exception("Invalid CompFormFilter");
            }
           
            var compForms = _UOW.ComplianceFormRepository.FindComplianceForms(CompFormFilter);

            if ((int)CompFormFilter.Status == -1)
            {
                //Commented on 16May2018: Pradeep
                //compForms = compForms.FindAll(x => x.StatusEnum ==
                //ComplianceFormStatusEnum.ReviewCompletedIssuesIdentified ||
                //x.StatusEnum == ComplianceFormStatusEnum.ReviewCompletedIssuesNotIdentified)
                //.ToList();
            }
            else if (CompFormFilter.Status == ComplianceFormStatusEnum.ReviewCompletedIssuesIdentified)
            {
                compForms = compForms.Where(x =>
                x.StatusEnum == ComplianceFormStatusEnum.ReviewCompletedIssuesIdentified ||
                x.StatusEnum == ComplianceFormStatusEnum.ReviewCompletedIssuesNotIdentified)
                .ToList();
            }
            else if (CompFormFilter.Status == ComplianceFormStatusEnum.FullMatchFoundReviewPending)
            {
                compForms = compForms.Where(x =>
                x.StatusEnum == ComplianceFormStatusEnum.FullMatchFoundReviewPending ||
                x.StatusEnum == ComplianceFormStatusEnum.PartialMatchFoundReviewPending ||
                x.StatusEnum == ComplianceFormStatusEnum.NoMatchFoundReviewPending ||
                x.StatusEnum == ComplianceFormStatusEnum.IssuesIdentifiedReviewPending ||
                x.StatusEnum == ComplianceFormStatusEnum.SingleMatchFoundReviewPending ||
                x.StatusEnum == ComplianceFormStatusEnum.ManualSearchSiteReviewPending ||
                x.StatusEnum == ComplianceFormStatusEnum.HasExtractionErrors ||
                x.StatusEnum == ComplianceFormStatusEnum.NotScanned)
                .ToList();
            }

            return getPrincipalInvestigators(compForms).OrderByDescending(x => x.SearchStartedOn).ToList();
        }

        public List<PrincipalInvestigator> GetClosedComplianceFormsFromFilters(
            ComplianceFormFilter CompFormFilter, string AssignedTo)
        {
            if (CompFormFilter == null)
            {
                throw new Exception("Invalid CompFormFilter");
            }

            CompFormFilter.AssignedTo = AssignedTo;

            var compForms = _UOW.ComplianceFormRepository.FindComplianceForms(CompFormFilter);

            if (CompFormFilter.Status == ComplianceFormStatusEnum.ReviewCompletedIssuesIdentified)
            {
                compForms = compForms.Where(x =>
                x.StatusEnum == ComplianceFormStatusEnum.ReviewCompletedIssuesIdentified ||
                x.StatusEnum == ComplianceFormStatusEnum.ReviewCompletedIssuesNotIdentified)
                .ToList();
            }

            return getPrincipalInvestigators(compForms).OrderByDescending(x => x.SearchStartedOn).ToList();
        }

        public List<InstituteFindingsSummaryViewModel> getInstituteFindingsSummary(Guid CompFormId)
        {
            var retList = new List<InstituteFindingsSummaryViewModel>();
            var compForm = _UOW.ComplianceFormRepository.FindById(CompFormId);
            if (compForm != null)
            {
                var InstSiteSources = compForm.SiteSources.Where(x => x.SearchAppliesTo == SearchAppliesToEnum.Institute);
                foreach (var instSite in InstSiteSources)
                {
                    var item = new InstituteFindingsSummaryViewModel();
                    item.SiteSourceId = instSite.Id;
                    item.DisplayPosition = instSite.DisplayPosition;
                    item.IsMandatory = instSite.IsMandatory;
                    item.SiteId = instSite.SiteId;
                    item.SiteName = instSite.SiteName;
                    item.SiteShortName = instSite.SiteShortName;
                    item.SiteUrl = instSite.SiteUrl;
                    item.IssuesFound = compForm.Findings.Where(x => x.SiteSourceId == instSite.Id).Count();

                    retList.Add(item);
                }
                return retList;
            }
            else
            {
                return null;
            }

        } 

        #endregion

        #region ComplianceFormGeneration - both PDF and Word
        public MemoryStream GenerateComplianceForm(
            Guid? ComplianceFormId,
            IWriter writer,
            string FileExtension,
            out string FileName)
        {
            //writer.AttachFile("", "");

            var form = _UOW.ComplianceFormRepository.FindById(ComplianceFormId);

            var PI = RemoveSpecialCharacters(form.InvestigatorDetails.FirstOrDefault().Name);

            //var ProjectNumber = RemoveSpecialCharacters(form.ProjectNumber);

            var ProjectNumber = form.ProjectNumber.Replace('/', '-');

            if (form.ProjectNumber2 != null && form.ProjectNumber2.Trim() != "")
                ProjectNumber += "-" + form.ProjectNumber2.Replace('/', '-');

            var PISearchName = form.InvestigatorDetails.FirstOrDefault().SearchName;

            var GeneratedFileName =
                ProjectNumber + "_" +
                form.Country + "_" +
                PISearchName + "_" +
                DateTime.Now.ToString("ddMMMyyyy") +
                FileExtension;

            FileName = GeneratedFileName.Replace(' ', '_');

            var GeneratedFileNameNPath =
                _config.ComplianceFormFolder + GeneratedFileName;

            //if (File.Exists(GeneratedFileNameNPath))
            //{
            //    return GeneratedFileNameNPath;
            //}

            //Below condition is required when file is created on the server and
            //path is returned to the client
            //if (File.Exists(GeneratedFileNameNPath))
            //    return GeneratedFileNameNPath;

            writer.Initialize(_config.WordTemplateFolder, GeneratedFileNameNPath);

            writer.WriteParagraph("INVESTIGATOR COMPLIANCE SEARCH FORM");

            writer.AddFormHeaders(
                form.ProjectNumber, form.ProjectNumber2,
                form.SponsorProtocolNumber, form.SponsorProtocolNumber2,
                form.Institute,
                (form.Address + " " + form.Country));

            //InvestigatorDetailsTable
            writer.WriteParagraph("Investigators:");

            string[] TableHeaders = InvestigatorTableHeaders();

            writer.AddTableHeaders(TableHeaders, 4, 1);

            foreach (InvestigatorSearched Investigator in form.InvestigatorDetails)
            {
                string MedicalLicenseNumber = null;
                if (Investigator.MedicalLiceseNumber == null || Investigator.MedicalLiceseNumber.Trim() == "")
                    MedicalLicenseNumber = "NA";
                else
                    MedicalLicenseNumber = Investigator.MedicalLiceseNumber;

                string[] CellValues = new string[]
                {
                    Investigator.Role,
                    Investigator.Name,
                    MedicalLicenseNumber,
                    Investigator.SearchName
                };
                writer.FillUpTable(CellValues, "center");
            }
            //SaveChanges is required for PDF generation
            writer.SaveChanges();

            //SitesTable
            writer.WriteParagraph(
                "Relevant sources of Investigator information, " +
                "against which this Investigator has been checked.");

            TableHeaders = SitesTableHeaders();
            writer.AddTableHeaders(TableHeaders, 5, 2);

            int ColumnIndex = 1;
            int RowIndex = 1;
            foreach (SiteSource Site in form.SiteSources)
            {
                if (!Site.IsMandatory)
                    continue;

                string SiteSourceUpdatedOn = "";

                if (Site.SiteSourceUpdatedOn != null)
                    SiteSourceUpdatedOn =
                        Site.SiteSourceUpdatedOn.Value.ToString("dd MMM yyyy");

                string[] CellValues = new string[]
                {
                    RowIndex.ToString(),
                    Site.SiteName,
                    SiteSourceUpdatedOn,
                    Site.SiteUrl,
                    Site.IssuesIdentified ? "Yes" : "No"
                };

                writer.FillUpTable(CellValues, "left");

                RowIndex += 1;
                ColumnIndex += 1;
            }
            writer.SaveChanges();

            //AdditionalSitesTable
            writer.WriteParagraph("Addtional sources:");
            TableHeaders = SitesTableHeaders();
            writer.AddTableHeaders(TableHeaders, 5, 3);

            foreach (SiteSource Site in form.SiteSources)
            {
                if (!Site.IsMandatory)
                {
                    string SiteSourceUpdatedOn = "";

                    if (Site.SiteSourceUpdatedOn != null)
                        SiteSourceUpdatedOn =
                            Site.SiteSourceUpdatedOn.Value.ToString("dd MMM yyyy");

                    string[] CellValues = new string[]
                    {
                        RowIndex.ToString(),
                        Site.SiteName,
                        SiteSourceUpdatedOn,
                        Site.SiteUrl,
                        Site.IssuesIdentified ? "Yes" : "No"
                    };
                    writer.FillUpTable(CellValues, "left");
                    RowIndex += 1;
                }
            }
            writer.SaveChanges();

            //FindingsTable
            writer.WriteParagraph(
                "Additional details for issues (Yes) identified above:");

            TableHeaders = FindingsTableHeaders();
            writer.AddTableHeaders(TableHeaders, 4, 4);

            if (form.IssuesFoundInvestigatorCount == 0)
            {
                string[] CellValues = new string[]
                {
                    "", "", "", "No Findings"
                };
                writer.FillUpTable(CellValues, "center");
            }
            else
            {
                foreach (Finding finding in form.Findings.OrderBy(x => x.DisplayPosition))
                {
                    string DateOfInspection = "";
                    if (finding.DateOfInspection != null)
                        DateOfInspection =
                            finding.DateOfInspection.Value.ToString("dd MMM yyyy");

                    if (finding.Selected && finding.IsAnIssue)
                    {
                        string[] CellValues = new string[]
                        {
                        finding.SiteSourceId.ToString(),
                        finding.InvestigatorName == null ? form.Institute : finding.InvestigatorName,
                        DateOfInspection,
                        finding.Observation != null ? finding.Observation.Trim() : ""
                        };
                        writer.FillUpTable(CellValues, "left");
                    }
                }
            }
            writer.SaveChanges();

            //SearchedByTable
            writer.WriteParagraph("Search Performed By:");

            var UserFullName = GetUserFullName(form.AssignedTo);
            writer.AddSearchedBy(UserFullName, DateTime.Now.ToString("dd MMM yyyy"));

            writer.SaveChanges();

            //writer.AddFooterPart("Updated On: " + form.UpdatedOn.ToString("dd MMM yyyy"));
            //writer.AddFooterPart(form.ProjectNumber + "_" + form.InvestigatorDetails.FirstOrDefault().Name + "_" + DateTime.Now.ToString("ddMMMyy"));

            writer.CloseDocument();

            //writer.AttachFile(@"C:\Development\test.pdf", GeneratedFileNameNPath);

            //return GeneratedFileNameNPath;
            return writer.ReturnStream();
        }

        private string[] InvestigatorTableHeaders()
        {
            return new string[]
            {
                "ROLE",
                "INVESTIGATOR NAME (As per 1572/Info received)",
                //"Qualification",
                "MEDICAL LICENSE NUMBER",
                "INVESTIGATOR NAME (All Combination Searched)"
            };
        }

        private string[] SitesTableHeaders()
        {
            return new string[]
            {
                "SOURCE #",
                "SOURCE NAME",
                "SOURCE DATE",
                "WEBLINK",
                "ISSUES IDENTIFIED"
            };
        }

        private string[] FindingsTableHeaders()
        {
            return new string[]
            {
                "SOURCE #",
                "INVESTIGATOR NAME",
                "DATE OF INSPECTION/ACTION",
                "DESCRIPTION OF FINDINGS"
            };
        }

        #endregion

        #region ByPatrick

        //3Dec2016
        public MemoryStream GenerateComplianceForm(Guid? ComplianceFormId)
        {
            var form = _UOW.ComplianceFormRepository.FindById(ComplianceFormId);

            var UtilitiesObject = new CreateComplianceFormWord();

            var FileName = form.InvestigatorDetails.FirstOrDefault().Name + ".docx";

            return UtilitiesObject.CreateComplianceForm(form, FileName);
        }

        //Not required?
        public InvestigatorSearched getInvestigatorSiteSummary(string compFormId, int InvestigatorId)
        {
            Guid gCompFormId = Guid.Parse(compFormId);
            var compForm = _UOW.ComplianceFormRepository.FindById(gCompFormId);
            if (compForm == null)
            {
                return null;
            }
            else
            {
                return getInvestigatorSiteSummary(compForm, InvestigatorId);
            }
        }

        public InvestigatorSearched getInvestigatorSiteSummary(ComplianceForm compForm, int InvestigatorId)
        {
            InvestigatorSearched retInv = new InvestigatorSearched();
            // inv.SitesSearched.Find(x => x.siteEnum == site.SiteEnum);
            InvestigatorSearched invInCompForm =
                compForm.InvestigatorDetails.Find(
                x => x.Id == InvestigatorId);

            if (invInCompForm == null)
            {
                return null;
            }
            retInv.Name = invInCompForm.Name;
            retInv.Role = invInCompForm.Role;
            retInv.Deleted = invInCompForm.Deleted;
            retInv.DisplayPosition = invInCompForm.DisplayPosition;
            retInv.Id = invInCompForm.Id;
            retInv.Sites_FullMatchCount = invInCompForm.Sites_FullMatchCount;
            retInv.Sites_PartialMatchCount = invInCompForm.Sites_PartialMatchCount;
            retInv.TotalIssuesFound = invInCompForm.TotalIssuesFound;

            //?? for manual sites ???
            foreach (SiteSource site in compForm.SiteSources)
            {
                var searchStatus = new SiteSearchStatus();
                searchStatus.SiteSourceId = site.Id;
                searchStatus.DisplayPosition = site.DisplayPosition;
                searchStatus.siteEnum = site.SiteEnum;
                searchStatus.SiteUrl = site.SiteUrl;
                searchStatus.SiteName = site.SiteName;

                var searchStatusInCompForm =
                    invInCompForm.SitesSearched.Find(
                    x => x.siteEnum == site.SiteEnum);

                if (searchStatusInCompForm == null)
                {
                    searchStatus.ExtractionErrorMessage = "Site not searched";
                }
                else
                {
                    searchStatus.FullMatchCount = searchStatusInCompForm.FullMatchCount;
                    searchStatus.IssuesFound = searchStatusInCompForm.IssuesFound;
                    searchStatus.PartialMatchCount = searchStatusInCompForm.PartialMatchCount;
                    searchStatus.ReviewCompleted = searchStatusInCompForm.ReviewCompleted;

                }

                retInv.SitesSearched.Add(searchStatus);
            }

            return retInv;
        }

        //public ComplianceForm UpdateFindings(ComplianceForm form)
        //{
        //    foreach (InvestigatorSearched Investigator in form.InvestigatorDetails)
        //    {
        //        Investigator.TotalIssuesFound = 0;

        //        foreach (SiteSearchStatus searchStatus in Investigator.SitesSearched)
        //        {
        //            var ListOfFindings = form.Findings;

        //            var Findings = ListOfFindings.Where(
        //                x => x.SiteEnum == searchStatus.siteEnum).ToList();

        //            int IssuesFound = 0;
        //            int InvId = 0;
        //            foreach (Finding Finding in Findings)
        //            {
        //                if (Finding != null && Finding.IsAnIssue &&
        //                    Finding.InvestigatorSearchedId == Investigator.Id)
        //                {
        //                    InvId = Finding.InvestigatorSearchedId;
        //                    IssuesFound += 1;
        //                    searchStatus.IssuesFound = IssuesFound;
        //                }
        //            }
        //            Investigator.TotalIssuesFound += IssuesFound;

        //            var Site = form.SiteSources.Find
        //                (x => x.SiteEnum == searchStatus.siteEnum);

        //            if (IssuesFound > 0 && Investigator.Id == InvId)
        //                Site.IssuesIdentified = true;
        //        }
        //    }
        //    return form;
        //}

        private List<MatchedRecord> GetFDADebarPageMatchedRecords(Guid? SiteDataId,
            string InvestigatorName,
            int ComponentsInInvestigatorName,
            out DateTime? SiteLastUpdatedOn)
        {
            var FDASearchResult =
                _UOW.FDADebarPageRepository.FindById(SiteDataId);

            //var FDASearchResult = _cachedData.GetFDADebarPageLatestCache();

            if (FDASearchResult == null)
                throw new Exception(
                    "Document with RecId: "
                    + SiteDataId +
                    " does not contain any records");

            UpdateMatchStatus(
                FDASearchResult.DebarredPersons,
                InvestigatorName);  //updates list with match count

            var DebarList = FDASearchResult.DebarredPersons.Where(
               debarredList => debarredList.MatchCount > 0).ToList();

            SiteLastUpdatedOn = FDASearchResult.SiteLastUpdatedOn;

            if (DebarList == null)
                return null;

            //Patrick: Further refactoring possible, ConvertToMatchedRecords may not be required:
            return ConvertToMatchedRecords(DebarList);
        }

        private List<MatchedRecord> GetClinicalInvestigatorPageMatchedRecords(Guid? SiteDataId,
            string InvestigatorName,
            int ComponentsInInvestigatorName,
            out DateTime? SiteLastUpdatedOn)
        {
            var CIILSearchResult =
                _UOW.ClinicalInvestigatorInspectionListRepository
                .FindById(SiteDataId);

            //var CIILSearchResult = _cachedData.GetClinicalInvestigatorInspectionListLatestCache();

            if (CIILSearchResult == null)
                throw new Exception(
                    "Document with RecId: "
                    + SiteDataId +
                    " does not contain any records");

            var CIILRecords = _UOW.ClinicalInvestigatorInspectionRepository.GetAll();
            //var CIILRecords = _cachedData.GetClinicalInvestigatorRecordsCache();

            AddRecordsToCIILSiteData(CIILSearchResult, CIILRecords);

            UpdateMatchStatus(CIILSearchResult.ClinicalInvestigatorInspectionList,
                InvestigatorName);  //updates list with match count

            var ClinicalInvestigatorList = CIILSearchResult.ClinicalInvestigatorInspectionList
                .Where(
               ClinicalList => ClinicalList.MatchCount > 0).ToList();

            SiteLastUpdatedOn = CIILSearchResult.SiteLastUpdatedOn;

            if (ClinicalInvestigatorList == null)
                return null;

            return ConvertToMatchedRecords(ClinicalInvestigatorList);
        }

        private void AddRecordsToCIILSiteData(
            ClinicalInvestigatorInspectionSiteData SiteData,
            List<ClinicalInvestigator> Records)
        {
            foreach (ClinicalInvestigator Record in Records)
            {
                var RecordToAdd = new ClinicalInvestigator();
                RecordToAdd.Name = Record.Name;
                RecordToAdd.Address = Record.Address;
                RecordToAdd.City = Record.City;
                RecordToAdd.ClassificationCode = Record.ClassificationCode;
                RecordToAdd.Country = Record.Country;
                RecordToAdd.DeficiencyCode = Record.DeficiencyCode;
                RecordToAdd.IdNumber = Record.IdNumber;
                RecordToAdd.InspectionDate = Record.InspectionDate;
                RecordToAdd.InspectionType = Record.InspectionType;
                RecordToAdd.State = Record.State;
                RecordToAdd.Status = Record.Status;
                RecordToAdd.Zip = Record.Zip;
                RecordToAdd.Location = Record.Location;
                RecordToAdd.Links = Record.Links;
                RecordToAdd.MatchCount = Record.MatchCount;
                RecordToAdd.RowNumber = Record.RowNumber;
                RecordToAdd.RecordNumber = Record.RecordNumber;

                SiteData.ClinicalInvestigatorInspectionList.Add(RecordToAdd);
            }
        }

        private List<MatchedRecord> GetFDAWarningLettersPageMatchedRecords(Guid? SiteDataId,
            string NameToSearch,
            int ComponentsInInvestigatorName, out DateTime? SiteLastUpdatedOn)
        {
            var FDAWarningSiteData = _UOW.FDAWarningLettersRepository.FindById(SiteDataId);
            //var FDAWarningSiteData = _cachedData.GetFDAWarningLettersLatestCache();

            if (FDAWarningSiteData == null)
                throw new Exception(
                    "Document with RecId: "
                    + SiteDataId +
                    " does not contain any records");

            var FDAWarningRecords = _UOW.FDAWarningRepository.GetAll();
            //var FDAWarningRecords = _cachedData.GetFDAWarningRecordsCache();

            AddRecordsToWarningLettersSiteData(FDAWarningSiteData, FDAWarningRecords);
            
            UpdateMatchStatus(FDAWarningSiteData.FDAWarningLetterList, NameToSearch);

            var FDAWarningLetterList =
                FDAWarningSiteData.FDAWarningLetterList
                .Where(FDAList => FDAList.MatchCount > 0)
                .ToList();

            SiteLastUpdatedOn = FDAWarningSiteData.SiteLastUpdatedOn;

            if (FDAWarningLetterList == null)
                return null;

            //only for FDAWarningLetters
            ConvertFDAWarningLinks(FDAWarningSiteData.FDAWarningLetterList);

            return ConvertToMatchedRecords(FDAWarningLetterList);
        }

        //Only for FDAWarningLettters site, 
        //link from the downloaded/extracted file is not working
        //hence converting it to active link
        private void ConvertFDAWarningLinks(IEnumerable<SiteDataItemBase> Records)
        {
            foreach (SiteDataItemBase Record in Records)
            {
                foreach (Link link in Record.Links)
                {
                    var NewLink =
                        "https://www.fda.gov/iceci/enforcementactions/warningletters/";

                    if (!Record.DateOfInspection.HasValue)
                        continue;
                    NewLink += Record.DateOfInspection.Value.ToString("yyyy");
                    NewLink += "/";
                    NewLink += link.url.Substring(link.url.Length - 9).ToLower();
                    NewLink += ".htm";

                    link.url = NewLink;
                }
            }
        }

        private void AddRecordsToWarningLettersSiteData(
            FDAWarningLettersSiteData SiteData, List<FDAWarningLetter> Records)
        {
            foreach (FDAWarningLetter Record in Records)
            {
                var RecordToAdd = new FDAWarningLetter();
                RecordToAdd.Company = Record.Company;
                RecordToAdd.PostedDate = Record.PostedDate;
                RecordToAdd.IssuingOffice = Record.IssuingOffice;
                RecordToAdd.LetterIssued = Record.LetterIssued;
                RecordToAdd.Links = Record.Links;
                RecordToAdd.MatchCount = Record.MatchCount;
                RecordToAdd.CloseoutDate = Record.CloseoutDate;
                RecordToAdd.RecordNumber = Record.RecordNumber;
                RecordToAdd.ResponseLetterPosted = Record.ResponseLetterPosted;
                RecordToAdd.RowNumber = Record.RowNumber;
                RecordToAdd.Status = Record.Status;
                RecordToAdd.Subject = Record.Subject;

                //return
                //    "Posted: " + PostedDate + "~" +
                //    "Letter Issued: " + LetterIssued + "~" +
                //    "Company: " + CompanyText + "~" +
                //    "Issuing Office: " + IssuingOffice + "~" +
                //    "Subject: " + Subject + "~" +
                //    "Response Letter Posted: " + ResponseLetterPostedText + "~" +
                //    "Closeout Date: " + CloseoutDateText;

                SiteData.FDAWarningLetterList.Add(RecordToAdd);
            }
        }

        private List<MatchedRecord> GetERRProposalToDebarPageMatchedRecords(Guid? SiteDataId,
            string InvestigatorName,
            int ComponentsInInvestigatorName,
            out DateTime? SiteLastUpdatedOn)
        {
            var ERRSearchResult =
                _UOW.ERRProposalToDebarRepository.FindById(SiteDataId);
            //var ERRSearchResult = _cachedData.GetProposalToDebarSiteScanDetailsLatestCache();

            if (ERRSearchResult == null)
                throw new Exception(
                    "Document with RecId: "
                    + SiteDataId +
                    " does not contain any records");

            UpdateMatchStatus(ERRSearchResult.ProposalToDebar,
                InvestigatorName);  //updates list with match count

            var ERRList = ERRSearchResult.ProposalToDebar.Where(
               ErrList => ErrList.MatchCount > 0).ToList();

            SiteLastUpdatedOn = ERRSearchResult.SiteLastUpdatedOn;

            if (ERRList == null)
                return null;

            return ConvertToMatchedRecords(ERRList);
        }

        private List<MatchedRecord> GetAdequateAssurancePageMatchedRecords(Guid? SiteDataId,
            string InvestigatorName,
            int ComponentsInInvestigatorName,
            out DateTime? SiteLastUpdatedOn)
        {
            var AdequateAssuranceSearchResult =
                _UOW.AdequateAssuranceListRepository.FindById(SiteDataId);

            //var AdequateAssuranceSearchResult = _cachedData.GetAdequateAssuranceListLatestCache();

            if (AdequateAssuranceSearchResult == null)
                throw new Exception(
                    "Document with RecId: "
                    + SiteDataId +
                    " does not contain any records");

            UpdateMatchStatus(
                AdequateAssuranceSearchResult.AdequateAssurances,
                InvestigatorName);  //updates list with match count

            var AdequateAssuranceList =
                AdequateAssuranceSearchResult.AdequateAssurances.Where(
                AssuranceList => AssuranceList.MatchCount > 0).ToList();

            SiteLastUpdatedOn = AdequateAssuranceSearchResult.SiteLastUpdatedOn;

            if (AdequateAssuranceList == null)
                return null;

            return ConvertToMatchedRecords(AdequateAssuranceList);
        }

        private List<MatchedRecord> GetClinicalInvestigatorDisqualificationPageMatchedRecords(
            Guid? SiteDataId, string NameToSearch,
            int ComponentsInInvestigatorName, out DateTime? SiteLastUpdatedOn)
        {
            var DisqualificationSiteData =
                _UOW.ClinicalInvestigatorDisqualificationRepository.FindById(SiteDataId);
            //var DisqualificationSiteData = _cachedData.GetClinicalInvestigatorDisqualificationLatestCache();

            if (DisqualificationSiteData == null)
                throw new Exception(
                    "Document with RecId: "
                    + SiteDataId +
                    " does not contain any records");

            UpdateMatchStatus(DisqualificationSiteData.DisqualifiedInvestigatorList,
                NameToSearch);

            //DateTime? temp = null;
            //_SearchEngine.ExtractData(
            //    SiteEnum.ClinicalInvestigatorDisqualificationPage, NameToSearch,
            //    MatchCountLowerLimit, out temp);

            //var siteData = _SearchEngine.SiteData;
            //SiteLastUpdatedOn = temp;

            //var baseSiteData = _SearchEngine.baseSiteData;

            //UpdateMatchStatus(siteData, NameToSearch);

            var SiteData =
                DisqualificationSiteData.DisqualifiedInvestigatorList
                .Where(site => site.MatchCount > 0)
                .ToList();

            SiteLastUpdatedOn = DisqualificationSiteData.SiteLastUpdatedOn;

            if (SiteData == null)
                return null;

            return ConvertToMatchedRecords(SiteData);
        }

        private List<MatchedRecord> GetCBERClinicalInvestigatorPageMatchedRecords(Guid? SiteDataId,
            string InvestigatorName,
            int ComponentsInInvestigatorName,
            out DateTime? SiteLastUpdatedOn)
        {
            var CBERSearchResult =
                _UOW.CBERClinicalInvestigatorRepository.FindById(SiteDataId);
            //var CBERSearchResult = _cachedData.GetCBERClinicalInvestigatorLatestCache();

            if (CBERSearchResult == null)
                throw new Exception(
                    "Document with RecId: "
                    + SiteDataId +
                    " does not contain any records");

            UpdateMatchStatus(
                CBERSearchResult.ClinicalInvestigator,
                InvestigatorName);  //updates list with match count

            var ClinicalInvestigatorList = CBERSearchResult.ClinicalInvestigator.Where(
               CBERList => CBERList.MatchCount > 0).ToList();

            SiteLastUpdatedOn = CBERSearchResult.SiteLastUpdatedOn;

            if (ClinicalInvestigatorList == null)
                return null;

            return ConvertToMatchedRecords(ClinicalInvestigatorList);
        }

        private List<MatchedRecord> GetExclusionDatabasePageMatchedRecords(Guid? SiteDataId,
            string InvestigatorName, int ComponentsInInvestigatorName,
            out DateTime? SiteLastUpdatedOn)
        {
            var ExclusionSearchResult =
                _UOW.ExclusionDatabaseSearchRepository.FindById(SiteDataId);
            //var ExclusionSearchResult = _cachedData.GetExclusionDatabaseSearchLatestCache();

            if (ExclusionSearchResult == null)
                throw new Exception(
                    "Document with RecId: "
                    + SiteDataId +
                    " does not contain any records");

            var ExclusionRecords = _UOW.ExclusionDatabaseRepository.GetAll();
            //var ExclusionRecords = _cachedData.GetExclusionDatabaseRecordsCache();

            AddRecordsToExclusionSiteData(ExclusionSearchResult, ExclusionRecords);

            UpdateMatchStatus(
                ExclusionSearchResult.ExclusionSearchList,
                InvestigatorName);  //updates list with match count

            var ExclusionDBList = ExclusionSearchResult.ExclusionSearchList.Where(
               ExclusionList => ExclusionList.MatchCount > 0).ToList();

            SiteLastUpdatedOn = ExclusionSearchResult.SiteLastUpdatedOn;

            if (ExclusionDBList == null)
                return null;

            return ConvertToMatchedRecords(ExclusionDBList);
        }

        private void AddRecordsToExclusionSiteData(
            ExclusionDatabaseSearchPageSiteData siteData,
            List<ExclusionDatabaseSearchList> Records)
        {
            foreach (ExclusionDatabaseSearchList Record in Records)
            {
                var RecordToAdd = new ExclusionDatabaseSearchList();
                RecordToAdd.FirstName = Record.FirstName;
                RecordToAdd.LastName = Record.LastName;
                RecordToAdd.MiddleName = Record.MiddleName;
                RecordToAdd.ExclusionDate = Record.ExclusionDate;
                RecordToAdd.ExclusionType = Record.ExclusionType;
                RecordToAdd.RecordNumber = Record.RecordNumber;
                RecordToAdd.Status = Record.Status;
                RecordToAdd.General = Record.General;
                RecordToAdd.BusinessName = Record.BusinessName;
                RecordToAdd.Address = Record.Address;
                RecordToAdd.City = Record.City;
                RecordToAdd.State = Record.State;
                RecordToAdd.Zip = Record.Zip;
                RecordToAdd.Specialty = Record.Specialty;
                RecordToAdd.Links = Record.Links;
                RecordToAdd.RowNumber = Record.RowNumber;
                RecordToAdd.MatchCount = Record.MatchCount;

                siteData.ExclusionSearchList.Add(RecordToAdd);
            }
        }

        private List<MatchedRecord> GetPHSAdministrativeActionPageMatchedRecords(Guid? SiteDataId,
            string InvestigatorName, int ComponentsInInvestigatorName,
            out DateTime? SiteLastUpdatedOn)
        {
            var PHSSearchResult =
                _UOW.PHSAdministrativeActionListingRepository.FindById(SiteDataId);
            //var PHSSearchResult = _cachedData.GetPHSAdministrativeActionListingLatestCache();

            if (PHSSearchResult == null)
                throw new Exception(
                    "Document with RecId: "
                    + SiteDataId +
                    " does not contain any records");

            UpdateMatchStatus(
                PHSSearchResult.PHSAdministrativeSiteData,
                InvestigatorName);  //updates list with match count

            var PHSList = PHSSearchResult.PHSAdministrativeSiteData.Where(
               PHSData => PHSData.MatchCount > 0).ToList();

            SiteLastUpdatedOn = PHSSearchResult.SiteLastUpdatedOn;

            if (PHSList == null)
                return null;

            return ConvertToMatchedRecords(PHSList);
        }

        private List<MatchedRecord> GetCIAPageMatchedRecords(Guid? SiteDataId,
            string InvestigatorName,
            int ComponentsInInvestigatorName,
            out DateTime? SiteLastUpdatedOn)
        {
            var CIASearchResult =
                _UOW.CorporateIntegrityAgreementRepository.FindById(SiteDataId);
            //var CIASearchResult = _cachedData.GetCorporateIntegrityAgreementLatestCache();

            if (CIASearchResult == null)
                throw new Exception(
                    "Document with RecId: "
                    + SiteDataId +
                    " does not contain any records");

            UpdateMatchStatus(
                CIASearchResult.CIAListSiteData,
                InvestigatorName);  //updates list with match count

            var CIAList = CIASearchResult.CIAListSiteData.Where(
               CIAData => CIAData.MatchCount > 0).ToList();

            SiteLastUpdatedOn = CIASearchResult.SiteLastUpdatedOn;

            if (CIAList == null)
                return null;

            return ConvertToMatchedRecords(CIAList);
        }

        private List<MatchedRecord> GetSAMPageMatchedRecords(Guid? SiteDataId,
            string NameToSearch,
            int ComponentsInIvestigatorName, out DateTime? SiteLastUpdatedOn)
        {
            var siteData =
                _UOW.SystemForAwardManagementRepository.FindById(SiteDataId);

            //var siteData = _cachedData.GetSystemForAwardManagementLatestCache();

            if (siteData == null)
                throw new Exception(
                    "Document with RecId: "
                    + SiteDataId +
                    " does not contain any records");

            var tempData = _UOW.SAMSiteDataRepository.GetAll();
            //var tempData = _cachedData.GetSystemForAwardManagementRecordsCache();

            //siteData.SAMSiteData.Concat(tempData);
            AddRecordsToSAMSiteData(siteData, tempData);

            UpdateMatchStatus(siteData.SAMSiteData, NameToSearch);

            SiteLastUpdatedOn = siteData.SiteLastUpdatedOn;

            var DisqualificationSiteData =
                siteData.SAMSiteData.Where(site => site.MatchCount > 0);

            if (DisqualificationSiteData == null)
                return null;

            return ConvertToMatchedRecords(DisqualificationSiteData);
        }

        private void AddRecordsToSAMSiteData(
            SystemForAwardManagementPageSiteData siteData,
            List<SystemForAwardManagement> records)
        {
            foreach (SystemForAwardManagement rec in records)
            {
                var recToAdd = new SystemForAwardManagement();
                recToAdd.ActiveDate = rec.ActiveDate;
                recToAdd.AdditionalComments = rec.AdditionalComments;

                recToAdd.ExcludingAgency = rec.ExcludingAgency;
                recToAdd.ExclusionType = rec.ExclusionType;
                recToAdd.First = rec.First;
                recToAdd.Last = rec.Last;
                recToAdd.Links = rec.Links;
                recToAdd.MatchCount = rec.MatchCount;
                recToAdd.Middle = rec.Middle;
                recToAdd.ParentId = rec.ParentId;
                recToAdd.RecId = rec.RecId;
                recToAdd.City = rec.City;
                recToAdd.State = rec.State;
                recToAdd.Country = rec.Country;
                recToAdd.RecordNumber = rec.RecordNumber;
                recToAdd.RecordStatus = rec.RecordStatus;
                recToAdd.RowNumber = rec.RowNumber;

                siteData.SAMSiteData.Add(recToAdd);
            }
        }

        private List<MatchedRecord> GetSDNPageMatchedRecords(Guid? SiteDataId,
            string InvestigatorName, int ComponentsInInvestigatorName,
            out DateTime? SiteLastUpdatedOn)
        {
            var SDNSearchResult =
                _UOW.SpeciallyDesignatedNationalsRepository.FindById(SiteDataId);
            //var SDNSearchResult = _cachedData.GetSpeciallyDesignatedNationalsLatestCache();        

            if (SDNSearchResult == null)
                throw new Exception(
                    "Document with RecId: "
                    + SiteDataId +
                    " does not contain any records");

            var SDNRecords = _UOW.SDNSiteDataRepository.GetAll();
            //var SDNRecords = _cachedData.GetSpeciallyDesignatedNationalsRecordsCache();

            AddRecordsToSDNSiteData(SDNSearchResult, SDNRecords);

            UpdateMatchStatus(
                SDNSearchResult.SDNListSiteData,
                InvestigatorName);

            var SDNList = SDNSearchResult.SDNListSiteData.Where(
               SDNData => SDNData.MatchCount > 0).ToList();

            SiteLastUpdatedOn = SDNSearchResult.SiteLastUpdatedOn;

            if (SDNList == null)
                return null;

            return ConvertToMatchedRecords(SDNList);
        }

        private void AddRecordsToSDNSiteData(
            SpeciallyDesignatedNationalsListSiteData SiteData,
            List<SDNList> Records)
        {
            foreach (SDNList Record in Records)
            {
                var RecordToAdd = new SDNList();
                RecordToAdd.Name = Record.Name;
                RecordToAdd.RecordNumber = Record.RecordNumber;
                RecordToAdd.RowNumber = Record.RowNumber;
                RecordToAdd.Status = Record.Status;
                RecordToAdd.MatchCount = Record.MatchCount;
                RecordToAdd.Links = Record.Links;

                SiteData.SDNListSiteData.Add(Record);
            }
        }

        #endregion

        #region GetSingleComponentMatchedValues

        private int GetSingleComponentMatches(SiteEnum Enum,
            Guid? SiteDataId,
            string[] NameComponents)
        {
            switch (Enum)
            {
                case SiteEnum.FDADebarPage:
                    return GetFDADebarSingleComponent(SiteDataId, NameComponents);

                case SiteEnum.ClinicalInvestigatorInspectionPage:
                    return GetCIILSingleComponent(SiteDataId, NameComponents);

                case SiteEnum.FDAWarningLettersPage:
                    return GetFDAWarningSingleComponent(SiteDataId, NameComponents);

                case SiteEnum.ERRProposalToDebarPage:
                    return GetERRSingleComponent(SiteDataId, NameComponents);

                case SiteEnum.AdequateAssuranceListPage:
                    return GetAdequateAssuranceSingleComponent(SiteDataId, NameComponents);

                case SiteEnum.ClinicalInvestigatorDisqualificationPage:
                    return GetDisqualificationSingleComponent(SiteDataId, NameComponents);

                case SiteEnum.PHSAdministrativeActionListingPage:
                    return GetPHSSingleComponent(SiteDataId, NameComponents);

                case SiteEnum.CBERClinicalInvestigatorInspectionPage:
                    return GetCBERSingleComponent(SiteDataId, NameComponents);

                case SiteEnum.ExclusionDatabaseSearchPage:
                    return GetExclusionSingleComponent(SiteDataId, NameComponents);

                case SiteEnum.CorporateIntegrityAgreementsListPage:
                    return GetCIASingleComponent(SiteDataId, NameComponents);

                case SiteEnum.SystemForAwardManagementPage:
                    return GetSAMSingleComponent(SiteDataId, NameComponents);

                case SiteEnum.SpeciallyDesignedNationalsListPage:
                    return GetSDNSingleComponent(SiteDataId, NameComponents);

                default: throw new Exception("Invalid Enum");
            }
        }

        private int GetFDADebarSingleComponent(Guid? SiteDataId, string[] NameComponents)
        {
            //var SiteData = _cachedData.GetFDADebarPageLatestCache();
            var SiteData = _UOW.FDADebarPageRepository.FindById(SiteDataId);

            string FullName = null;
            foreach (string Name in NameComponents)
            {
                if (FullName == null)
                    FullName += Name;
                else
                    FullName += " " + Name;
            }

            var Matches = new int[NameComponents.Count()];

            UpdateMatchStatus(SiteData.DebarredPersons, FullName, 0);

            int MatchCount = 0;

            MatchCount = SiteData.DebarredPersons.Where(x =>
            x.MatchCount == 1).Count();

            return MatchCount;
        }

        private int GetCIILSingleComponent(Guid? SiteDataId, string[] NameComponents)
        {
            var SiteData =
                _UOW.ClinicalInvestigatorInspectionListRepository
                .FindById(SiteDataId);

            //var SiteData = _cachedData.GetClinicalInvestigatorInspectionListLatestCache();

            var CIILRecords = _UOW.ClinicalInvestigatorInspectionRepository.GetAll();
            //var CIILRecords = _cachedData.GetClinicalInvestigatorRecordsCache();

            AddRecordsToCIILSiteData(SiteData, CIILRecords);

            string FullName = null;
            foreach (string Name in NameComponents)
            {
                if (FullName == null)
                    FullName += Name;
                else
                    FullName += " " + Name;
            }

            UpdateMatchStatus(SiteData.ClinicalInvestigatorInspectionList, FullName, 0);

            int MatchCount = 0;

            MatchCount = SiteData.ClinicalInvestigatorInspectionList.
                Where(x => x.MatchCount == 1).Count();

            return MatchCount;
        }

        private int GetFDAWarningSingleComponent(Guid? SiteDataId, string[] NameComponents)
        {
            var SiteData =
                _UOW.FDAWarningLettersRepository
                .FindById(SiteDataId);
            //var SiteData = _cachedData.GetFDAWarningLettersLatestCache();

            var FDAWarningRecords = _UOW.FDAWarningRepository.GetAll();
            //var FDAWarningRecords =  _cachedData.GetFDAWarningRecordsCache();

            AddRecordsToWarningLettersSiteData(SiteData, FDAWarningRecords);

            string FullName = null;
            foreach (string Name in NameComponents)
            {
                if (FullName == null)
                    FullName += Name;
                else
                    FullName += " " + Name;
            }

            UpdateMatchStatus(SiteData.FDAWarningLetterList, FullName, 0);

            int MatchCount = 0;

            MatchCount = SiteData.FDAWarningLetterList.Where(
                x => x.MatchCount == 1).Count();

            return MatchCount;
        }

        private int GetERRSingleComponent(Guid? SiteDataId, string[] NameComponents)
        {
            var SiteData =
                _UOW.ERRProposalToDebarRepository
                .FindById(SiteDataId);

            //var SiteData = _cachedData.GetProposalToDebarSiteScanDetailsLatestCache();

            string FullName = null;
            foreach (string Name in NameComponents)
            {
                if (FullName == null)
                    FullName += Name;
                else
                    FullName += " " + Name;
            }

            UpdateMatchStatus(SiteData.ProposalToDebar, FullName, 0);

            int MatchCount = 0;

            MatchCount = SiteData.ProposalToDebar.Where(
                x => x.MatchCount == 1).Count();

            return MatchCount;
        }

        private int GetAdequateAssuranceSingleComponent(Guid? SiteDataId, string[] NameComponents)
        {
            var SiteData =
                _UOW.AdequateAssuranceListRepository
                .FindById(SiteDataId);
            //var SiteData = _cachedData.GetAdequateAssuranceListLatestCache();

            string FullName = null;
            foreach (string Name in NameComponents)
            {
                if (FullName == null)
                    FullName += Name;
                else
                    FullName += " " + Name;
            }

            UpdateMatchStatus(SiteData.AdequateAssurances, FullName, 0);

            int MatchCount = 0;

            MatchCount = SiteData.AdequateAssurances.Where(
                x => x.MatchCount == 1).Count();

            return MatchCount;
        }

        private int GetDisqualificationSingleComponent(Guid? SiteDataId, string[] NameComponents)
        {
            var SiteData =
                _UOW.ClinicalInvestigatorDisqualificationRepository
                .FindById(SiteDataId);
            //var SiteData = _cachedData.GetClinicalInvestigatorDisqualificationLatestCache();

            string FullName = null;
            foreach (string Name in NameComponents)
            {
                if (FullName == null)
                    FullName += Name;
                else
                    FullName += " " + Name;
            }

            UpdateMatchStatus(SiteData.DisqualifiedInvestigatorList, FullName, 0);

            int MatchCount = 0;

            MatchCount = SiteData.DisqualifiedInvestigatorList.Where(
                x => x.MatchCount == 1).Count();

            return MatchCount;
        }

        private int GetPHSSingleComponent(Guid? SiteDataId, string[] NameComponents)
        {
            var SiteData =
                _UOW.PHSAdministrativeActionListingRepository
                .FindById(SiteDataId);
            //var SiteData = _cachedData.GetPHSAdministrativeActionListingLatestCache();

            string FullName = null;
            foreach (string Name in NameComponents)
            {
                if (FullName == null)
                    FullName += Name;
                else
                    FullName += " " + Name;
            }

            UpdateMatchStatus(SiteData.PHSAdministrativeSiteData, FullName, 0);

            int MatchCount = 0;

            MatchCount = SiteData.PHSAdministrativeSiteData.Where(
                x => x.MatchCount == 1).Count();

            return MatchCount;

            //var Matches = new int[NameComponents.Count()];

            //for (int Counter = 0; Counter < NameComponents.Count(); Counter++)
            //{
            //    var Component = NameComponents[Counter];
            //    var ExcludeComponents = FullName.Replace(Component, "");

            //    ExcludeComponents = ExcludeComponents.Trim();

            //    var ExcludeMatches = SiteData.PHSAdministrativeSiteData.Where(x =>
            //    x.FullName.ToLower().Contains(ExcludeComponents.ToLower())).ToList();

            //    var MatchCount = SiteData.PHSAdministrativeSiteData.Where(x =>
            //    x.FullName.ToLower().Contains(Component.ToLower())).Except(ExcludeMatches).Count();

            //    Matches[Counter] = MatchCount;
            //}
        }

        private int GetCBERSingleComponent(Guid? SiteDataId, string[] NameComponents)
        {
            var SiteData =
                _UOW.CBERClinicalInvestigatorRepository
                .FindById(SiteDataId);
            //var SiteData = _cachedData.GetCBERClinicalInvestigatorLatestCache();

            string FullName = null;
            foreach (string Name in NameComponents)
            {
                if (FullName == null)
                    FullName += Name;
                else
                    FullName += " " + Name;
            }

            UpdateMatchStatus(SiteData.ClinicalInvestigator, FullName, 0);

            int MatchCount = 0;

            MatchCount = SiteData.ClinicalInvestigator.Where(
                x => x.MatchCount == 1).Count();

            return MatchCount;
        }

        private int GetExclusionSingleComponent(Guid? SiteDataId, string[] NameComponents)
        {
            return 0;
            //return -1; //returning -1 will not mark the site as review completed automatically
        }

        private int GetCIASingleComponent(Guid? SiteDataId, string[] NameComponents)
        {
            var SiteData =
                _UOW.CorporateIntegrityAgreementRepository
                .FindById(SiteDataId);

            string FullName = null;
            foreach (string Name in NameComponents)
            {
                if (FullName == null)
                    FullName += Name;
                else
                    FullName += " " + Name;
            }

            UpdateMatchStatus(SiteData.CIAListSiteData, FullName, 0);

            int MatchCount = 0;

            MatchCount = SiteData.CIAListSiteData.Where(
                x => x.MatchCount == 1).Count();

            return MatchCount;

            //var Matches = new int[NameComponents.Count()];

            //for (int Counter = 0; Counter < NameComponents.Count(); Counter++)
            //{
            //    var Component = NameComponents[Counter];
            //    var ExcludeComponents = FullName.Replace(Component, "");

            //    ExcludeComponents = ExcludeComponents.Trim();

            //    var ExcludeMatches = SiteData.CIAListSiteData.Where(x =>
            //    x.FullName.ToLower().Contains(ExcludeComponents.ToLower())).ToList();

            //    var MatchCount = SiteData.CIAListSiteData.Where(x =>
            //    x.FullName.ToLower().Contains(Component.ToLower())).Except(ExcludeMatches).Count();

            //    Matches[Counter] = MatchCount;
            //}
        }

        private int GetSAMSingleComponent(Guid? SiteDataId, string[] NameComponents)
        {
            return 0;
            //return -1; //returning -1 will not mark the site as review completed automatically
        }

        private int GetSDNSingleComponent(Guid? SiteDataId, string[] NameComponents)
        {
            return 0;
            //return -1; //returning -1 will not mark the site as review completed automatically
        }
        #endregion

        #region GetSingleComponentMatchedRecords

        private List<Finding> ConvertToFindings(
            IEnumerable<SiteDataItemBase> Records, SiteEnum Enum)
        {
            var Findings = new List<Finding>();

            foreach (SiteDataItemBase Record in Records)
            {
                var Finding = new Finding();
                Finding.SiteEnum = Enum;
                Finding.IsMatchedRecord = true;
                Finding.RowNumberInSource = Record.RowNumber;
                Finding.MatchCount = Record.MatchCount;
                Finding.RecordDetails = Record.RecordDetails;
                Finding.Links = Record.Links;
                if (Record.DateOfInspection.HasValue)
                    Finding.DateOfInspection = Record.DateOfInspection;

                Findings.Add(Finding);
            }
            return Findings;
        }

        public List<Finding> GetSingleComponentMatchedRecords(
            Guid? SiteDataId,
            SiteEnum Enum,
            string FullName)
        {
            switch (Enum)
            {
                case SiteEnum.FDADebarPage:
                    return GetFDADebarMatchedRecords(SiteDataId, FullName);

                case SiteEnum.ClinicalInvestigatorInspectionPage:
                    return GetCIILMatchedRecords(SiteDataId, FullName);

                case SiteEnum.FDAWarningLettersPage:
                    return GetFDAWarningLettersMatchedRecords(SiteDataId, FullName);

                case SiteEnum.ERRProposalToDebarPage:
                    return GetERRMatchedRecords(SiteDataId, FullName);

                case SiteEnum.AdequateAssuranceListPage:
                    return GetAdequateAssuranceMatchedRecords(SiteDataId, FullName);

                case SiteEnum.ClinicalInvestigatorDisqualificationPage:
                    return GetDisqualificationMatchedRecords(SiteDataId, FullName);

                case SiteEnum.CBERClinicalInvestigatorInspectionPage:
                    return GetCBERMatchedRecords(SiteDataId, FullName);

                case SiteEnum.PHSAdministrativeActionListingPage:
                    return GetPHSMatchedRecords(SiteDataId, FullName);

                case SiteEnum.ExclusionDatabaseSearchPage:
                    return GetExclusionMatchedRecords(SiteDataId, FullName);

                case SiteEnum.CorporateIntegrityAgreementsListPage:
                    return GetCIAMatchedRecords(SiteDataId, FullName);

                case SiteEnum.SystemForAwardManagementPage:
                    return GetSAMMatchedRecords(SiteDataId, FullName);

                case SiteEnum.SpeciallyDesignedNationalsListPage:
                    return GetSDNMatchedRecords(SiteDataId, FullName);

                default: throw new Exception("Invalid Enum");
            }
        }

        private List<Finding> GetFDADebarMatchedRecords(Guid? SiteDataId, string FullName)
        {
            //var SiteData = _UOW.FDADebarPageRepository.FindById(SiteDataId);

            //var SiteData = _UOW.FDADebarPageRepository.GetAll()
            //    .Where(x => x.DataExtractionSucceeded == true)
            //    .OrderByDescending(s => s.CreatedOn)
            //    .First();

            var SiteData = _UOW.FDADebarPageRepository.GetLatestDocument();

            if (FullName == null || FullName.Trim().Length == 0)
                throw new Exception("FullName cannot be empty");

            UpdateMatchStatus(SiteData.DebarredPersons, FullName, 0); //include single match count as well

            var MatchedRecords = SiteData.DebarredPersons.Where(x =>
            x.MatchCount == 1)
            .OrderByDescending(x => x.MatchCount)
            .ToList();

            return ConvertToFindings(
                MatchedRecords,
                SiteEnum.FDADebarPage);
        }

        private List<Finding> GetCIILMatchedRecords(Guid? SiteDataId, string FullName)
        {
            var SiteData = _UOW.ClinicalInvestigatorInspectionListRepository
                .FindById(SiteDataId);

            if (FullName == null || FullName.Trim().Length == 0)
                throw new Exception("FullName cannot be empty");

            var CIILRecords = _UOW.ClinicalInvestigatorInspectionRepository.GetAll();

            AddRecordsToCIILSiteData(SiteData, CIILRecords);

            UpdateMatchStatus(SiteData.ClinicalInvestigatorInspectionList, FullName, 0); //include single match count as well

            var MatchedRecords = SiteData.ClinicalInvestigatorInspectionList
                .Where(x =>
                x.MatchCount == 1)
                .OrderByDescending(x => x.MatchCount)
                .ToList();

            return ConvertToFindings(
                MatchedRecords,
                SiteEnum.ClinicalInvestigatorInspectionPage);
        }

        private List<Finding> GetFDAWarningLettersMatchedRecords(Guid? SiteDataId, string FullName)
        {
            var SiteData = _UOW.FDAWarningLettersRepository
                .FindById(SiteDataId);

            if (FullName == null || FullName.Trim().Length == 0)
                throw new Exception("FullName cannot be empty");

            var FDAWarningRecords = _UOW.FDAWarningRepository.GetAll();
            AddRecordsToWarningLettersSiteData(SiteData, FDAWarningRecords);

            UpdateMatchStatus(SiteData.FDAWarningLetterList, FullName, 0); //include single match count as well

            var MatchedRecords = SiteData.FDAWarningLetterList.Where(x =>
            x.MatchCount == 1)
            .OrderByDescending(x => x.MatchCount)
            .ToList();

            //only for FDAWarningLetters
            ConvertFDAWarningLinks(SiteData.FDAWarningLetterList);

            return ConvertToFindings(
                MatchedRecords,
                SiteEnum.FDAWarningLettersPage);
        }

        private List<Finding> GetERRMatchedRecords(Guid? SiteDataId, string FullName)
        {
            //var SiteData = _UOW.ERRProposalToDebarRepository
            //.FindById(SiteDataId);

            //var SiteData = _UOW.ERRProposalToDebarRepository.GetAll()
            //    .Where(x => x.DataExtractionSucceeded == true)
            //    .OrderByDescending(s => s.CreatedOn)
            //    .First();
            var SiteData = _UOW.ERRProposalToDebarRepository.GetLatestDocument();


            if (FullName == null || FullName.Trim().Length == 0)
                throw new Exception("FullName cannot be empty");

            UpdateMatchStatus(SiteData.ProposalToDebar, FullName, 0); //include single match count as well

            var MatchedRecords = SiteData.ProposalToDebar.Where(x =>
            x.MatchCount == 1)
            .OrderByDescending(x => x.MatchCount)
            .ToList();

            return ConvertToFindings(
                MatchedRecords,
                SiteEnum.ERRProposalToDebarPage);
        }

        private List<Finding> GetAdequateAssuranceMatchedRecords(Guid? SiteDataId, string FullName)
        {
            //var SiteData = _UOW.AdequateAssuranceListRepository
            //    .FindById(SiteDataId);

            //var SiteData = _UOW.AdequateAssuranceListRepository.GetAll()
            //    .Where(x => x.DataExtractionSucceeded == true)
            //    .OrderByDescending(s => s.CreatedOn)
            //    .First();
            var SiteData = _UOW.AdequateAssuranceListRepository.GetLatestDocument();

            if (FullName == null || FullName.Trim().Length == 0)
                throw new Exception("FullName cannot be empty");

            UpdateMatchStatus(SiteData.AdequateAssurances, FullName, 0); //include single match count as well

            var MatchedRecords = SiteData.AdequateAssurances.Where(x =>
            x.MatchCount == 1)
            .OrderByDescending(x => x.MatchCount)
            .ToList();

            return ConvertToFindings(
                MatchedRecords,
                SiteEnum.AdequateAssuranceListPage);
        }

        private List<Finding> GetDisqualificationMatchedRecords(Guid? SiteDataId, string FullName)
        {
            //var SiteData = _UOW.ClinicalInvestigatorDisqualificationRepository
            //    .FindById(SiteDataId);

            //var SiteData = _UOW.ClinicalInvestigatorDisqualificationRepository.GetAll()
            //    .Where(x => x.DataExtractionSucceeded == true)
            //    .OrderByDescending(s => s.CreatedOn)
            //    .First();

            var SiteData = _UOW.ClinicalInvestigatorDisqualificationRepository.GetLatestDocument();

            if (FullName == null || FullName.Trim().Length == 0)
                throw new Exception("FullName cannot be empty");

            UpdateMatchStatus(SiteData.DisqualifiedInvestigatorList, FullName, 0); //include single match count as well

            var MatchedRecords = SiteData.DisqualifiedInvestigatorList.Where(x =>
            x.MatchCount == 1)
            .OrderByDescending(x => x.MatchCount)
            .ToList();

            return ConvertToFindings(
                MatchedRecords,
                SiteEnum.ClinicalInvestigatorDisqualificationPage);
        }

        private List<Finding> GetCBERMatchedRecords(Guid? SiteDataId, string FullName)
        {
            //var SiteData = _UOW.CBERClinicalInvestigatorRepository
            //    .FindById(SiteDataId);

            //var SiteData = _UOW.CBERClinicalInvestigatorRepository.GetAll()
            //    .Where(x => x.DataExtractionSucceeded == true)
            //    .OrderByDescending(s => s.CreatedOn)
            //    .First();

            var SiteData = _UOW.CBERClinicalInvestigatorRepository.GetLatestDocument();

            if (FullName == null || FullName.Trim().Length == 0)
                throw new Exception("FullName cannot be empty");

            UpdateMatchStatus(SiteData.ClinicalInvestigator, FullName, 0); //include single match count as well

            var MatchedRecords = SiteData.ClinicalInvestigator.Where(x =>
            x.MatchCount == 1)
            .OrderByDescending(x => x.MatchCount)
            .ToList();

            return ConvertToFindings(
                MatchedRecords,
                SiteEnum.CBERClinicalInvestigatorInspectionPage);
        }

        private List<Finding> GetPHSMatchedRecords(Guid? SiteDataId, string FullName)
        {
            //var SiteData = _UOW.PHSAdministrativeActionListingRepository
            //    .FindById(SiteDataId);

            //var SiteData = _UOW.PHSAdministrativeActionListingRepository.GetAll()
            //    .Where(x => x.DataExtractionSucceeded == true)
            //    .OrderByDescending(s => s.CreatedOn)
            //    .First();

            var SiteData = _UOW.PHSAdministrativeActionListingRepository.GetLatestDocument();

            if (FullName == null || FullName.Trim().Length == 0)
                throw new Exception("FullName cannot be empty");

            UpdateMatchStatus(SiteData.PHSAdministrativeSiteData, FullName, 0); //include single match count as well

            var MatchedRecords = SiteData.PHSAdministrativeSiteData.Where(x =>
            x.MatchCount == 1)
            .OrderByDescending(x => x.MatchCount)
            .ToList();

            return ConvertToFindings(
                MatchedRecords,
                SiteEnum.PHSAdministrativeActionListingPage);
        }

        private List<Finding> GetExclusionMatchedRecords(Guid? SiteDataId, string FullName)
        {
            var SiteData = _UOW.ExclusionDatabaseSearchRepository
                .FindById(SiteDataId);

            if (FullName == null || FullName.Trim().Length == 0)
                throw new Exception("FullName cannot be empty");

            UpdateMatchStatus(SiteData.ExclusionSearchList, FullName); //include single match count as well

            var MatchedRecords = SiteData.ExclusionSearchList.Where(x =>
            x.MatchCount == 1)
            .OrderByDescending(x => x.MatchCount)
            .ToList();

            return ConvertToFindings(
                MatchedRecords,
                SiteEnum.ExclusionDatabaseSearchPage);
        }

        private List<Finding> GetCIAMatchedRecords(Guid? SiteDataId, string FullName)
        {
            //var SiteData = _UOW.CorporateIntegrityAgreementRepository
            //    .FindById(SiteDataId);

            //var SiteData = _UOW.CorporateIntegrityAgreementRepository.GetAll()
            //    .Where(x => x.DataExtractionSucceeded == true)
            //    .OrderByDescending(s => s.CreatedOn)
            //    .First();

            var SiteData = _UOW.CorporateIntegrityAgreementRepository.GetLatestDocument();

            if (FullName == null || FullName.Trim().Length == 0)
                throw new Exception("FullName cannot be empty");

            UpdateMatchStatus(SiteData.CIAListSiteData, FullName, 0); //include single match count as well

            var MatchedRecords = SiteData.CIAListSiteData.Where(x =>
            x.MatchCount == 1)
            .OrderByDescending(x => x.MatchCount)
            .ToList();

            return ConvertToFindings(
                MatchedRecords,
                SiteEnum.CorporateIntegrityAgreementsListPage);
        }

        private List<Finding> GetSAMMatchedRecords(Guid? SiteDataId, string FullName)
        {
            var SiteData = _UOW.SystemForAwardManagementRepository
                .FindById(SiteDataId);

            if (FullName == null || FullName.Trim().Length == 0)
                throw new Exception("FullName cannot be empty");

            UpdateMatchStatus(SiteData.SAMSiteData, FullName); //include single match count as well

            var MatchedRecords = SiteData.SAMSiteData.Where(x =>
            x.MatchCount == 1)
            .OrderByDescending(x => x.MatchCount)
            .ToList();

            return ConvertToFindings(
                MatchedRecords,
                SiteEnum.SystemForAwardManagementPage);
        }

        private List<Finding> GetSDNMatchedRecords(Guid? SiteDataId, string FullName)
        {
            var SiteData = _UOW.SpeciallyDesignatedNationalsRepository
                .FindById(SiteDataId);

            if (FullName == null || FullName.Trim().Length == 0)
                throw new Exception("FullName cannot be empty");

            UpdateMatchStatus(SiteData.SDNListSiteData, FullName); //include single match count as well

            var MatchedRecords = SiteData.SDNListSiteData.Where(x =>
            x.MatchCount == 1)
            .OrderByDescending(x => x.MatchCount)
            .ToList();

            return ConvertToFindings(
                MatchedRecords,
                SiteEnum.SpeciallyDesignedNationalsListPage);
        }

        #endregion

        #region OutputFile

        public MemoryStream GenerateOutputFile(
            IGenerateOutputFile GenerateOutputFile,
            List<ComplianceForm> forms)
        {
            int Row = 2;

            foreach (ComplianceForm form in forms)
            {
                DateTime? WorldCheckCompletedOn = null;
                DateTime? InsWorldCheckCompletedOn = null;
                DateTime? DMCCheckCompletedOn = null;

                foreach (InvestigatorSearched Investigator in form.InvestigatorDetails)
                {
                    //var InvestigatorWorldCheck =
                    //    form.SiteSources.Find(x =>
                    //    x.SiteType == SiteTypeEnum.WorldCheck &&
                    //    x.SearchAppliesTo == SearchAppliesToEnum.PIs);

                    //if (InvestigatorWorldCheck != null)
                    //    WorldCheckCompletedOn = InvestigatorWorldCheck.SiteSourceUpdatedOn;

                    WorldCheckCompletedOn = InvestigatorWorldCheckCompletedOn(form.SiteSources);

                    //var InstituteWorldCheck = form.SiteSources.Find(x =>
                    //x.SiteType == SiteTypeEnum.WorldCheck &&
                    //x.SearchAppliesTo == SearchAppliesToEnum.Institute);

                    //if (InstituteWorldCheck != null)
                    //    InsWorldCheckCompletedOn =
                    //        InstituteWorldCheck.SiteSourceUpdatedOn;

                    InsWorldCheckCompletedOn = InstituteWorldCheckCompletedOn(form.SiteSources);

                    //var DMCExclusion = form.SiteSources.Find(x =>
                    //x.SiteType == SiteTypeEnum.DMCExclusion);

                    //if (DMCExclusion != null)
                    //    DMCCheckCompletedOn = DMCExclusion.SiteSourceUpdatedOn;

                    DMCCheckCompletedOn = DMCExclusionCompletedOn(form.SiteSources);

                    if (Investigator.ReviewCompletedOn != null)
                    {
                        GenerateOutputFile.AddInvestigator(
                            Investigator.InvestigatorId,
                            Investigator.MemberId,
                            Investigator.FirstName,
                            Investigator.LastName,
                            1,
                            Investigator.ReviewCompletedOn,
                            Investigator.ReviewCompletedOn,
                            WorldCheckCompletedOn,
                            1,
                            WorldCheckCompletedOn,
                            1,
                            InsWorldCheckCompletedOn,
                            //InstituteWorldCheck != null ? ToYesNoString(InstituteWorldCheck.IssuesIdentified) : "",
                            ToYesNoString(InstituteComplianceIssue(form.SiteSources)),
                            DMCCheckCompletedOn,
                            //DMCExclusion != null ? ToYesNoString(DMCExclusion.IssuesIdentified) : "",
                            DMCExclusionIssuesIdentified(form.SiteSources),
                            Row);
                        Row += 1;
                        WorldCheckCompletedOn = null;
                        InsWorldCheckCompletedOn = null;
                        DMCCheckCompletedOn = null;
                    }
                }
            }
            var OutputFileName = "OutputFile_" +
                DateTime.Now.ToString("dd_MMM_yyyy HH_mm") +
                ".xlsx";

            //GenerateOutputFile.SaveChanges(_config.OutputFileFolder +
            //    OutputFileName);

            //return _config.OutputFileFolder + OutputFileName;
            return GenerateOutputFile.GetMemoryStream();
        }

        private DateTime? InvestigatorWorldCheckCompletedOn(List<SiteSource> Sites)
        {
            DateTime? InvWorldCheckCompletedOn = null;

            var InvWorldCheck = Sites.Find(x =>
                x.SiteType == SiteTypeEnum.WorldCheck &&
                x.SearchAppliesTo == SearchAppliesToEnum.PIs);

            if (InvWorldCheck != null)
                InvWorldCheckCompletedOn = InvWorldCheck.SiteSourceUpdatedOn;

            return InvWorldCheck != null ? InvWorldCheck.SiteSourceUpdatedOn : InvWorldCheckCompletedOn;
        }

        private DateTime? InstituteWorldCheckCompletedOn(List<SiteSource> Sites)
        {
            DateTime? InstituteWorldCheckCompletedOn = null;

            var InsWorldCheck = Sites.Find(x =>
                x.SiteType == SiteTypeEnum.WorldCheck &&
                x.SearchAppliesTo == SearchAppliesToEnum.Institute);

            if (InsWorldCheck != null)
                InstituteWorldCheckCompletedOn = InsWorldCheck.SiteSourceUpdatedOn;

            return InsWorldCheck != null ? InsWorldCheck.SiteSourceUpdatedOn : InstituteWorldCheckCompletedOn;
        }

        private bool InstituteComplianceIssue(List<SiteSource> Sites)
        {
            var InsWorldCheck = Sites.Find(x =>
                x.SiteType == SiteTypeEnum.WorldCheck &&
                x.SearchAppliesTo == SearchAppliesToEnum.Institute);

            return InsWorldCheck != null ? InsWorldCheck.IssuesIdentified : false;
        }

        private DateTime? DMCExclusionCompletedOn(List<SiteSource> Sites)
        {
            DateTime? DMCCheckCompletedOn = null;

            var DMCCheck = Sites.Find(x =>
                x.SiteType == SiteTypeEnum.DMCExclusion);

            if (DMCCheck != null)
                DMCCheckCompletedOn = DMCCheck.SiteSourceUpdatedOn;

            return DMCCheck != null ? DMCCheck.SiteSourceUpdatedOn : DMCCheckCompletedOn;
        }

        private string DMCExclusionIssuesIdentified(List<SiteSource> Sites)
        {
            var DMCExclusion = Sites.Find(x =>
                x.SiteType == SiteTypeEnum.DMCExclusion);

            if (DMCExclusion != null)
                return ToYesNoString(DMCExclusion.IssuesIdentified);
            else
                return null;
        }
        #endregion

        #region Helpers

        private string RemoveExtraCharacters(string Value)
        {
            return Regex.Replace(Value, "[,.'/;]", " ");
        }


        private static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '_'
                    || c == ' ')
                    sb.Append(c);
            }
            return sb.ToString();
        }

        private string AddSpaceBetweenWords(string Name)
        {
            string res = Regex.Replace(Name, "[A-Z]", " $0").Trim();
            return res;
        }

        private string ToYesNoString(bool Value)
        {
            return Value ? "Yes" : "No";
        }
        #endregion

        #region ExcelValidations

        private List<string> ValidateExcelInputs(ExcelInputRow InputRow, int Row)
        {
            var ValidationMessages = new List<string>();

            if (Row == 2 && InputRow.Role.ToLower() != "pi")
                ValidationMessages.Add(
                    "RowNumber: 2 - First Investigator must be a PI");

            var FullName = InputRow.FullName;

            var InvComponent = FullName.Split(' ').Count();

            if (InvComponent <= 1)
                ValidationMessages.Add("Row number: " + Row +
                    " - please provide at least two components to search - First Name/Middle Name/Last Name");

            var Components = FullName.Split(' ');

            foreach (string Component in Components)
            {
                if (Component.Trim().Length == 1)
                    ValidationMessages.Add("Row number: " + Row +
                        " - FirstName/Middle Name/Last Name - single characters are not " +
                        "accepted. Please provide two or more characters to search");
                else if (Component.Trim().Length == 2 &&
                    HasSpecialCharacters(Component.Trim()) ||
                    Component.Trim().Contains("."))
                    ValidationMessages.Add("Row number: " + Row +
                        " - FirstName/Middle Name/Last Name - special characters are not " +
                        "accepted. Please provide two or more characters to search");
            }

            //if(InputRow.FirstName.Trim().Length > 0 &&
            //    HasSpecialCharacters(InputRow.FirstName))
            //{
            //    ValidationMessages.Add("Row number: " + Row +
            //        " - FirstName has special characters. Remove special characters " +
            //        "and upload");
            //}

            //if (InputRow.MiddleName.Trim().Length > 0 &&
            //    HasSpecialCharacters(InputRow.MiddleName))
            //{
            //    ValidationMessages.Add("Row number: " + Row +
            //        " - Middle Name has special characters. Remove special characters " +
            //        "and upload");
            //}

            //if (InputRow.LastName.Trim().Length > 0 &&
            //    HasSpecialCharacters(InputRow.LastName))
            //{
            //    ValidationMessages.Add("Row number: " + Row +
            //        " - First Name has special characters. Remove special characters " +
            //        "and upload");
            //}

            if (InputRow.DisplayName == null || InputRow.DisplayName.Trim() == "")
                ValidationMessages.Add("Row number: " + Row +
                    " - Display Name is mandatory");

            if (InputRow.Role.ToLower() != "pi" &&
                InputRow.Role.ToLower() != "sub i")
                ValidationMessages.Add("Row number: " + Row +
                    " - Role column should have either 'PI' or 'Sub I'");

            if (InputRow.Role.Length > 9)
                ValidationMessages.Add("RowNumber: " + Row +
                    " - Role column exceeds max character(9) limit");

            if (!IsValidProjectNumber(InputRow.ProjectNumber))
                ValidationMessages.Add("RowNumber: " + Row +
                    " - change the project number format to '1234/5678'");
            if (InputRow.ProjectNumber2 != null &&
                InputRow.ProjectNumber2.Trim() != ""
                && !IsValidProjectNumber(InputRow.ProjectNumber2))
                ValidationMessages.Add("RowNumber: " + Row +
                    " - change the project number format to '1234/5678'");

            if (InputRow.DisplayName.Trim().Length == 0)
                ValidationMessages.Add("RowNumber: " + Row +
                    " - Investigator Name With Qualification (ICSF) is missing");

            #region MaxCharacterValidation
            //if(InputRow.InvestigatorID.Length > 10)
            //    ValidationMessages.Add("RowNumber: " + Row +
            //        " - Investigator ID should not be more than 10 characters");

            //if (InputRow.MemeberID.Length > 10)
            //    ValidationMessages.Add("RowNumber: " + Row +
            //        " - Member ID should not be more than 10 characters");

            //if(InputRow.InstituteName.Length > 500)
            //    ValidationMessages.Add("RowNumber: " + Row +
            //        " - Institute Name should not be more than 500 characters");

            //if (InputRow.AddressLine1.Length > 500)
            //    ValidationMessages.Add("RowNumber: " + Row +
            //        " - Address Line 1 should not be more than 500 characters");

            //if (InputRow.AddressLine2.Length > 500)
            //    ValidationMessages.Add("RowNumber: " + Row +
            //        " - Address Line 2 should not be more than 500 characters");

            //if (InputRow.City.Length > 20)
            //    ValidationMessages.Add("RowNumber: " + Row +
            //        " - City should not be more than 20 characters");

            //if (InputRow.State.Length > 20)
            //    ValidationMessages.Add("RowNumber: " + Row +
            //        " - State should not be more than 20 characters");

            //if (InputRow.PostalCode.Length > 10)
            //    ValidationMessages.Add("RowNumber: " + Row +
            //        " - Postal Code should not be more than 10 characters");

            //if (InputRow.Country.Length > 20)
            //    ValidationMessages.Add("RowNumber: " + Row +
            //        " - Country should not be more than 20 characters");

            //if (InputRow.MedicalLicenseNumber.Length > 20)
            //    ValidationMessages.Add("RowNumber: " + Row +
            //        " - Medical License Number should not be more than 20 characters");
            #endregion

            return ValidationMessages;
        }

        private bool IsNumericOrHasSpecialCharacters(string Value)
        {
            foreach (char c in Value)
            {
                if ((c >= '0' && c <= '9'))
                    return true;
            }
            return HasSpecialCharacters(Value) ? true : false;
        }

        private bool HasSpecialCharacters(string Value)
        {
            foreach (char c in Value)
            {
                if (c == '?' || c == '\\' ||
                    c == '$' || c == '#' ||
                    c == '*' ||
                    c == '_' || c == '&' ||
                    c == '@' || c == '!' ||
                    c == '%' || c == '^')
                    return true;
            }
            return false;
        }

        private bool IsValidProjectNumber(string Value)
        {
            string Expression = "^\\d{4}/\\d{4}$";
            return Regex.IsMatch(Value, Expression);
        }

        #endregion

        public void Dispose()
        {
            Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
