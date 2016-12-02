using DDAS.Models.Entities.Domain;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class ReadUploadedExcelFile
    {
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
                                ColumnIndex += 1;
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

        public List<RowData> ReadData(string FilePath)
        {
            SLDocument doc = new SLDocument(FilePath);

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

                int ColumnIndex = 6;
                while (doc.HasCellValue(RowIndex, ColumnIndex) == true)
                {
                    DataFromEachRow.Add(doc.GetCellValueAsString(RowIndex, ColumnIndex));
                    ColumnIndex += 1;
                }
                RowData.DetailsInEachRow = DataFromEachRow;
                ListOfRows.Add(RowData);

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
