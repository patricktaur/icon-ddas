//using DocumentFormat.OpenXml.Drawing;
using DDAS.Models.Entities.Domain;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Linq;

namespace Utilities.WordTemplate
{
    public class ReplaceTextFromWordTemplate
    {
        public void ReplaceTextFromWord(ComplianceForm form,string Name, string CompanyName)
        {
            using (WordprocessingDocument doc =
                   WordprocessingDocument.Open(
                    @"C:\Development\p926-ddas\DDAS.API\App_Data\SITE LIST REQUEST FORM_Updated.docx", true))
            {
                var body = doc.MainDocumentPart.Document.Body;

                var Table = body.Descendants<Table>().ElementAt(0);
                UpdateTable(Table, 0, 1, "SPR-1234");
                UpdateTable(Table, 1, 1, form.NameToSearch);
                //UpdateTable(Table, 2, 1, "Some Sub Investigator Name");

                Table = body.Descendants<Table>().ElementAt(2);
                
                var Sites = form.SiteDetails.OrderBy(Site => Site.SiteEnum).ToList();

                var FindingsTable = body.Descendants<Table>().ElementAt(3);
                //int ApprovedOrRejectedCounter = 0;
                for (int i = 0; i < 12; i++)
                {
                    var ApprovedOrRejectedRecords = 
                    Sites[i].MatchedRecords.Where(record =>
                    record.Status == "Approve" || record.Status == "Reject").
                    ToList();

                    UpdateTable(Table, i, 2, DateTime.Now.ToShortDateString());
                    if (Sites[i].IssuesFound > 0)
                    {
                        ChangeCheckBoxStatus(Table, i, 4, false);
                        ChangeCheckBoxStatus(Table, i, 5, true);

                        AddFindings(FindingsTable, (i + 1).ToString(),
                            Sites[i].DataExtractedOn,
                            ApprovedOrRejectedRecords[0].Observation);
                        //ApprovedOrRejectedCounter += 1;
                    }
                    else
                    {
                        ChangeCheckBoxStatus(Table, i, 4, true);
                        ChangeCheckBoxStatus(Table, i, 5, false);
                    }
                }
                //AddFindings(FindingsTable, "NA", DateTime.Now,
                //"Person was also searched as goes here");
            }
        }

        public void AddFindings(Table table, string SourceNumber, DateTime DateOfInspection,
            string DescriptionOfFindings)
        {
            var tr = new TableRow();

            var SourceTableCell = new TableCell();
            SourceTableCell.Append(new Paragraph(new Run(new Text(
                SourceNumber))));

            var DateTableCell = new TableCell();
            DateTableCell.Append(new Paragraph(new Run(new Text(
                DateOfInspection.ToShortDateString()))));

            var FindingsTableCell = new TableCell();
            FindingsTableCell.Append(new Paragraph(new Run(new Text(
                DescriptionOfFindings))));

            tr.Append(SourceTableCell, DateTableCell, FindingsTableCell);
            table.Append(tr);
        }

        public void UpdateTable(Table table, int RowIndex, int CellIndex, 
            string replaceWith)
        {
            TableRow Row = table.Elements<TableRow>().ElementAt(RowIndex);
            TableCell Cell = Row.Elements<TableCell>().ElementAt(CellIndex);
            Paragraph paragraph = Cell.Elements<Paragraph>().First();
            //ParagraphProperties pr = paragraph.Elements<ParagraphProperties>().First();
            Run run = paragraph.Elements<Run>().First();
            Text text = run.Elements<Text>().First();
            text.Text = null;
            text.Text = replaceWith;
        }

        public void ChangeCheckBoxStatus(Table table, int RowIndex, int CellIndex,
            bool CheckOrUnCheck)
        {
            TableRow Row = table.Elements<TableRow>().ElementAt(RowIndex);
            TableCell Cell = Row.Elements<TableCell>().ElementAt(CellIndex);
            Paragraph paragraph = Cell.Elements<Paragraph>().First();
            Run run = paragraph.Elements<Run>().First();
            CheckBox chk = paragraph.Descendants<CheckBox>().FirstOrDefault();
            DefaultCheckBoxFormFieldState DefaultStatus =
                chk.GetFirstChild<DefaultCheckBoxFormFieldState>();
            if(DefaultStatus.Val.Value != CheckOrUnCheck)
                DefaultStatus.Val.Value = CheckOrUnCheck;
        }
    }
}
//foreach( var t in Table)
//{
//    foreach(var Row in t.Elements<TableRow>())
//    {
//        foreach(var cell in Row.Elements<TableCell>())
//        {
//            if (cell.InnerText.Contains("$$Company"))
//            {
//                //cell.InnerText = cell.InnerText.Replace("$$Company", "$$Updated");
//                var p = cell.Elements<Paragraph>().First();
//                var run = p.Elements<Run>().First();
//                var text = run.Elements<Text>().First();
//                text.Text = CompanyName;
//            }
//            else if(cell.InnerText.Contains("$$Name"))
//            {
//                var p = cell.Elements<Paragraph>().First();
//                var run = p.Elements<Run>().First();
//                var text = run.Elements<Text>().First();
//                text.Text = Name;
//            }
//        }
//    }
//}

