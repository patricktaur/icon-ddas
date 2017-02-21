using DDAS.Models.Entities.Domain;
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
        private ISearchService _SearchService;

        private string ErrorScreenCaptureFolder;
        private string RootPath;

        public AppAdminController(ISearchService SearchService)
        {
            _SearchService = SearchService;

            RootPath = HttpRuntime.AppDomainAppPath;

            ErrorScreenCaptureFolder = RootPath +
                System.Configuration.ConfigurationManager.AppSettings["ErrorScreenCaptureFolder"];
        }

        [Route("GetErrorImages")]
        [HttpGet]
        public List<ErrorScreenCapture> GetAllErrorImages()
        {
            var ErrorImages = Directory.GetFiles(ErrorScreenCaptureFolder);

            var ListOfErrorImages = new List<ErrorScreenCapture>();

            foreach(string File in ErrorImages)
            {
                var ErrorImage = new ErrorScreenCapture();

                FileInfo info = new FileInfo(File);
                ErrorImage.FileName = info.Name;
                ErrorImage.FileSize = info.Length / 1024;
                ListOfErrorImages.Add(ErrorImage);
            }
            return ListOfErrorImages;
        }
              //DeleteAllErrorImages
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
    }
}
