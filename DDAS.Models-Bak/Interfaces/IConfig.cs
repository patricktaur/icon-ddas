using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Interfaces
{
    public interface IConfig
    {
        string AppDataDownloadsFolder { get; set; }
        string ErrorScreenCaptureFolder { get; set; }
        string DataExtractionLogFile { get; set; }
        string UploadsFolder { get; set; }
        string ComplianceFormFolder { get; set; }
        string ExcelTempateFolder { get; set; }
        string AttachmentsFolder { get; set; }
        string WordTemplateFolder { get; set; }
        string CIILFolder { get; set; }
        string OutputFileFolder { get; set; }
        string ExclusionDatabaseFolder { get; set; }
        string FDAWarningLettersFolder { get; set; }
        string SAMFolder { get; set; }
        string SDNFolder { get; set; }
    }
}
