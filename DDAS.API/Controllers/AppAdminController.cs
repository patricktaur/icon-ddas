﻿using DDAS.API.Helpers;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using DDAS.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Runtime.CompilerServices;
using Microsoft.AspNet.Identity;


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
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private string _logMode;


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
            _logMode = System.Configuration.ConfigurationManager.AppSettings["LogMode"];

        }

        //[Route("GetCBERRecords")]
        //[HttpGet]
        //public IHttpActionResult GetCBERData()
        //{
        //    return Ok(_AppAdminService.GetCBERData());
        //}

        #region Get/Delete/Download ErrorImages
        [Route("GetErrorImages")]
        [HttpGet]
        public List<ErrorScreenCapture> GetAllErrorImages()
        {
            using (new TimeMeasurementBlock(Logger, _logMode, CurrentUser(), GetCallerName()))
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
        }

        [Route("DeleteAllErrorImages")]
        [HttpGet]
        public IHttpActionResult DeleteAllErrorImages()
        {
            using (new TimeMeasurementBlock(Logger, _logMode, CurrentUser(), GetCallerName()))
            {
                var ErrorImages = Directory.GetFiles(_ErrorScreenCaptureFolder);

                foreach (string file in ErrorImages)
                {
                    File.Delete(file);
                }
                return Ok(true);
            }
        }

        [Route("DeleteErrorImage")]
        [HttpGet]
        public IHttpActionResult DeleteErrorImage(string FileName)
        {
            using (new TimeMeasurementBlock(Logger, _logMode, CurrentUser(), GetCallerName()))
            {
                if (File.Exists(_ErrorScreenCaptureFolder + FileName))
                    File.Delete(_ErrorScreenCaptureFolder + FileName);
                return Ok(true);
            }
        }

        [Route("GetErrorScreenCaptureFolderPath")]
        [HttpGet]
        public IHttpActionResult DownloadErrorImage()
        {
            using (new TimeMeasurementBlock(Logger, _logMode, CurrentUser(), GetCallerName()))
            {
                string FilePath = _ErrorScreenCaptureFolder;
                string path = FilePath.Replace(_RootPath, "");
                return Ok(path);
            }
        }

        #endregion

        #region DataExtractionHistory


        [Route("GetDataExtractionPerSite")]
        [HttpGet]
        public IHttpActionResult GetDataExtractionPerSite(SiteEnum Enum)
        {
            using (new TimeMeasurementBlock(Logger, _logMode, CurrentUser(), GetCallerName()))
            {
                var ExtractionDataPerSite = _AppAdminService.GetDataExtractionPerSite(Enum);
                return Ok(ExtractionDataPerSite);
            }
        }

        [Route("DeleteExtractionData")]
        [HttpGet]
        public IHttpActionResult DeleteExtractionEntryAAA(string RecId, SiteEnum Enum)
        {
            using (new TimeMeasurementBlock(Logger, _logMode, CurrentUser(), GetCallerName()))
            {
                var Id = Guid.Parse(RecId);
                _AppAdminService.DeleteExtractionEntry(Enum, Id);
                return Ok(true);
            }
        }

        #endregion

        #region get/delete Uploaded Files

        [Route("GetUploadsFolderPath")]
        [HttpGet]
        public IHttpActionResult GetUploadsFolderPath()
        {
            using (new TimeMeasurementBlock(Logger, _logMode, CurrentUser(), GetCallerName()))
            {
                string FilePath = _UploadsFolder;
                string path = FilePath.Replace(_RootPath, "");
                return Ok(path);
            }
        }

        [Route("GetUploadedFiles")]
        [HttpGet]
        public IHttpActionResult GetUploadedFiles()
        {
            using (new TimeMeasurementBlock(Logger, _logMode, CurrentUser(), GetCallerName()))
            {
                var UploadedFiles = _AppAdminService.GetUploadedFiles();
                return Ok(UploadedFiles);
            }
        }

        [Route("GetUploadedFile")]
        [HttpGet]
        public HttpResponseMessage GetUploadedFile(string GeneratedFileName)
        {
            using (new TimeMeasurementBlock(Logger, _logMode, CurrentUser(), GetCallerName()))
            {
                HttpResponseMessage Response = null;

                if (!File.Exists(_UploadsFolder + GeneratedFileName))
                    Response = Request.CreateResponse(HttpStatusCode.Gone);
                else
                {
                    Response = Request.CreateResponse(HttpStatusCode.OK);

                    var UserAgent = Request.Headers.UserAgent.ToString();
                    var Browser = IdentifyBrowser.GetBrowserType(UserAgent);

                    byte[] ByteArray =
                        File.ReadAllBytes(_UploadsFolder + GeneratedFileName);

                    Response.Content = new ByteArrayContent(ByteArray);

                    Response.Content.Headers.ContentDisposition =
                        new ContentDispositionHeaderValue("attachment");

                    Response.Content.Headers.ContentType =
                        new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

                    Response.Content.Headers.Add("Browser", Browser);
                    Response.Content.Headers.Add("Access-Control-Expose-Headers", "Browser");
                }
                return Response;
            }
        }

        [Route("DeleteUploadedFile")]
        [HttpGet]
        public IHttpActionResult DeleteUploadedFile(string GeneratedFileName)
        {
            using (new TimeMeasurementBlock(Logger, _logMode, CurrentUser(), GetCallerName()))
            {
                return Ok(_AppAdminService.DeleteUploadedFile(GeneratedFileName));
            }
        }

        [Route("DeleteAllUploadedFiles")]
        [HttpGet]
        public IHttpActionResult DeleteAllUploadedFiles()
        {
            using (new TimeMeasurementBlock(Logger, _logMode, CurrentUser(), GetCallerName()))
            {
                return Ok(_AppAdminService.DeleteAllUploadedFiles());
            }
        }

        #endregion

        #region get/delete Output Files

        [Route("GetOutputFilePath")]
        [HttpGet]
        public IHttpActionResult GetOutputFilePath()
        {
            using (new TimeMeasurementBlock(Logger, _logMode, CurrentUser(), GetCallerName()))
            {
                string FilePath = _OutputFilePath;
                string path = FilePath.Replace(_RootPath, "");
                return Ok(path);
            }
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
            using (new TimeMeasurementBlock(Logger, _logMode, CurrentUser(), GetCallerName()))
            {
                var result = _AppAdminService.LaunchLiveScanner(_RootPath + "bin");
                return Ok(result);
            }
        }

        [Route("LiveScannerInfo")]
        [HttpGet]
        public IHttpActionResult LiveScannerCount()
        {
            using (new TimeMeasurementBlock(Logger, _logMode, CurrentUser(), GetCallerName()))
            {
                var result = _AppAdminService.getLiveScannerProcessorsInfo();
                return Ok(result);
            }
        }

        [Route("KillLiveScanner")]
        [HttpGet]
        public IHttpActionResult KillLiveScanner()
        {
            using (new TimeMeasurementBlock(Logger, _logMode, CurrentUser(), GetCallerName()))
            {
                var result = _AppAdminService.KillLiveSiteScanner();
                return Ok(result);
            }
        }

        #endregion

        private string CurrentUser()
        {
            return User.Identity.GetUserName();
        }

        private string GetCallerName([CallerMemberName] string caller = null)
        {
            return caller;
        }

    }
}
