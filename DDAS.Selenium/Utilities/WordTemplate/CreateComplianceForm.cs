﻿//using DocumentFormat.OpenXml.Drawing;
using DDAS.Models.Entities.Domain;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.IO;
using System.Linq;

namespace Utilities.WordTemplate
{
    public class CreateComplianceForm
    {
        public MemoryStream ReplaceTextFromWord(ComplianceForm form, string TemplateFolder, string fileName = "")
        {
            //string FilePath = System.Configuration.ConfigurationManager.AppSettings["TemplatesFolder"];

            //string FilePath = @"C:\Development\p926-ddas\DDAS.API\App_Data\Data\Templates\ComplianceFormTemplate.docx";
            string TemplateFile = TemplateFolder + @"\ComplianceFormTemplate.docx";

            byte[] byteArray = File.ReadAllBytes(
                TemplateFile);

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

                    UpdateTable(HeaderTable, 0, 3, form.Address);

                    UpdateTable(HeaderTable, 1, 1, form.SponsorProtocolNumber);

                    UpdateTable(HeaderTable, 2, 3, form.Country);

                    UpdateTable(HeaderTable, 0, 3, form.Address);

                    UpdateTable(HeaderTable, 2, 1, form.Institute);

                    //var PI = form.InvestigatorDetails.FirstOrDefault().Name;

                    //UpdateTable(HeaderTable, 1, 1, PI);

                    //UpdateTable(HeaderTable, 1, 3, form.Address);

                    var SitesTable = body.Descendants<Table>().ElementAt(2);

                    var FindingsTable = body.Descendants<Table>().ElementAt(3);

                    string [] SIs = new string[form.InvestigatorDetails.Count];

                    foreach(SiteSource siteSource in form.SiteSources)
                    {
                        if(siteSource.IssuesIdentified)
                        AddSites(
                            SitesTable,
                            siteSource.Id.ToString(), 
                            siteSource.SiteName,
                            siteSource.SiteSourceUpdatedOn.ToShortDateString(),
                            siteSource.SiteUrl, "Yes");
                        else
                            AddSites(
                                SitesTable,
                                siteSource.Id.ToString(),
                                siteSource.SiteName,
                                siteSource.SiteSourceUpdatedOn.ToShortDateString(),
                                siteSource.SiteUrl, "No");
                    }

                    //int ArrayIndex = 0;

                    var InvestigatorsTable = body.Descendants<Table>().ElementAt(1);

                    foreach (InvestigatorSearched Investigator in form.InvestigatorDetails)
                    {
                        int RowIndex = 1;

                        //SIs[ArrayIndex] = Investigator.Name;
                        AddInvestigatorDetails(
                            InvestigatorsTable, Investigator.Name,
                            Investigator.Qualification, 
                            Investigator.MedicalLiceseNumber, Investigator.Role);

                        foreach (SiteSearchStatus Site in Investigator.SitesSearched)
                        {
                            var ListOfFindings = form.Findings;

                            var Findings = ListOfFindings.Where(
                                x => x.SiteEnum == Site.siteEnum).
                                ToList();

                            if (Findings == null)
                                continue;

                            foreach(Finding Finding in Findings)
                            {
                                var SiteSource = form.SiteSources.Find(x =>
                                    x.SiteEnum == Site.siteEnum);

                                if (Finding.Observation != null &&
                                    Finding.Selected && 
                                    Investigator.Id == Finding.InvestigatorSearchedId)
                                {
                                    //Add Observation
                                    AddFindings(
                                        FindingsTable, 
                                        RowIndex.ToString(),
                                        Finding.InvestigatorName,
                                        DateTime.Now,
                                        Finding.Observation);
                                }
                            }

                            RowIndex += 1;
                        }
                        //ArrayIndex += 1;
                    }
                    //AddSubInvestigators(HeaderTable, 2, 1, SIs);
                }

                //var FileName = form.InvestigatorDetails.FirstOrDefault().Name + ".docx";

                //FileName += "_" + DateTime.Now.ToShortDateString() + ".docx";
               
