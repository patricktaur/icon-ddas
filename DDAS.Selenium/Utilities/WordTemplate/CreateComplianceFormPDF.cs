using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using DDAS.Models.Interfaces;

namespace Utilities.WordTemplate
{
    public class CreateComplianceFormPDF : IWriter
    {
        private FileStream _stream;
        private Document _document;
        private PdfWriter _writer;
        private PdfPTable _table;

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
            PDFCell.VerticalAlignment = Element.ALIGN_CENTER;
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

        public void Initialize(string FilePath)
        {
            _document = new Document(PageSize.LETTER, 10, 10, 42, 35);

            if (File.Exists(FilePath))
                File.Delete(FilePath);

            _stream = new FileStream(FilePath, FileMode.CreateNew);

            _writer = PdfWriter.GetInstance(_document,
                _stream);

            _document.Open();
            _writer.CloseStream = false;
        }

        public void WriteParagraph(string Text)
        {
            _document.Add(
                ParagraphCenterAlign(
                    Text));
        }

        public void AddFormHeaders(string ProjectNumber, 
            string SponsorProtocolNumber, string InstituteName, string Address)
        {
            PdfPTable table = new PdfPTable(4);

            table.AddCell(PDFCellWithCenterAlign("ICON Project Number:"));
            table.AddCell(PDFCellWithCenterAlign(ProjectNumber));
            table.AddCell(PDFCellWithCenterAlign("Sponsor Protocol No.:"));
            table.AddCell(PDFCellWithCenterAlign(SponsorProtocolNumber));

            table.AddCell(PDFCellWithCenterAlign("Institution Name:"));
            table.AddCell(PDFCellWithCenterAlign(InstituteName));
            table.AddCell(PDFCellWithCenterAlign("Address:"));
            table.AddCell(PDFCellWithCenterAlign(Address));

            _document.Add(table);
        }

        public void AddTableHeaders(string[] Headers, int Columns)
        {
            _table = new PdfPTable(Columns);

            for (int Index = 0; Index < Columns; Index++)
            {
                _table.AddCell(Headers[Index]);
            }
        }

        public void FillUpTable(int RowIndex, int ColumnIndex, int TableIndex, string Text)
        {
            _table.AddCell(PDFCellWithCenterAlign(Text));
        }

        public void AddSearchedBy(string SearchedBy, string Date)
        {

        }
        public void CloseDocument()
        {
            _document.Close();
            _writer.Close();
        }
    }
}
