
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Web.Http;
using AutoMapper;
using DDAS.Models;
using DDAS.Models.Entities.Domain;
using System.Threading.Tasks;
using NLog.Config;
using NLog.Targets;
using NLog;
using System.Web;
using System.Runtime.CompilerServices;
using Microsoft.AspNet.Identity;
using DDAS.API.Helpers;
using DDAS.Models.ViewModels;

namespace DDAS.API.Controllers
{
    [RoutePrefix("api/logs")]
    public class LogsController : ApiController
    {
        private readonly IUnitOfWork _UOW;
        private readonly IMapper _Mapper;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private string _RootPath;
        private string _logMode;

        /*
         Trace
          Debug
        Info
        Warn
        Error
        Fatal
         */

        public LogsController(IUnitOfWork uow, IMapper mapper)
        {
            _UOW = uow;
            _Mapper = mapper;
            _RootPath = HttpRuntime.AppDomainAppPath;
            _logMode = System.Configuration.ConfigurationManager.AppSettings["LogMode"];

        }

        //For later use:
        //Unable to read NLog Status, hence log-start, resume not used 
        //[Route("log-resume")]
        //[HttpGet]
        //public IHttpActionResult LogResume()
        //{
        //    using (new TimeMeasurementBlock(Logger, _logMode, CurrentUser(), GetCallerName()))
        //    {
        //        NLog.LogManager.ReconfigExistingLoggers();

        //        return Ok();
        //    }
        //}

        //For later use:
        //Unable to read NLog Status, hence log-start, resume not used 
        //[Route("log-stop")]
        //[HttpGet]
        //public IHttpActionResult LogStop()
        //{
        //    using (new TimeMeasurementBlock(Logger, _logMode, CurrentUser(), GetCallerName()))
        //    {   //var config = Logger.Factory.Configuration;
        //        ////var target = (FileTarget)config.FindTargetByName("file");
        //        ////var loggingRule = new LoggingRule("*", target);
        //        //config.RemoveTarget("file");
        //        ////config.LoggingRules.Remove(loggingRule);
        //        //config.Reload();

        //        //var target = NLog.LogManager.Configuration?.FindTargetByName<BlobStorageTarget>("blob");
        //        var target = NLog.LogManager.Configuration?.FindTargetByName("file");
        //        target?.Dispose();   // Closes the target so it is uninitialized

        //        return Ok();
        //    }
        //}

        [Route("log-status")]
        [HttpGet]
        public IHttpActionResult LogStatus()
        {
            return Ok(_logMode);
            
            //using (new TimeMeasurementBlock(Logger, _logMode, CurrentUser(), GetCallerName()))
            //{
            //    bool enabled = false;
            //    var config = Logger.Factory.Configuration;

            //does not display correct value, always displays enabled = true even Logging is stopeed through a call to LogStop.
            //    if (Logger.IsInfoEnabled)
            //    {
            //        enabled = true;
            //    }

            //    return Ok(enabled);
            //}
        }

        [Route("archived-logs")]
        [HttpGet]
        public IHttpActionResult ArchivedLogs()
        {
            using (new TimeMeasurementBlock(Logger, _logMode, CurrentUser(), GetCallerName()))
            {
                var retList = new List<FileViewModel>();
                DirectoryInfo dir = new DirectoryInfo(_RootPath + @"Logs\Archive");
                var logFiles = dir.GetFiles("*.*")
                    .OrderByDescending(p => p.CreationTimeUtc)
                    .Take(100)
                    .ToList();
                //return Ok(logFiles);
                foreach (FileInfo fileInfo in logFiles)
                {
                    var fileVM = new FileViewModel();
                    fileVM.CreatedOn = fileInfo.CreationTimeUtc;
                    fileVM.FileName = fileInfo.Name;
                    fileVM.FileSize = fileInfo.Length;
                    fileVM.Path = fileInfo.FullName.Replace(_RootPath, "");
                    retList.Add(fileVM);
                }

                return Ok(retList);
            }
        }

        [Route("archived-file-count")]
        [HttpGet]
        public IHttpActionResult ArchivedFileCount()
        {
            using (new TimeMeasurementBlock(Logger, _logMode, CurrentUser(), GetCallerName()))
            {
                
                DirectoryInfo dir = new DirectoryInfo(_RootPath + @"Logs\Archive");
                var fileCount = dir.EnumerateFiles().Count();
                return Ok(fileCount);
            }
        }


        [Route("delete-archive")]
        [HttpGet]
        public IHttpActionResult DeleteArchive(int olderThan)
        {
            using (new TimeMeasurementBlock(Logger, _logMode, CurrentUser(), GetCallerName()))
            {
                FileInfo[] files = null;
                DirectoryInfo dir = new DirectoryInfo(_RootPath + @"Logs\Archive");
                files = dir.GetFiles("*.*");
                var deletedCount = 0;
                foreach (var file in files)
                {
                    if (DateTime.UtcNow - file.CreationTimeUtc > TimeSpan.FromDays(olderThan))
                    {
                        File.Delete(file.FullName);
                        deletedCount += 1;
                    }
                }

                return Ok("Deleted: " + deletedCount);
            }
        }

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