                //Patrick 06Dec2016
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                if ( fileName != null)
                {
                    using (FileStream fileStream = new FileStream(fileName,
                    FileMode.CreateNew))
                    {
                        stream.WriteTo(fileStream);
                    }
                }
                return stream;
            }
        }

        public TableCell CellWithVerticalAlign()
        {
            var tableCell = new TableCell();
            var SitesTableProperties = new TableCellProperties();
            var VerticalAlignProperty = new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center };

            SitesTableProperties.Append(VerticalAlignProperty);
            tableCell.Append(SitesTableProperties);

            return tableCell;
        }

        public Paragraph ParagraphWithCenterAlign()
        {
            var paragraph = new Paragraph();
            var paragraphProperties = new ParagraphProperties();
            var justification = new Justification() { Val = JustificationValues.Center };

            paragraphProperties.Append(justification);
            paragraph.Append(paragraphProperties);
            return paragraph;
        }

        public void AddInvestigatorDetails(Table HeaderTable, string InvestigatorName, 
            string Qualification, string MLNumber, string Role)
        {
            var tr = new TableRow();

            var InvestigatorNameCell = CellWithVerticalAlign();
            var paragraph = ParagraphWithCenterAlign();

            paragraph.Append(
                new Run(
                    new Text(
                        InvestigatorName)));

            InvestigatorNameCell.Append(paragraph);

            var QualificationCell = CellWithVerticalAlign();
            var QualificationParagraph = ParagraphWithCenterAlign();

            QualificationParagraph.Append(
                new Run(
                    new Text(
                        Qualification)));

            QualificationCell.Append(QualificationParagraph);

            var MLNumberCell = CellWithVerticalAlign();
            var MLNumberParagraph = ParagraphWithCenterAlign();

            MLNumberParagraph.Append(
                new Run(
                    new Text(
                        MLNumber)));

            MLNumberCell.Append(MLNumberParagraph);

            var RoleCell = CellWithVerticalAlign();
            var RoleParagraph = ParagraphWithCenterAlign();

            RoleParagraph.Append(
                new Run(
                    new Text(
                        Role)));

            RoleCell.Append(RoleParagraph);

            tr.Append(InvestigatorNameCell, QualificationCell, MLNumberCell, RoleCell);

            HeaderTable.Append(tr);
        }

        public void AddSites(Table SitesTable, string SourceNumber, string SourceName, 
            string SourceDate, string WebLink, string IssueIdentified)
        {
            var tr = new TableRow();

            var SourceNumberCell = CellWithVerticalAlign(); //new TableCell();

            var paragraph = ParagraphWithCenterAlign();

            paragraph.Append(new Run(new Text(SourceNumber)));

            SourceNumberCell.Append(paragraph);

            var SourceNameCell = CellWithVerticalAlign(); //new TableCell();

            var SourceNameParagraph = ParagraphWithCenterAlign();

            SourceNameParagraph.Append(new Run(new Text(SourceName)));
            SourceNameCell.Append(SourceNameParagraph);

            var SourceDateCell = CellWithVerticalAlign(); //new TableCell();

            var SourceDateParagraph = ParagraphWithCenterAlign();

            SourceDateParagraph.Append(new Run(new Text(SourceDate)));
            SourceDateCell.Append(SourceDateParagraph);

            var WebLinkCell = CellWithVerticalAlign(); //new TableCell();

            var WebLinkParagraph = ParagraphWithCenterAlign();

            WebLinkParagraph.Append(new Run(new Text(WebLink)));
            WebLinkCell.Append(WebLinkParagraph);

            var IssueIdentifiedCell = CellWithVerticalAlign(); //new TableCell();

            var IssueIdentifiedParagraph = ParagraphWithCenterAlign();

            IssueIdentifiedParagraph.Append(new Run(new Text(IssueIdentified)));
            IssueIdentifiedCell.Append(IssueIdentifiedParagraph);

            tr.Append(SourceNumberCell, SourceNameCell, SourceDateCell, WebLinkCell,
                IssueIdentifiedCell);

            SitesTable.Append(tr);
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

        public void AddFindings(Table table, string SourceNumber, 
            string InvestigatorName, DateTime DateOfInspection, 
            string DescriptionOfFindings)
        {
            var tr = new TableRow();

            var SourceNumberCell = CellWithVerticalAlign(); //new TableCell();
            var paragraph = ParagraphWithCenterAlign();

            paragraph.Append(
                new Run(
                    new Text(
                        SourceNumber)));

            SourceNumberCell.Append(paragraph);

            var InvestigatorNameCell = CellWithVerticalAlign();
            var InvestigatorNameParagraph = ParagraphWithCenterAlign();

            InvestigatorNameParagraph.Append(
                new Run(
                    new Text(
                        InvestigatorName)));

            InvestigatorNameCell.Append(InvestigatorNameParagraph);


            var DateOfInspectionCell = CellWithVerticalAlign(); //new TableCell();
            var DateOfInspectionParagraph = ParagraphWithCenterAlign();

            DateOfInspectionParagraph.Append(
                new Run(
                    new Text(
                        DateOfInspection.ToShortDateString())));

            DateOfInspectionCell.Append(DateOfInspectionParagraph);

            var FindingsCell = CellWithVerticalAlign(); //new TableCell();
            var FindingsParagraph = ParagraphWithCenterAlign();

            FindingsParagraph.Append(
                new Run(
                    new Text(
                    DescriptionOfFindings)));

            FindingsCell.Append(FindingsParagraph);

            tr.Append(SourceNumberCell, InvestigatorNameCell, DateOfInspectionCell, FindingsCell);
            table.Append(tr);
        }

        public void AddSubInvestigators(Table table, int RowIndex, int CellIndex, string[] SIs)
        {
            TableRow Row = table.Elements<TableRow>().ElementAt(RowIndex);
            TableCell Cell = Row.Elements<TableCell>().ElementAt(CellIndex);
            Paragraph paragraph = Cell.Elements<Paragraph>().First();
            Run run = paragraph.Elements<Run>().First();
            Text text = run.Elements<Text>().First();
            text.Text = null;
            foreach(string SI in SIs)
            {
                run.AppendChild(new Text(SI));
                run.AppendChild(new Break());
                run.AppendChild(new Break());
            }
        }

        public void UpdateTable(Table table, int RowIndex, int CellIndex, 
            string Value)
        {
            TableRow Row = table.Elements<TableRow>().ElementAt(RowIndex);
            TableCell Cell = Row.Elements<TableCell>().ElementAt(CellIndex);
            Paragraph paragraph = Cell.Elements<Paragraph>().First();
            Run run = paragraph.Elements<Run>().First();
            Text text = run.Elements<Text>().First();
            text.Text = null;
            text.Text = Value;
        }

        public void ChangeCheckBoxStatus(Table table, int RowIndex, int CellIndex,
            bool CheckOrUnCheck)
        {
            TableRow Row = table.Elements<TableRow>().ElementAt(RowIndex);
            TableCell Cell = Row.Elements<TableCell>().ElementAt(CellIndex);
            Paragraph paragraph = Cell.Elements<Paragraph>().First();
            //Run run = paragraph.Elements<Run>().First();
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

