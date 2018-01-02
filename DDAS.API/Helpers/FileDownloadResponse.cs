using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Net;
using System.IO;
using System.Net.Http.Headers;
using System.Text;

namespace DDAS.API.Helpers
{
    public class FileDownloadResponse
    {
        private CSVConvertor _csvConvertor;
        public FileDownloadResponse() {
            _csvConvertor = new CSVConvertor();
        }

        public HttpResponseMessage GetResponse(HttpRequestMessage Request, string content, string FileName)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(content);
            MemoryStream stream = new MemoryStream(byteArray);
            return GetResponse(Request, stream, FileName);
        }


        public HttpResponseMessage GetResponse(HttpRequestMessage Request, object list, string FileName)
        {
            var content = _csvConvertor.ConvertToCSVString(list);
            byte[] byteArray = Encoding.UTF8.GetBytes(content);
            MemoryStream stream = new MemoryStream(byteArray);
            return GetResponse(Request, stream, FileName);
        }


        public HttpResponseMessage GetResponse(HttpRequestMessage Request, object list, string FileName, List<string> headers)
        {
            var contents = _csvConvertor.ConvertToCSVString(list, headers);
            byte[] byteArray = Encoding.UTF8.GetBytes(contents);
            MemoryStream stream = new MemoryStream(byteArray);
            return GetResponse(Request, stream, FileName);
        }
        public HttpResponseMessage GetResponse(HttpRequestMessage Request, MemoryStream memoryStream, string FileName)
        {

            var UserAgent = Request.Headers.UserAgent.ToString();
            var Browser = IdentifyBrowser.GetBrowserType(UserAgent);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new ByteArrayContent(memoryStream.ToArray());

            response.Content.Headers.ContentDisposition =
                new ContentDispositionHeaderValue("attachment");

            response.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/ms-word");

            response.Content.Headers.ContentDisposition.FileName = FileName;

            var FileNameHeader = FileName + " " + Browser;
            //add custom headers to the response
            //easy for angular2 to read this header
            response.Content.Headers.Add("Filename", FileNameHeader);
            //response.Content.Headers.Add("Browser", Browser);
            response.Content.Headers.Add("Access-Control-Expose-Headers", "Filename");

            return response;
        }

    }
}