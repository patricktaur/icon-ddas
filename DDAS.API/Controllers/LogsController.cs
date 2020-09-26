
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

namespace DDAS.API.Controllers
{
    [RoutePrefix("api/logs")]
    public class LogsController : ApiController
    {
        private readonly IUnitOfWork _UOW;
        private readonly IMapper _Mapper;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private string _RootPath;

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
        }




        [Route("log-resume")]
        [HttpGet]
        public IHttpActionResult LogResume()
        {
            //var config = Logger.Factory.Configuration;
            ////var target = (FileTarget)config.FindTargetByName("file");
            //var target = new FileTarget();
            //target.FileName = "Logs / log.txt";
            //target.FileNameKind = FilePathKind.Relative;
            //target.ArchiveFileName = "Logs/Archive/log.{#}.txt";
            //target.ArchiveNumbering = NLog.Targets.ArchiveNumberingMode.Date; //"Date";
            //target.ArchiveEvery = NLog.Targets.FileArchivePeriod.Day; //"Day"
            //target.ArchiveDateFormat = "yyyyMMdd";
            //target.Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}";

            //var loggingRule = new LoggingRule("*", LogLevel.Debug, target);
            //config.LoggingRules.Add(loggingRule);

            //config.AddTarget(target);
            ////config.LoggingRules.Add(loggingRule);
            //config.Reload();
            using (new TimeMeasurementBlock(Logger, CurrentUser(), GetCallerName()))
            {
                NLog.LogManager.ReconfigExistingLoggers();

                return Ok();
            }
        }


        [Route("log-stop")]
        [HttpGet]
        public IHttpActionResult LogStop()
        {
            using (new TimeMeasurementBlock(Logger, CurrentUser(), GetCallerName()))
            {   //var config = Logger.Factory.Configuration;
                ////var target = (FileTarget)config.FindTargetByName("file");
                ////var loggingRule = new LoggingRule("*", target);
                //config.RemoveTarget("file");
                ////config.LoggingRules.Remove(loggingRule);
                //config.Reload();

                //var target = NLog.LogManager.Configuration?.FindTargetByName<BlobStorageTarget>("blob");
                var target = NLog.LogManager.Configuration?.FindTargetByName("file");
                target?.Dispose();   // Closes the target so it is uninitialized

                return Ok();
            }
        }

        [Route("log-status")]
        [HttpGet]
        public IHttpActionResult LogStatus()
        {
            using (new TimeMeasurementBlock(Logger, CurrentUser(), GetCallerName()))
            {
                bool enabled = false;
                var config = Logger.Factory.Configuration;

                if (Logger.IsInfoEnabled)
                {
                    enabled = true;
                }

                return Ok(enabled);
            }
        }

        [Route("archived-logs")]
        [HttpGet]
        public IHttpActionResult ArchivedLogs()
        {
            using (new TimeMeasurementBlock(Logger, CurrentUser(), GetCallerName()))
            {
                var retList = new List<FileInfo>();
                DirectoryInfo dir = new DirectoryInfo(_RootPath + @"Logs\Archive");
                var logFiles = dir.GetFiles("*.*").OrderBy(p => p.CreationTimeUtc).ToList();

                foreach (FileInfo fileInfo in logFiles)
                {
                    retList.Add(fileInfo);
                }

                return Ok(retList);
            }
        }

        [Route("delete-archive")]
        [HttpGet]
        public IHttpActionResult DeleteArchive(int olderThan)
        {
            using (new TimeMeasurementBlock(Logger, CurrentUser(), GetCallerName()))
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
