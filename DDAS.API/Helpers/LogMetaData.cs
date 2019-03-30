using System;
using System.Net;
//https://www.infoworld.com/article/3211590/how-to-log-request-and-response-metadata-in-aspnet-web-api.html
namespace DDAS.API.Helpers
{
    public class LogMetaData
    {
        public string RequestContentType { get; set; }
        public string RequestUri { get; set; }
        public string RequestMethod { get; set; }
        public DateTime? RequestTimestamp { get; set; }
        public string ResponseContentType { get; set; }
        public HttpStatusCode ResponseStatusCode { get; set; }
        public DateTime? ResponseTimestamp { get; set; }
    }
}