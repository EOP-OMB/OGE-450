using System;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

using OGC.Data.SharePoint.Models;
using OMB.SharePoint.Infrastructure;

namespace OGC.Event.API.Controllers
{
    [Authorize]
    public class UserController : BaseController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            try
            {
                var principal = HttpContext.Current.User.Identity as ClaimsIdentity;
                AppUser = UserInfo.GetUser(principal, true);

                return Json(AppUser, CamelCase);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        public IHttpActionResult Get(string query)
        {
            try
            {
                var principal = HttpContext.Current.User.Identity as ClaimsIdentity;
                var results = UserInfo.Search(query);

                return Json(results, CamelCase);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
