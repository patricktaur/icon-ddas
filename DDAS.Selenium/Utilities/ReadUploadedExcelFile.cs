using DDAS.Models.Entities.Domain;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using SpreadsheetLight;
using System.Collections.Generic;
using System.Linq;

namespace Utilities
{
    public class ReadUploadedExcelFile
    {
        public List<List<string>> ReadData(string FilePath)
        {
            SLDocument doc = new SLDocument(FilePath);

            var ValidationMessages = new List<string>();

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

            int RowIndex = 2;

            var ListOfRows = new List<List<string>>();
            
            //var ListOfRows = new List<RowData>();

            while (doc.HasCellValue("A" + RowIndex))
            {
                //var RowData = new RowData();
                //var RowData = new List<string>();

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
                while (doc.HasCellValue(RowIndex, ColumnIndex) == true)
                {
                    if (doc.GetCellValueAsString(1, ColumnIndex).ToLower() !=
                        "sub investigator")
                        ValidationMessages.Add("cannot find column - Sub Investigator");

                    DataFromEachRow.Add(doc.GetCellValueAsString(RowIndex, ColumnIndex)); //SI Name

                    if (doc.GetCellValueAsString(1, ColumnIndex + 1).ToLower() !=
                        "sub investigator ml#")
                        ValidationMessages.Add("cannot find column - Sub Investigator ML#");

                    DataFromEachRow.Add(doc.GetCellValueAsString(RowIndex, ColumnIndex + 1)); //ML#

                    if (doc.GetCellValueAsString(1, ColumnIndex).ToLower() !=
                        "si qualification")
                        ValidationMessages.Add("cannot find column - SI Qualification");

                    DataFromEachRow.Add(doc.GetCellValueAsString(RowIndex, ColumnIndex + 2)); //Qualification

                    ColumnIndex += 3;
                }
                //RowData.Add(DataFromEachRow);
                //ListOfRows.Add(RowData);

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
    }


    public class RowData
    {
        public List<string> DetailsInEachRow { get; set; }
    }
}
