using System;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

using OGC.Data.SharePoint.Models;
using System.Collections.Generic;
using OGC.Data.SharePoint;

namespace OGC.Form450.API.Controllers
{
    public class NotificationTemplateController : BaseController
    {
        [Form450Authorize("requireAuthorization")]
        [HttpGet]
        public IHttpActionResult Get()
        {
            try
            {
                var identity = HttpContext.Current.User.Identity as ClaimsIdentity;
                OGE450User = UserInfo.GetUser(identity);

                if (OGE450User.IsAdmin)
                {
                    var templates = NotificationTemplates.GetAllBy("Application", Constants.ApplicationName.OGE_FORM_450);

                    return Json(templates.OrderBy(x => x.Title), CamelCase);
                }
                else
                {
                    return Unauthorized();
                }

            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [Form450Authorize("requireAuthorization")]
        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            try
            {
                var identity = HttpContext.Current.User.Identity as ClaimsIdentity;
                OGE450User = UserInfo.GetUser(identity);

                if (OGE450User.IsAdmin)
                {
                    var template = NotificationTemplates.Get(id);

                    return Json(template, CamelCase);
                }
                else
                {
                    return Unauthorized();
                }

            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [Form450Authorize("requireAuthorization")]
        [HttpPut]
        public IHttpActionResult Update(NotificationTemplates item)
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
