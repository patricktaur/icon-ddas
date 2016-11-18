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
                   WordprocessingDocument.Open(@"C:\Development\Temp\SITE LIST REQUEST FORM_Updated.docx", true))
            {
                var body = doc.MainDocumentPart.Document.Body;

                var Table = body.Descendants<Table>().ElementAt(0);
                UpdateTable(Table, 0, 1, form.ProjectNumber);
                UpdateTable(Table, 1, 1, form.NameToSearch);
                //UpdateTable(Table, 2, 1, "Some Sub Investigator Name");

                Table = body.Descendants<Table>().ElementAt(2);

                for(int i=0; i<=12; i++)
                {
                    UpdateTable(Table, i, 2, DateTime.Now.ToString());
                }
            }
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
            text.Text = replaceWith;
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

