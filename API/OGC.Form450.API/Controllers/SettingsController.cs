using System;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

using OGC.Data.SharePoint.Models;


namespace OGC.Form450.API.Controllers
{
    [Form450Authorize("requireAuthorization")]
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

        [HttpPut]
        public IHttpActionResult Update(Settings item)
        {
            try
            {
                var identity = HttpContext.Current.User.Identity as ClaimsIdentity;
                OGE450User = UserInfo.GetUser(identity);

                if (OGE450User.IsAdmin)
                {
                    return Json(item.Save(), CamelCase);
                }
                else
                    return Unauthorized();
                
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
