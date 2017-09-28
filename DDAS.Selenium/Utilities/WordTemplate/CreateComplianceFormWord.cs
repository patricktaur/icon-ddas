//using DocumentFormat.OpenXml.Drawing;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Interfaces;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using OpenXmlEmbedObjectNew;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace Utilities.WordTemplate
{
    public class CreateComplianceFormWord : IWriter
    {
        //private FileStream _stream;
        private WordprocessingDocument _document;
        private Table _table;
        private TableRow _row;
        private MemoryStream _memoryStream;
        
        #region Old working code
        public MemoryStream CreateComplianceForm(ComplianceForm form, string TemplateFolder, string fileName = "")
        {
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

                    UpdateTable(HeaderTable, 0, 3, form.SponsorProtocolNumber);

                    UpdateTable(HeaderTable, 1, 1, form.Institute);

                    UpdateTable(HeaderTable, 1, 3, form.Address);

                    var SitesTable = body.Descendants<Table>().ElementAt(2);

                    var AdditionalSitesTable = body.Descendants<Table>().ElementAt(3);

                    var FindingsTable = body.Descendants<Table>().ElementAt(4);

                    string [] SIs = new string[form.InvestigatorDetails.Count];

                    int RowNumber = 1;

                    foreach(SiteSource siteSource in form.SiteSources)
                    {
                        if (siteSource.SiteSourceUpdatedOn == null)
                            siteSource.SiteSourceUpdatedOn = DateTime.Now; //Refactor

                        string SiteSourceUpdatedOn = 
                            siteSource.SiteSourceUpdatedOn.Value.ToString("dd MMM yyyy");

                        if (RowNumber > 12 && siteSource.IssuesIdentified)
                        {
                            AddSites(
                                AdditionalSitesTable,
                                RowNumber.ToString(),
                                siteSource.SiteName,
                                SiteSourceUpdatedOn,
                                siteSource.SiteUrl, "Yes");
                        }
                        else if (RowNumber > 12)
                        {
                            AddSites(
                                AdditionalSitesTable,
                                RowNumber.ToString(),
                                siteSource.SiteName,
                                SiteSourceUpdatedOn,
                                siteSource.SiteUrl, "No");
                        }
                        else if (siteSource.IssuesIdentified)
                        {
                            AddSites(
                                SitesTable,
                                siteSource.Id.ToString(),
                                siteSource.SiteName,
                                SiteSourceUpdatedOn,
                                siteSource.SiteUrl, "Yes");
                        }
                        else
                            AddSites(
                                SitesTable,
                                siteSource.Id.ToString(),
                                siteSource.SiteName,
                                SiteSourceUpdatedOn,
                                siteSource.SiteUrl, "No");

                        RowNumber += 1;
                    }

                    var InvestigatorsTable = body.Descendants<Table>().ElementAt(1);

                    foreach (InvestigatorSearched Investigator in form.InvestigatorDetails)
                    {
                        int RowIndex = 1;

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

                            //if (Findings == null)
                            //    continue;

                            foreach(Finding Finding in Findings)
                            {
                                var SiteSource = form.SiteSources.Find(x =>
                                    x.SiteEnum == Site.siteEnum);

                                if (//Finding.Observation != null &&
                                    Finding.Selected && 
                                    Investigator.Id == Finding.InvestigatorSearchedId)
                                {
                                    //Add Observation
                                    AddFindings(
                                        FindingsTable, 
                                        RowIndex.ToString(),
                                        Finding.InvestigatorName,
                                        DateTime.Now, //Refactor - clarification required
                                        Finding.Observation);
                                }
                            }
                            RowIndex += 1;
                        }
                    }

                    var SearchedByTable = body.Descendants<Table>().ElementAt(5);
                    AddSearchedByDetails(SearchedByTable, form.AssignedTo, 0, 0);
                    AddSearchedByDetails(SearchedByTable, DateTime.Now.ToString("dd MMM yyyy"),
                        1, 0);
                }
                
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

        //Pradeep 12Dec2016 - Clean up required

        public void AddSearchedByDetails(Table SearchedByTable, 
            string Value, int RowIndex, int CellIndex)
        {
            TableRow Row = SearchedByTable.Elements<TableRow>().ElementAt(RowIndex);
            TableCell Cell = Row.Elements<TableCell>().ElementAt(CellIndex);
            Paragraph paragraph = Cell.Elements<Paragraph>().First();
            Run run = paragraph.Elements<Run>().First();
            Text text = run.Elements<Text>().First();
            text.Text += " " +Value;
        }

        public TableCell CellWithVerticalAlign()
        {
            var tableCell = new TableCell();
            var SitesTableProperties = new TableCellProperties();
            var VerticalAlignProperty = new TableCellVerticalAlignment() {
                Val = TableVerticalAlignmentValues.Center };

            SitesTableProperties.Append(VerticalAlignProperty);
            tableCell.Append(SitesTableProperties);

            return tableCell;
        }

        public Paragraph ParagraphWithCenterAlign()
        {
            var paragraph = new Paragraph();
            var paragraphProperties = new ParagraphProperties();
            var justification = new Justification() {
                Val = JustificationValues.Center };

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

        private void AddTableCell(string Text)
        {
            var TableCell = CellWithVerticalAlign();
            var paragraph = ParagraphWithCenterAlign();

            if(Text != null && Text.Length > 2 && Text.Contains("\n"))
            {
                var Values = 
                    Text.Split('\n');

                var run = new Run();
                for(int Index = 0; Index < Values.Count(); Index++)
                {
                    if(Index > 0)
                        run.Append(new Break());

                    run.Append(new Text(Values[Index]));
                }
                paragraph.Append(run);
            }
            else
                paragraph.Append(new Run(new Text(Text)));

            TableCell.Append(paragraph);
            _row.Append(TableCell);
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
        #endregion

        public void Temp(ComplianceForm form, string TemplateFolder, string fileName = "")
        {
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
                    Table table = new Table();
                    AddInvestigatorDetails(table, "Pradeep", "MD", "#221145", "Sub");
                    body.Append(table);
                }
            }
        }
        
        #region IWriter Implementation

        public void Initialize(string TemplateFolder, string ComplianceFormFolder)
        {   
            byte[] ByteArray = File.ReadAllBytes(
                TemplateFolder + "ComplianceFormTemplate.docx");

            //required to create file on server
            //_stream = new FileStream(ComplianceFormFolder, FileMode.CreateNew);
            //_stream.Write(ByteArray, 0, ByteArray.Length);
            //_document = WordprocessingDocument.Open(_stream, true);

            //creates file in memorystream
            _memoryStream = new MemoryStream();
            _memoryStream.Write(ByteArray, 0, ByteArray.Length);

            _document = WordprocessingDocument.Open(_memoryStream, true);

            //var FilePath = @"c:\Development\test.pdf";
            //var document = new OpenXmlHelper(_document, _document.MainDocumentPart);
            //document.AddObject(FilePath, Path.GetFileName(FilePath));
        }

        public void AddFormHeaders(string ProjectNumber,
            string SponsorProtocolNumber, string InstituteName, string Address)
        {

            var body = _document.MainDocumentPart.Document.Body;

            var HeaderTable = body.Descendants<Table>().ElementAt(0);

            UpdateTable(HeaderTable, 0, 1, ProjectNumber);
            UpdateTable(HeaderTable, 0, 3, SponsorProtocolNumber);
            UpdateTable(HeaderTable, 1, 1, InstituteName);
            UpdateTable(HeaderTable, 1, 3, Address);
        }

        public void AddTableHeaders(string[] Headers, int Columns, int TableIndex)
        {
            var body = _document.MainDocumentPart.Document.Body;
            _table = body.Descendants<Table>().ElementAt(TableIndex);
        }

        public void FillUpTable(string[] CellValues)
        {
            _row = new TableRow();
            foreach (string Value in CellValues)
            {
                //string[] temp = new string[] { };
                //if(Value.Contains("\n"))
                //{
                //    temp = Value.Split('\n');
                //    AddCellValue(temp);
                //}
                //else
                AddTableCell(Value);
            }
            _table.Append(_row);
        }

        private void AddCellValue(string[] Values)
        {
            var TableCell = CellWithVerticalAlign();
            var paragraph = ParagraphWithCenterAlign();

            paragraph.Append(new Run(new Text(Values.ToString())
            { Space = SpaceProcessingModeValues.Preserve }));

            TableCell.Append(paragraph);
            _row.Append(TableCell);
        }

        public void WriteParagraph(string Text)
        {
            
        }

        public void AddSearchedBy(string SearchedBy, string Date)
        {            
            var body = _document.MainDocumentPart.Document.Body;
            var SearchedByTable = body.Descendants<Table>().ElementAt(5);
            AddSearchedByDetails(SearchedByTable, SearchedBy, 0, 0);
            AddSearchedByDetails(SearchedByTable, Date,
                1, 0);
        }

        public void SaveChanges()
        {
            
        }

        public void AddFooterPart(string FooterText)
        {
            var MainDocPart = _document.MainDocumentPart;
            var parts = MainDocPart.FooterParts;

            foreach(FooterPart part in parts)
            {
                var myFooter = part.Footer;
                foreach( Paragraph para in myFooter.Descendants<Paragraph>())
                {
                    foreach (Run run in para.Descendants<Run>())
                    {
                        foreach (Text text in run.Descendants<Text>())
                        {
                            text.Text += " " + FooterText;
                            break;
                        }
                        break;
                    }
                    break;
                }
            }
        }

        public void AttachFile(string EmbeddingFilePath, string ComplianceFormDocPath)
        {
            //var start = new Start();
            //start.EmbedObjectIntoDocument(EmbeddingFilePath, ComplianceFormDocPath);
            //start.EmbedObjectIntoDocument("C:\\Development\\test.pdf", 
            //    "C:\\Development\\p926-ddas\\DDAS.API\\App_Data\\Templates\\ComplianceFormTemplate.docx");

            var ProcessInfo = new ProcessStartInfo();
            ProcessInfo.FileName = @"C:\Development\p926-ddas\ddas.selenium\webscraping\bin\Debug\webscraping.exe";
            //ProcessInfo.FileName = @"C:\Development\OpenXmlDocumentGenerator-master\OpenXmlCidGenerator\bin\Debug\OpenXmlCidGenerator.exe";
            ////ProcessInfo.Arguments = "\"" + EmbeddingFilePath + "  \"" + ComplianceFormDocPath + "\"";
            ProcessInfo.Arguments = @"C:\Development\test.pdf" + " " +
                @"C:\Development\p926-ddas\DDAS.API\App_Data\Templates\ComplianceFormTemplate.docx";
            //@"C:\Development\test.pdf ""C:\Development\p926-ddas\DDAS.API\App_Data\Templates\ComplianceFormTemplate.docx""";
            ProcessInfo.CreateNoWindow = false;
            ProcessInfo.RedirectStandardOutput = true;
            ProcessInfo.RedirectStandardError = true;
            ProcessInfo.UseShellExecute = false;

            var MyProcess = new Process();
            MyProcess.StartInfo = ProcessInfo;

            MyProcess.Start();
            string output = MyProcess.StandardOutput.ReadToEnd();
            string Error = MyProcess.StandardError.ReadToEnd();
            MyProcess.WaitForExit();
            MyProcess.Close();

            //ThreadStart ths = new ThreadStart(() =>
            //Start.EmbedObjectIntoDocument(EmbeddingFilePath, ComplianceFormDocPath));

            //Thread th = new Thread(ths);
            //th.SetApartmentState(ApartmentState.STA);
            //th.Join();

            //var document = new OpenXmlHelper(_document, _document.MainDocumentPart);
            //document.AddObject(EmbeddingFilePath, Path.GetFileName(EmbeddingFilePath));
            //document.Close();
        }

        public void CloseDocument()
        {
            _document.Close();
        }

        public MemoryStream ReturnStream()
        {
            return _memoryStream;
        }
        #endregion
    }
}
