using System;
using System.Linq;
using System.Web.Http;

using OGC.Data.SharePoint.Models;
using System.Collections.Generic;
using System.Web;
using System.Security.Claims;
using System.Web.Http.Description;
using System.IO;
using OGC.Event.API.Models;

namespace OGC.Event.API.Controllers
{
    [Authorize]
    public class AttachmentController : BaseController
    {
        [HttpOptions]
        public IHttpActionResult Options()
        {
            HttpContext.Current.Response.AppendHeader("Access-Control-Allow-Methods", "POST");
            HttpContext.Current.Response.AppendHeader("Access-Control-Allow-Headers", "X-Requested-With,content-type");
            HttpContext.Current.Response.AppendHeader("Access-Control-Allow-Credentials", "true");

            return Ok();
        }

        [HttpPost]
        public IHttpActionResult Create()
        {
            var httpRequest = HttpContext.Current.Request;

            if (httpRequest.Files.Count > 0)
            {
                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    var bytes = new byte[postedFile.ContentLength];

                    var attachment = new Attachment();

                    using (var memory = new MemoryStream())
                    {
                        postedFile.InputStream.CopyTo(memory);
                        bytes = memory.ToArray();
                    }
                    
                    attachment.Content = bytes;
                    attachment.FileName = postedFile.FileName;

                    attachment.AttachmentGuid = httpRequest.Params["guid"].ToString();
                    attachment.EventRequestId = Convert.ToInt32(httpRequest.Params["id"]);
                    attachment.TypeOfAttachment = httpRequest.Params["type"].ToString();
                    attachment.Size = bytes.Length;

                    attachment.Create();
                }
            }

            return Json("OK", CamelCase);
        }

        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            var file = Attachment.Get(id);
            
            //adding bytes to memory stream   
            var dataStream = new MemoryStream(file.Content);

            return new FileResult(dataStream, Request, file.FileName);
        }
    }
}

