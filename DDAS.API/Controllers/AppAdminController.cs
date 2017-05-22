using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using DDAS.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace DDAS.API.Controllers
{
    [Authorize(Roles = "app-admin")] 
    [RoutePrefix("api/AppAdmin")]
    public class AppAdminController : ApiController
    {
        private IAppAdminService _AppAdminService;
        private string _ErrorScreenCaptureFolder;
        private string _UploadsFolder;
        private string _RootPath;
        private string _OutputFilePath;

        public AppAdminController(IAppAdminService AppAdmin)
        {
            _AppAdminService = AppAdmin;

            _RootPath = HttpRuntime.AppDomainAppPath;

            _ErrorScreenCaptureFolder = _RootPath +
                System.Configuration.ConfigurationManager.AppSettings["ErrorScreenCaptureFolder"];
            
            _UploadsFolder = _RootPath +
                System.Configuration.ConfigurationManager.AppSettings["UploadsFolder"];

            _OutputFilePath = _RootPath +
                System.Configuration.ConfigurationManager.AppSettings["OutputFileFolder"];
        }

        #region Get/Delete/Download ErrorImages
        [Route("GetErrorImages")]
        [HttpGet]
        public List<ErrorScreenCapture> GetAllErrorImages()
        {
  
            var ListOfErrorImages = new List<ErrorScreenCapture>();

            DirectoryInfo info = new DirectoryInfo(_ErrorScreenCaptureFolder);
            FileInfo[] files = info.GetFiles().OrderByDescending(o => o.CreationTime).ToArray();

            foreach (FileInfo File in files)
            {
                var ErrorImage = new ErrorScreenCapture();

                ErrorImage.FileName = File.Name;
                ErrorImage.FileSize = File.Length / 1024;
                ErrorImage.Created = File.CreationTime;
                ListOfErrorImages.Add(ErrorImage);

            }
            return ListOfErrorImages;
        }
        
        [Route("DeleteAllErrorImages")]
        [HttpGet]
        public IHttpActionResult DeleteAllErrorImages()
        {
            var ErrorImages = Directory.GetFiles(_ErrorScreenCaptureFolder);

            foreach (string file in ErrorImages)
            {
                File.Delete(file);
            }
            return Ok(true);
        }

        [Route("DeleteErrorImage")]
        [HttpGet]
        public IHttpActionResult DeleteErrorImage(string FileName)
        {
            if(File.Exists(_ErrorScreenCaptureFolder + FileName))
                File.Delete(_ErrorScreenCaptureFolder + FileName);
            return Ok(true);
        }

        [Route("DownloadErrorImage")]
        [HttpGet]
        public IHttpActionResult DownloadErrorImage()
        {
            string FilePath = _ErrorScreenCaptureFolder;
            string path = FilePath.Replace(_RootPath, "");
            return Ok(path);
        }

        #endregion

        #region DataExtractionHistory
       

        [Route("GetDataExtractionPerSite")]
        [HttpGet]
        public IHttpActionResult GetDataExtractionPerSite(SiteEnum Enum)
        {
            var ExtractionDataPerSite = _AppAdminService.GetDataExtractionPerSite(Enum);
            return Ok(ExtractionDataPerSite);
        }

        [Route("DeleteExtractionData")]
        [HttpGet]
        public IHttpActionResult DeleteExtractionEntryAAA(string RecId, SiteEnum Enum)
        {
            var Id = Guid.Parse(RecId);
            _AppAdminService.DeleteExtractionEntry(Enum, Id);
            return Ok(true);
        }

        #endregion

        #region get/delete Uploaded Files
        
        [Route("GetUploadsFolderPath")]
        [HttpGet]
        public IHttpActionResult GetUploadsFolderPath()
        {
            string FilePath = _UploadsFolder;
            string path = FilePath.Replace(_RootPath, "");
            return Ok(path);
        }

        [Route("GetUploadedFiles")]
        [HttpGet]
        public IHttpActionResult GetUploadedFiles()
        {
            var UploadedFiles = _AppAdminService.GetUploadedFiles();
            return Ok(UploadedFiles);
        }

        [Route("DeleteUploadedFile")]
        [HttpGet]
        public IHttpActionResult DeleteUploadedFile(string GeneratedFileName)
        {
            return Ok(_AppAdminService.DeleteUploadedFile(GeneratedFileName));
        }

        [Route("DeleteAllUploadedFiles")]
        [HttpGet]
        public IHttpActionResult DeleteAllUploadedFiles()
        {
            return Ok(_AppAdminService.DeleteAllUploadedFiles());
        }

        #endregion

        #region get/delete Output Files
        
        [Route("GetOutputFilePath")]
        [HttpGet]
        public IHttpActionResult GetOutputFilePath()
        {
            string FilePath = _OutputFilePath;
            string path = FilePath.Replace(_RootPath, "");
            return Ok(path);
        }
        
        [Route("GetOutputFiles")]
        [HttpGet]
        public IHttpActionResult GetOutputFiles()
        {
            return Ok();
        }

        [Route("DeleteOutputFile")]
        [HttpGet]
        public IHttpActionResult DeleteOutputFile()
        {
            return Ok();
        }
        #endregion
        #region LiveScanner


        [Route("LaunchLiveScanner")]
        [HttpGet]
        public IHttpActionResult LaunchLiveScanner()
        {
            var result = _AppAdminService.LaunchLiveScanner(_RootPath + "bin");
            return Ok(result);
        }

        [Route("LiveScannerInfo")]
        [HttpGet]
        public IHttpActionResult LiveScannerCount()
        {
            var result = _AppAdminService.getLiveScannerProcessorsInfo();
            return Ok(result);
        }

        [Route("KillLiveScanner")]
        [HttpGet]
        public IHttpActionResult KillLiveScanner()
        {
            var result = _AppAdminService.KillLiveSiteScanner();
            return Ok(result);
        }

        #endregion     
    }
}
