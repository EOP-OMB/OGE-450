using System;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

using OGC.Data.SharePoint.Models;

namespace OGC.Training.API.Controllers
{
    [Authorize]
    public class SettingsController : BaseController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            try
            {
                var settings = Settings.GetAll().FirstOrDefault();

                return Json(settings, CamelCase);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
