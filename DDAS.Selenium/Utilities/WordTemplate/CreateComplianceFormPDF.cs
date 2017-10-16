using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using DDAS.Models.Interfaces;

namespace Utilities.WordTemplate
{
    public class CreateComplianceFormPDF : IWriter
    {
        //private FileStream _stream;
        private Document _document;
        private PdfWriter _writer;
        private PdfPTable _table;
        private MemoryStream _memoryStream;

        public MemoryStream CreateComplianceForm()
        {
            Document document = new Document(PageSize.LETTER, 10, 10, 42, 35);

            string TempFilePath = 
                @"c:\Development\p926-ddas\DDAS.API\DataFiles\pdf_trial.pdf";

            MemoryStream stream = new MemoryStream();
            //new FileStream(TempFilePath, FileMode.CreateNew)
            using (PdfWriter writer = PdfWriter.GetInstance(document, 
                stream))
            {
                document.Open();
                writer.CloseStream = false;

                var paragraph = 
                    ParagraphCenterAlign("INVESTIGATOR COMPLIANCE SEARCH FORM");

                document.Add(paragraph);

                //AddHeaderDetails(document);

                //Phrase pharse = new Phrase("This is my second line using Pharse.");
                //Chunk chunk = new Chunk(" This is my third line using Chunk.");

                paragraph = ParagraphCenterAlign("Investigators:");
                document.Add(paragraph);

                AddInvestigatorDetails(document);

                AddSitesTableHeaders(document);

                AddSearchedBy(document, "Pradeep", "10 Jan 2017");

                document.Close();
                writer.Close();
            }
            using (FileStream fileStream = 
                new FileStream(TempFilePath, FileMode.CreateNew))
            {
                stream.WriteTo(fileStream);
            }
            return stream;
        }

        public Paragraph ParagraphCenterAlign(string ParagraphText)
        {
            Paragraph paragraph = new Paragraph(ParagraphText);
            paragraph.Alignment = 1;
            return paragraph;
        }

        public PdfPCell PDFCellWithCenterAlign(string Text)
        {
            var PDFCell = new PdfPCell(new Phrase(Text));
            PDFCell.HorizontalAlignment = Element.ALIGN_CENTER;
            PDFCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            return PDFCell;
        }

        public void AddHeaderDetails()
        {
            PdfPTable table = new PdfPTable(4);// 2 columns
            table.AddCell("ICON Project Number:");
            table.AddCell("123");
            table.AddCell("Sponsor Protocol No.:");
            table.AddCell("123");

            table.AddCell("Institution Name:");
            table.AddCell("123");
            table.AddCell("Address:");
            table.AddCell("123");

            _document.Add(table);
        }

        public void AddInvestigatorDetails(Document document)
        {
            var table = new PdfPTable(4);
            table.AddCell(PDFCellWithCenterAlign("INVESTIGATOR NAME"));
            table.AddCell(PDFCellWithCenterAlign("QUALIFICATION"));
            table.AddCell(PDFCellWithCenterAlign("MEDICAL LICENSE NUMBER"));
            table.AddCell(PDFCellWithCenterAlign("ROLE"));

            document.Add(table);
        }

        public void AddSitesTableHeaders(Document document)
        {
            var table = new PdfPTable(5);
            table.AddCell(PDFCellWithCenterAlign("SOURCE #"));
            table.AddCell(PDFCellWithCenterAlign("SOURCE NAME"));
            table.AddCell(PDFCellWithCenterAlign("SOURCE DATE"));
            table.AddCell(PDFCellWithCenterAlign("WEBLINK"));
            table.AddCell(PDFCellWithCenterAlign("ISSUES IDENTIFIED"));

            document.Add(table);
        }

        public void AddFindingsTableHeaders(Document document)
        {
            var table = new PdfPTable(4);
            table.AddCell(PDFCellWithCenterAlign("SOURCE #"));
            table.AddCell(PDFCellWithCenterAlign("INVESTIGATOR NAME"));
            table.AddCell(PDFCellWithCenterAlign("DATE OF INSPECTION/ACTION"));
            table.AddCell(PDFCellWithCenterAlign("DESCRIPTION OF FINDINGS"));

            document.Add(table);
        }

        public void AddSearchedBy(Document document, string SearchedBy, string Date)
        {
            var table = new PdfPTable(2);
            table.AddCell(PDFCellWithCenterAlign("Printed Name: " + SearchedBy));
            table.AddCell(PDFCellWithCenterAlign("Signature:"));

            var cell = PDFCellWithCenterAlign("Date: ");
            cell.Rowspan = 2;
            table.AddCell(cell);
        }

        #region IWriter Implementation

        public void Initialize(string TemplateFolder, string ComplianceFormFolder)
        {
            _document = new Document(PageSize.LETTER, 10, 10, 42, 35);

            //if (File.Exists(ComplianceFormFolder))
            //    File.Delete(ComplianceFormFolder);

            //_stream = new FileStream(ComplianceFormFolder, FileMode.CreateNew);

            _memoryStream = new MemoryStream();

            _writer = PdfWriter.GetInstance(_document,
                _memoryStream); //_stream

            _document.Open();
            _writer.CloseStream = false;
        }

        public void WriteParagraph(string Text)
        {
            _document.Add(new Chunk("\n"));
            _document.Add(
                ParagraphCenterAlign(
                    Text));
            //_document.Add(new Chunk("\n"));
        }

        public void AddFormHeaders(string ProjectNumber, 
            string SponsorProtocolNumber, string InstituteName, string Address)
        {
            _table = new PdfPTable(4);

            _table.AddCell(PDFCellWithCenterAlign("ICON Project Number:"));
            _table.AddCell(PDFCellWithCenterAlign(ProjectNumber));
            _table.AddCell(PDFCellWithCenterAlign("Sponsor Protocol No.:"));
            _table.AddCell(PDFCellWithCenterAlign(SponsorProtocolNumber));

            _table.AddCell(PDFCellWithCenterAlign("Institution Name:"));
            _table.AddCell(PDFCellWithCenterAlign(InstituteName));
            _table.AddCell(PDFCellWithCenterAlign("Address:"));
            _table.AddCell(PDFCellWithCenterAlign(Address));

            _document.Add(_table);
            //_document.Add(new Chunk("\n"));
        }

        public void AddTableHeaders(string[] Headers, int Columns, int TableIndex)
        {
            _table = new PdfPTable(Columns);
            
            for (int Index = 0; Index < Columns; Index++)
            {
                _table.AddCell(PDFCellWithCenterAlign(Headers[Index]));
            }
        }

        public void FillUpTable(string[] CellValues)
        {
            foreach(string Value in CellValues)
            {
                _table.AddCell(PDFCellWithCenterAlign(Value));
            }
        }

        public void AddSearchedBy(string SearchedBy, string Date)
        {
            _document.Add(new Chunk("\n"));

            _table = new PdfPTable(2);
            _table.AddCell(PDFCellWithCenterAlign("Printed Name: " + SearchedBy));

            var cell = new PdfPCell(new Phrase("Signature:"));
            cell.Rowspan = 2;
            _table.AddCell(cell);

            _table.AddCell(PDFCellWithCenterAlign("Date: " + Date));
        }

        public void SaveChanges()
        {
            _document.Add(_table);
        }

        public void AddFooterPart(string FooterText)
        {

        }

        public void AttachFile(string FilePath, string ComlianceFormDocPath)
        {

        }

        public void CloseDocument()
        {
            _document.Close();
            //_writer.Close();
        }

        public MemoryStream ReturnStream()
        {
            return _memoryStream;
        }

        #endregion
    }
}
