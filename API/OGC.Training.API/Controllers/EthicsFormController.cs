using System;
using System.Linq;
using System.Web.Http;

using OGC.Data.SharePoint.Models;
using System.Collections.Generic;
using System.Web;
using System.Security.Claims;
using System.Web.Http.Description;
using System.IO;
using OGC.Training.API.Models;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;

namespace OGC.Training.API.Controllers
{
    [Authorize]
    public class EthicsFormController : BaseController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            var files = EthicsForm.GetAllDocuments();

            return Json(files.OrderBy(x => x.SortOrder), CamelCase); 
        }

        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            var file = EthicsForm.Get(id);

            string extension = Path.GetExtension(file.FileName);
            string mimeType;

            if (!String.IsNullOrEmpty(extension) && MimeTypeLookup.Mappings.TryGetValue(extension, out mimeType))
                file.ContentType = mimeType;
            else
                file.ContentType = "application/octet-stream";


            //adding bytes to memory stream   
            var dataStream = new MemoryStream(file.Content);

            return new FileResult(dataStream, Request, file.FileName, file.ContentType);
        }
    }
}