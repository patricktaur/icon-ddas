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
    //[Authorize]
    [RoutePrefix("api/AppAdmin")]
    public class AppAdminController : ApiController
    {
        private IAppAdminService _AppAdminService;
        private string ErrorScreenCaptureFolder;
        private string _RootPath;


        public AppAdminController(IAppAdminService AppAdmin)
        {
            _AppAdminService = AppAdmin;

            _RootPath = HttpRuntime.AppDomainAppPath;

            ErrorScreenCaptureFolder = _RootPath +
                System.Configuration.ConfigurationManager.AppSettings["ErrorScreenCaptureFolder"];
            //ErrorScreenCaptureFolder = @"DataFiles\ErrorScreenCapture";

        }

        [Route("GetErrorImages")]
        [HttpGet]
        public List<ErrorScreenCapture> GetAllErrorImages()
        {
  
            var ListOfErrorImages = new List<ErrorScreenCapture>();

            DirectoryInfo info = new DirectoryInfo(ErrorScreenCaptureFolder);
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
            var ErrorImages = Directory.GetFiles(ErrorScreenCaptureFolder);

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
            if(File.Exists(ErrorScreenCaptureFolder + FileName))
                File.Delete(ErrorScreenCaptureFolder + FileName);
            return Ok(true);
        }

        [Route("GetDataExtractionHistory")]
        [HttpGet]
        public IHttpActionResult GetDataExtractionHistory()
        {
            return Ok(_AppAdminService.GetDataExtractionHistory());
        }

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

        [Route("DownloadErrorImage")]
        [HttpGet]
        public IHttpActionResult DownloadErrorImage()
        {
            string FilePath = ErrorScreenCaptureFolder;
            string path = FilePath.Replace(_RootPath, "");
            return Ok(path);
        }

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

        #region Add/Delete sites
        
        [Route("AddSite")]
        [HttpPost]
        public IHttpActionResult AddSite(SearchQuerySite Site)
        {
            _AppAdminService.AddSitesInDbCollection(Site);
            return Ok();
        }
        #endregion
    }
}
