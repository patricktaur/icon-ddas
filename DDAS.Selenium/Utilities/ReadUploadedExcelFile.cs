using DDAS.Models.Entities.Domain;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class ReadUploadedExcelFile
    {
        public List<ComplianceForm> ReadDataFromExcelFile(string FilePath)
        {
            var forms = new List<ComplianceForm>();

            using (SpreadsheetDocument doc =
                   SpreadsheetDocument.Open(FilePath, false))
            {
                WorkbookPart workbookPart = doc.WorkbookPart;
                WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();

                //bool ReadDataFromEachRow = true;
                var Investigators = new List<InvestigatorSearched>();

                bool ReadDataFromEachRow = true;

                int RowIndex = 1;
                //int CellIndex = 1;

                while (ReadDataFromEachRow)
                {
                    Row row = sheetData.Elements<Row>().ElementAt(RowIndex);

                    if (row.Elements<Cell>().ElementAt(0).CellValue.Text != "")
                    {
                        var form = new ComplianceForm();
                        var Investigator = new InvestigatorSearched();

                        Investigator.Name = row.Elements<Cell>().ElementAt(0).
                                            CellValue.Text;
                        Investigator.Role = row.Elements<Cell>().ElementAt(1).
                                            CellValue.Text;
                        form.ProjectNumber = row.Elements<Cell>().ElementAt(2).
                                            CellValue.Text;
                        form.SponsorProtocolNumber = row.Elements<Cell>().ElementAt(3).
                                            CellValue.Text;
                        form.Address = row.Elements<Cell>().ElementAt(4).
                                            CellValue.Text;
                        form.Country = row.Elements<Cell>().ElementAt(5).
                                            CellValue.Text;

                        Investigators.Add(Investigator);
                        form.InvestigatorDetails = Investigators;
                        if (Investigator.Role == "PI")
                            forms.Add(form);
                    }
                    else
                        ReadDataFromEachRow = false;

                    RowIndex += 1;
                }
            }
            return forms;
        }

        public void ReadData()
        {
            

        }
    }
}
