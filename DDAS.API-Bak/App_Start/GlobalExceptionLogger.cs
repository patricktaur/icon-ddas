using DDAS.Data.Mongo;
using DDAS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

//ref: http://www.nesterovsky-bros.com/weblog/2014/03/10/CustomErrorHandlingWithWebAPI.aspx
namespace DDAS.API.App_Start
{
    public class GlobalExceptionLogger:ExceptionLogger
    {
        private IUnitOfWork _UOW;

        private string _ConnStr =
            System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        private string _DBName =
            System.Configuration.ConfigurationManager.AppSettings["DBName"];

        public GlobalExceptionLogger()
        {
            _UOW = new UnitOfWork(_ConnStr, _DBName);
        }

        public override void Log(ExceptionLoggerContext context)
        {
            var log = context.Exception.ToString();

            //_UOW =
            //    GlobalConfiguration.Configuration.DependencyResolver.GetService(
            //        typeof(IUnitOfWork)) as IUnitOfWork;

            try
            {
                var request = context.Request;
                var exception = context.Exception;

                var id = LogError(
                  request.RequestUri.ToString(),
                  context.RequestContext == null ?
                    null : context.RequestContext.Principal.Identity.Name,
                  request.ToString(),
                  exception.Message,
                  exception.StackTrace);

                // associates retrieved error ID with the current exception
                exception.Data["NesterovskyBros:id"] = id;
            }
            catch (Exception)
            {
                // logger shouldn't throw an exception!!!
            }
        }

        private long LogError(
            string address,
            string userid,
            string request,
            string message,
            string stackTrace
            )
        {
            long LastId = 0;

            var LastRec = _UOW.ExceptionLoggerRepository.GetAll().LastOrDefault();

            if (LastRec != null)
                LastId = LastRec.Id;
            
            var logger = new Models.Entities.ExceptionLogger();

            logger.AddedOn = DateTime.Now;
            logger.Address = address;
            logger.UserId = userid;
            logger.Request = request;
            logger.Message = message;
            logger.StackTrace = stackTrace;
            logger.Id = LastId + 1;

            _UOW.ExceptionLoggerRepository.Add(logger);

            return LastId;
        }
    }
}