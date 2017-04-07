using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using SpreadsheetLight;
using System.Collections.Generic;
using System.Linq;

namespace Utilities
{
    public class ReadUploadedExcelFile
    {
        #region OldFormat
        public List<List<string>> ReadData(string FilePath)
        {
            SLDocument doc = new SLDocument(FilePath);

            var ValidationMessages = new List<string>();
            var ListOfRows = new List<List<string>>();

            if (doc.GetCellValueAsString("A1").ToLower() != "pi name")
                ValidationMessages.Add("cannot find column - PI Name in cell A1");
            if (doc.GetCellValueAsString("B1").ToLower() != ("pi medical license #"))
                ValidationMessages.Add("cannot find column - PI Medical license # in cell B1");
            if (doc.GetCellValueAsString("C1").ToLower() != "pi qualification")
                ValidationMessages.Add("cannot find column - PI Qualification in cell C1");
            if (doc.GetCellValueAsString("D1").ToLower() != "project number")
                ValidationMessages.Add("cannot find column - Project Number in cell D1");
            if (doc.GetCellValueAsString("E1").ToLower() != "sponsor protocol #")
                ValidationMessages.Add("cannot find column - Sponsor Protocol # in cell E1");
            if (doc.GetCellValueAsString("F1").ToLower() != "institute name")
                ValidationMessages.Add("cannot find column - Institute Name in cell F1");
            if (doc.GetCellValueAsString("G1").ToLower() != "address")
                ValidationMessages.Add("cannot find column - Address in cell G1");
            if (doc.GetCellValueAsString("H1").ToLower() != "country")
                ValidationMessages.Add("cannot find column - Country in cell H1");

            int Column = 9;

            while(doc.HasCellValue(1, Column) == true)
            {
                if (doc.GetCellValueAsString(1, Column).ToLower() !=
                    "sub investigator")
                    ValidationMessages.Add("cannot find column - Sub Investigator");

                if (doc.GetCellValueAsString(1, Column + 1).ToLower() !=
                    "sub investigator ml#")
                    ValidationMessages.Add("cannot find column - Sub Investigator ML#");

                if (doc.GetCellValueAsString(1, Column + 2).ToLower() !=
                    "si qualification")
                    ValidationMessages.Add("cannot find column - SI Qualification");

                Column += 3;
            }

            if (ValidationMessages.Count > 0)
            {
                ListOfRows.Add(ValidationMessages);
                return ListOfRows;
            }

            int TotalColumns = Column - 1;
            int RowIndex = 2;

            while (doc.HasCellValue("A" + RowIndex))
            {
                var DataFromEachRow = new List<string>();

                DataFromEachRow.Add(doc.GetCellValueAsString("A" + RowIndex));
                DataFromEachRow.Add(doc.GetCellValueAsString("B" + RowIndex));
                DataFromEachRow.Add(doc.GetCellValueAsString("C" + RowIndex));
                DataFromEachRow.Add(doc.GetCellValueAsString("D" + RowIndex));
                DataFromEachRow.Add(doc.GetCellValueAsString("E" + RowIndex));
                DataFromEachRow.Add(doc.GetCellValueAsString("F" + RowIndex));
                DataFromEachRow.Add(doc.GetCellValueAsString("G" + RowIndex));
                DataFromEachRow.Add(doc.GetCellValueAsString("H" + RowIndex));

                int ColumnIndex = 9;
                while (ColumnIndex <= TotalColumns)
                {
                     //if (!doc.HasCellValue(RowIndex, ColumnIndex) &&
                    //    (doc.HasCellValue(RowIndex, ColumnIndex + 1) ||
                    //    doc.HasCellValue(RowIndex, ColumnIndex + 2)))
                    //    ValidationMessages.Add("Sub Investigator Name is empty " +
                    //        "at row:" + RowIndex +
                    //        " and column: " + ColumnIndex);
                    if (doc.HasCellValue(RowIndex, ColumnIndex))
                    {
                        DataFromEachRow.Add(
                            doc.GetCellValueAsString(RowIndex, ColumnIndex)); //SI Name

                        DataFromEachRow.Add(
                            doc.GetCellValueAsString(RowIndex, ColumnIndex + 1)); //ML#

                        DataFromEachRow.Add(
                            doc.GetCellValueAsString(RowIndex, ColumnIndex + 2)); //Qualification
                    }
                    ColumnIndex += 3;
                }

                if (ValidationMessages.Count > 0)
                {
                    ListOfRows.Add(ValidationMessages);
                    break;
                }
                else
                    ListOfRows.Add(DataFromEachRow);

                RowIndex += 1;
            }
            return ListOfRows;
        }
        #endregion

