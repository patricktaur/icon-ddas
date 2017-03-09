using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Interfaces
{
    public interface IWriter
    {
        void Initialize(string TemplateFolder, string ComplianceFormFolder);

        void WriteParagraph(string Text);

        void AddFormHeaders(string ProjectNumber, string SponsorProtocolNumber,
            string InstituteName, string Address);

        void AddTableHeaders(string[] Headers, int Columns, int TableIndex);

        void FillUpTable(string[] CellValues);

        void AddSearchedBy(string SearchedBy, string Date);

        void SaveChanges();

        void AddFooterPart(string FooterText);

        void CloseDocument();
    }
}
