using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Interfaces
{
    public interface IWriter
    {
        void Initialize(string TemplateFolder, string ComplianceFormFolder);

        void WriteParagraph(string Text);

        void AddFormHeaders(string ProjectNumber, string ProjectNumber2, 
            string SponsorProtocolNumber, string SponsorProtocolNumber2,
            string InstituteName, string Address);

        void AddTableHeaders(string[] Headers, int Columns, int TableIndex);

        void FillUpTable(string[] CellValues, string CellAlignment);

        void AddSearchedBy(string SearchedBy, string Date);

        void AddFooterPart(string FooterText);

        void AttachFile(string FilePath, string ComplianceFormDocPath);

        void SaveChanges();

        void CloseDocument();

        MemoryStream ReturnStream();
    }
}
