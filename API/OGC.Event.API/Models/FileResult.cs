using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace OGC.Event.API.Models
{
    public class FileResult : IHttpActionResult
    {
        MemoryStream ResultStream;
        string FileName;
        HttpRequestMessage HttpRequestMessage;
        HttpResponseMessage HttpResponseMessage;

        public FileResult(MemoryStream data, HttpRequestMessage request, string filename)
        {
            ResultStream = data;
            HttpRequestMessage = request;
            FileName = filename;
        }

        public System.Threading.Tasks.Task<HttpResponseMessage> ExecuteAsync(System.Threading.CancellationToken cancellationToken)
        {
            HttpResponseMessage = HttpRequestMessage.CreateResponse(HttpStatusCode.OK);
            HttpResponseMessage.Content = new StreamContent(ResultStream);
            HttpResponseMessage.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            HttpResponseMessage.Content.Headers.ContentDisposition.FileName = FileName;
            HttpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

            return System.Threading.Tasks.Task.FromResult(HttpResponseMessage);
        }
    }
}