
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using DDAS.Models;
using DDAS.Models.Entities.Domain;
using System.Threading.Tasks;
using NLog.Config;
using NLog.Targets;
using NLog;

namespace DDAS.API.Controllers
{
    [RoutePrefix("api/logs")]
    public class LogsController : ApiController
    {
        private readonly IUnitOfWork _UOW;
        private readonly IMapper _Mapper;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

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

            NLog.LogManager.ReconfigExistingLoggers();

            return Ok();
        }

    
        [Route("log-stop")]
        [HttpGet]
        public IHttpActionResult LogStop()
        {
            //var config = Logger.Factory.Configuration;
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

        [Route("log-status")]
        [HttpGet]
        public IHttpActionResult LogStatus()
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
}
