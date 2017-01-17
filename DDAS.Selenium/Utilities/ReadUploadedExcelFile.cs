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

            if (doc.GetCellValueAsString("A1").ToLower() != "pi name")
                return null;

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
                    DataFromEachRow.Add(doc.GetCellValueAsString(RowIndex, ColumnIndex)); //SI Name
                    DataFromEachRow.Add(doc.GetCellValueAsString(RowIndex, ColumnIndex + 1)); //ML#
                    DataFromEachRow.Add(doc.GetCellValueAsString(RowIndex, ColumnIndex + 2)); //Qualification

                    ColumnIndex += 3;
                }
                //RowData.Add(DataFromEachRow);
                //ListOfRows.Add(RowData);

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
