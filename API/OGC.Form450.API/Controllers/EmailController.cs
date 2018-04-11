using System;
using System.Web.Http;

using OGC.Data.SharePoint;

namespace OGC.Form450.API.Controllers
{
    public class EmailController : BaseController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            try
            {
                EmailHelper.ProcessEmails();

                return Json("OK", CamelCase);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
