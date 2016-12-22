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
        #region Not in use
        public List<RowData> ReadDataFromExcelFile(string FilePath)
        {
            var forms = new List<ComplianceForm>();
            var ListOfRows = new List<RowData>();

            using (SpreadsheetDocument doc =
                   SpreadsheetDocument.Open(FilePath, false))
            {
                WorkbookPart workbookPart = doc.WorkbookPart;
                WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();

                //var sheetData = doc.WorkbookPart.Workbook.Sheets.GetFirstChild<Sheet>();
                //var tempDetailsInEachRow = new List<RowData>();

                foreach (Row row in sheetData.Elements<Row>())
                {
                    if (row.Elements<Cell>().ElementAt(0).CellValue.InnerText != "")
                    {
                        foreach (Cell cell in row.Elements<Cell>())
                        {

                            var Rows = new RowData();
                            var Details = new List<string>();

                            string Name = row.Elements<Cell>().ElementAt(0).
                                                CellValue.InnerText.ToString();

                            string ProjectNumber = row.Elements<Cell>().ElementAt(1).
                                                CellValue.InnerText;

                            Details.Add(row.Elements<Cell>().ElementAt(0).
                                                CellValue.InnerText); //PI Name
                            Details.Add(row.Elements<Cell>().ElementAt(1).
                                                CellValue.InnerText); //Project#
                            Details.Add(row.Elements<Cell>().ElementAt(2).
                                                CellValue.InnerText); //SponsorProtocol#
                            Details.Add(row.Elements<Cell>().ElementAt(3).
                                                CellValue.InnerText); //Address
                            Details.Add(row.Elements<Cell>().ElementAt(4).
                                                CellValue.InnerText); //Country

                            int ColumnIndex = 5;

                            while (row.Elements<Cell>().ElementAt(ColumnIndex).CellValue.InnerText != "")
                            {
                                Details.Add(
                                    row.Elements<Cell>().ElementAt(ColumnIndex).
                                                    CellValue.Text); //SI Name

                                Details.Add(
                                    row.Elements<Cell>().ElementAt(ColumnIndex + 1).
                                                    CellValue.Text); //MedicalLicenseNumber

                                Details.Add(
                                    row.Elements<Cell>().ElementAt(ColumnIndex + 2).
                                                    CellValue.Text); //Qualification

                                ColumnIndex += 3;
                            }
                            Rows.DetailsInEachRow = Details;
                            ListOfRows.Add(Rows);
                        }
                    }
                    else
                        break;
                }
            }
            return ListOfRows;
        }
        #endregion

        public List<RowData> ReadData(string FilePath)
        {
            SLDocument doc = new SLDocument(FilePath);

            if (doc.GetCellValueAsString("A1").ToLower() != "pi name")
                return null;

            int RowIndex = 2;

            var ListOfRows = new List<RowData>();

            while (doc.HasCellValue("A" + RowIndex))
            {
                var RowData = new RowData();
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
                    DataFromEachRow.Add(doc.GetCellValueAsString(RowIndex, ColumnIndex)); //SI Name
                    DataFromEachRow.Add(doc.GetCellValueAsString(RowIndex, ColumnIndex + 1)); //ML#
                    DataFromEachRow.Add(doc.GetCellValueAsString(RowIndex, ColumnIndex + 2)); //Qualification

                    ColumnIndex += 3;
                }
                RowData.DetailsInEachRow = DataFromEachRow;
                ListOfRows.Add(RowData);

                RowIndex += 1;
            }
            return ListOfRows;
        }

        //yet to be done
        public void CreateComplianceForm()
        {

        }
    }


    public class RowData
    {
        public List<string> DetailsInEachRow { get; set; }
    }
}
