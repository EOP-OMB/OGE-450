using System;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

using OGC.Data.SharePoint.Models;

namespace OGC.Training.API.Controllers
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
    }
}
