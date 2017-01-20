using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Interfaces
{
    public interface IWriter
    {
        void Initialize(string TemplateFolder);
        void WriteParagraph(string Text);

        void AddFormHeaders(string ProjectNumber, string SponsorProtocolNumber,
            string InstituteName, string Address);

        void AddTableHeaders(string[] Headers, int Columns);

        void FillUpTable(int RowIndex, 
            int ColumnIndex, 
            int TableIndex,
            string Text);

        //void AddInvestigatorDetailsTable();
        //void AddSitesTable();
        //void AddAdditionalSitesTable();
        //void AddFindingsTable();
        void AddSearchedBy(string SearchedBy, string Date);
        void CloseDocument();
    }
}