        public List<string> ReadDataFromExcel(string FilePath, int RowIndex)
        {
            SLDocument doc = new SLDocument(FilePath);

            var ValidationMessages = new List<string>();

            if(doc.GetCellValueAsString("A1").ToLower() != "role (principal/sub)")
                ValidationMessages.Add("cannot find column - Role (Principal/Sub) in cell A1");
            if (doc.GetCellValueAsString("B1").ToLower() != "project number")
                ValidationMessages.Add("cannot find column - Project Number in cell B1");
            if (doc.GetCellValueAsString("C1").ToLower() != "sponsor protocol number")
                ValidationMessages.Add("cannot find column - Sponsor Protocol Number in cell C1");
            if (doc.GetCellValueAsString("D1").ToLower() != "investigator name with qualification (icsf)")
                ValidationMessages.Add("cannot find column - Investigator Name with Qualification (ICSF) in cell D1");
            if (doc.GetCellValueAsString("E1").ToLower() != "lead/investigator id")
                ValidationMessages.Add("cannot find column - Lead/Investigator ID in cell E1");
            if (doc.GetCellValueAsString("F1").ToLower() != "member id")
                ValidationMessages.Add("cannot find column - Member ID in cell F1");
            if (doc.GetCellValueAsString("G1").ToLower() != ("first name"))
                ValidationMessages.Add("cannot find column - First Name in cell G1");
            if (doc.GetCellValueAsString("H1").ToLower() != ("middle name"))
                ValidationMessages.Add("cannot find column - Middle Name in cell H1");
            if (doc.GetCellValueAsString("I1").ToLower() != "last name")
                ValidationMessages.Add("cannot find column - Last Name in cell I1");
            if (doc.GetCellValueAsString("J1").ToLower() != "selected institute")
                ValidationMessages.Add("cannot find column - Selected Institute in cell J1");
            if (doc.GetCellValueAsString("K1").ToLower() != "institute address line 1")
                ValidationMessages.Add("cannot find column - Institute Address Line 1 in cell K1");
            if (doc.GetCellValueAsString("L1").ToLower() != "institute address line 2")
                ValidationMessages.Add("cannot find column - Institute Address Line 2 in cell L1");
            if (doc.GetCellValueAsString("M1").ToLower() != "institute city")
                ValidationMessages.Add("cannot find column - Institute City in cell M1");
            if (doc.GetCellValueAsString("N1").ToLower() != "institute state/province")
                ValidationMessages.Add("cannot find column - Institute State/Province in cell N1");
            if (doc.GetCellValueAsString("O1").ToLower() != "institute zip/postal code")
                ValidationMessages.Add("cannot find column - Institute Zip/Postal Code in cell O1");
            if (doc.GetCellValueAsString("P1").ToLower() != "member country")
                ValidationMessages.Add("cannot find column - Member Country in cell P1");
            if (doc.GetCellValueAsString("Q1").ToLower() != "medical license number")
                ValidationMessages.Add("cannot find column - Medical License Number in cell Q1");

            if (ValidationMessages.Count > 0)
                return ValidationMessages;

            var ComplianceForm = new List<string>();
            ComplianceForm.Add(doc.GetCellValueAsString("A" + (RowIndex)));
            ComplianceForm.Add(doc.GetCellValueAsString("B" + (RowIndex)));
            ComplianceForm.Add(doc.GetCellValueAsString("C" + (RowIndex)));
            ComplianceForm.Add(doc.GetCellValueAsString("D" + (RowIndex)));
            ComplianceForm.Add(doc.GetCellValueAsString("E" + (RowIndex)));
            ComplianceForm.Add(doc.GetCellValueAsString("F" + (RowIndex)));
            ComplianceForm.Add(doc.GetCellValueAsString("G" + (RowIndex)));
            ComplianceForm.Add(doc.GetCellValueAsString("H" + (RowIndex)));
            ComplianceForm.Add(doc.GetCellValueAsString("I" + (RowIndex)));
            ComplianceForm.Add(doc.GetCellValueAsString("J" + (RowIndex)));
            ComplianceForm.Add(doc.GetCellValueAsString("K" + (RowIndex)));
            ComplianceForm.Add(doc.GetCellValueAsString("L" + (RowIndex)));
            ComplianceForm.Add(doc.GetCellValueAsString("M" + (RowIndex)));
            ComplianceForm.Add(doc.GetCellValueAsString("N" + (RowIndex)));
            ComplianceForm.Add(doc.GetCellValueAsString("O" + (RowIndex)));
            ComplianceForm.Add(doc.GetCellValueAsString("P" + (RowIndex)));
            ComplianceForm.Add(doc.GetCellValueAsString("Q" + (RowIndex)));

            return ComplianceForm;
        }
    }


    public class RowData
    {
        public List<string> DetailsInEachRow { get; set; }
    }
}
