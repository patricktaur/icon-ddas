using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenXmlEmbedObjectNew
{
    public class Start
    {
        public void EmbedObjectIntoDocument(
            string EmbeddingObjectPath, 
            string WordDocumentPath)
        {
            //var EmbeddingObjectPath = "C:\\Development\\test.pdf";
            //var WordDocumentPath  = "C:\\Development\\p926-ddas\\DDAS.API\\App_Data\\Templates\\ComplianceFormTemplate.docx";

            var Document = WordprocessingDocument.Open(WordDocumentPath, true);

            var openXmlDocument = new OpenXmlHelper(Document, Document.MainDocumentPart);

            openXmlDocument.AddObject(
                EmbeddingObjectPath, Path.GetFileName(EmbeddingObjectPath));

            openXmlDocument.Close();

            return;
        }
    }
}
