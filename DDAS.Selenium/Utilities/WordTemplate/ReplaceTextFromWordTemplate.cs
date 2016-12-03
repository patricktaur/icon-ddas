//using DocumentFormat.OpenXml.Drawing;
using DDAS.Models.Entities.Domain;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.IO;
using System.Linq;

namespace Utilities.WordTemplate
{
    public class ReplaceTextFromWordTemplate
    {
        public MemoryStream ReplaceTextFromWord(ComplianceForm form)
        {
            string FilePath = @"C:\Development\p926-ddas\DDAS.API\App_Data\SITE LIST REQUEST FORM_Updated.docx";

            byte[] byteArray = File.ReadAllBytes(
                FilePath);

            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(byteArray, 0, byteArray.Length);

                using (WordprocessingDocument doc =
                   WordprocessingDocument.Open(
                    stream, true))
                {
                    var body = doc.MainDocumentPart.Document.Body;

                    var HeaderTable = body.Descendants<Table>().ElementAt(0);

                    UpdateTable(HeaderTable, 0, 1, form.ProjectNumber);

                    UpdateTable(HeaderTable, 0, 1, form.SponsorProtocolNumber);

                    var PI = form.InvestigatorDetails.FirstOrDefault().Name;

                    UpdateTable(HeaderTable, 1, 1, PI);

                    UpdateTable(HeaderTable, 1, 3, form.Address);

                    var SitesTable = body.Descendants<Table>().ElementAt(2);

                    var FindingsTable = body.Descendants<Table>().ElementAt(3);

                    foreach (InvestigatorSearched Investigator in form.InvestigatorDetails)
                    {
                        int RowIndex = 1;

                        UpdateTable(HeaderTable, 2, 1, Investigator.Name); //Add SI

                        foreach (SiteSearchStatus Site in Investigator.SitesSearched)
                        {
                            var ListOfFindings = form.Findings;

                            var Findings = ListOfFindings.Where(
                                x => x.SiteEnum == Site.siteEnum).
                                FirstOrDefault();

                            if (Findings == null)
                                continue;

                            if (Findings.Status != null &&
                                Findings.Status.ToLower() == "approve")
                            {
                                CheckOrUnCheckIssuesIdentified(SitesTable, RowIndex, true);
                                //Add Observation
                                AddFindings(FindingsTable, RowIndex.ToString(),
                                    DateTime.Now,
                                    Findings.Observation);
                            }
                            else
                                CheckOrUnCheckIssuesIdentified(SitesTable, RowIndex, false);

                            RowIndex += 1;
                        }
                    }
                }

                var FileName = form.InvestigatorDetails.FirstOrDefault().Name + ".docx";

                //FileName += "_" + DateTime.Now.ToShortDateString() + ".docx";

                return stream;
                //using (FileStream fileStream = new FileStream("Test.docx",
                //FileMode.Open))
                //{
                //    stream.WriteTo(fileStream);
                //    return fileStream;
                //}
            }
        }

        public void GenerateComplianceForm()
        {

            byte[] byteArray = File.ReadAllBytes(@"C:\Development\p926-ddas\DDAS.API\App_Data\SITE LIST REQUEST FORM_Updated.docx");

            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(byteArray, 0, byteArray.Length);

                using (WordprocessingDocument doc =
                   WordprocessingDocument.Open(
                    stream, true))
                {

                }

                using (FileStream fileStream = new FileStream("Test2.docx",
                FileMode.CreateNew))
                {
                    stream.WriteTo(fileStream);
                }
            }
            
        }

        public void CheckOrUnCheckIssuesIdentified(Table table, int RowIndex, bool IsIssueIdentified)
        {

            if(IsIssueIdentified)
            {
                ChangeCheckBoxStatus(table, RowIndex, 4, false);
                ChangeCheckBoxStatus(table, RowIndex, 5, true);
            }
            else
            {
                ChangeCheckBoxStatus(table, RowIndex, 4, true);
                ChangeCheckBoxStatus(table, RowIndex, 5, false);
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
            if (DefaultStatus.Val.Value != CheckOrUnCheck)
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

