using System;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

using OGC.Data.SharePoint.Models;
using System.Collections.Generic;
using OGC.Data.SharePoint;

namespace OGC.Training.API.Controllers
{
    [Authorize]
    public class NotificationTemplateController : BaseController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            try
            {
                var identity = HttpContext.Current.User.Identity as ClaimsIdentity;
                AppUser = UserInfo.GetUser(identity);

                if (AppUser.IsAdmin)
                {
                    var templates = NotificationTemplates.GetAll();

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
        
        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            try
            {
                var identity = HttpContext.Current.User.Identity as ClaimsIdentity;
                AppUser = UserInfo.GetUser(identity);

                if (AppUser.IsAdmin)
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

        [HttpPut]
        public IHttpActionResult Update(NotificationTemplates item)
        {
            try
            {
                var identity = HttpContext.Current.User.Identity as ClaimsIdentity;
                AppUser = UserInfo.GetUser(identity);

                if (AppUser.IsAdmin)
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
