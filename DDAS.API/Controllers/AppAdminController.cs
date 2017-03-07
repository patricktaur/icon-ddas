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
        private IAppAdmin _AppAdmin;
        private string ErrorScreenCaptureFolder;
        private string RootPath;

        public AppAdminController(IAppAdmin AppAdmin)
        {
            _AppAdmin = AppAdmin;

            RootPath = HttpRuntime.AppDomainAppPath;

            ErrorScreenCaptureFolder = RootPath +
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
            return Ok(_AppAdmin.GetDataExtractionHistory());
        }

        [Route("GetDataExtractionPerSite")]
        [HttpGet]
        public IHttpActionResult GetDataExtractionPerSite(SiteEnum Enum)
        {
            var ExtractionDataPerSite = _AppAdmin.GetDataExtractionPerSite(Enum);
            return Ok(ExtractionDataPerSite);
        }

        [Route("DeleteExtractionData")]
        [HttpGet]
        public IHttpActionResult DeleteExtractionEntryAAA(string RecId, SiteEnum Enum)
        {
            var Id = Guid.Parse(RecId);
            _AppAdmin.DeleteExtractionEntry(Enum, Id);
            return Ok(true);
        }

        [Route("DownloadErrorImage")]
        [HttpGet]
        public IHttpActionResult DownloadErrorImage()
        {
            string FilePath = ErrorScreenCaptureFolder;
            string path = FilePath.Replace(RootPath, "");
            return Ok(path);
        }
    }
}
