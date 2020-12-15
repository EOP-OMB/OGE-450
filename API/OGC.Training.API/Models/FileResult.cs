using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace OGC.Training.API.Models
{
    public class FileResult : IHttpActionResult
    {
        MemoryStream ResultStream;
        string FileName;
        HttpRequestMessage HttpRequestMessage;
        HttpResponseMessage HttpResponseMessage;

        MediaTypeHeaderValue ContentType;

        public FileResult(MemoryStream data, HttpRequestMessage request, string filename, string contentType = null)
        {
            ResultStream = data;
            HttpRequestMessage = request;
            FileName = filename;
            if (!string.IsNullOrEmpty(contentType)) {
                ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
            }
        }

        public System.Threading.Tasks.Task<HttpResponseMessage> ExecuteAsync(System.Threading.CancellationToken cancellationToken)
        {
            HttpResponseMessage = HttpRequestMessage.CreateResponse(HttpStatusCode.OK);
            HttpResponseMessage.Content = new StreamContent(ResultStream);
            HttpResponseMessage.Content.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
            HttpResponseMessage.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment") { FileName = FileName };
            HttpResponseMessage.Content.Headers.ContentType = ContentType;

            return System.Threading.Tasks.Task.FromResult(HttpResponseMessage);
        }
    }
}