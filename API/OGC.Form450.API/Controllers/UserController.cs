using System;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

using OGC.Data.SharePoint.Models;

namespace OGC.Form450.API.Controllers
{
    [Form450Authorize("requireAuthorization")]
    public class UserController : BaseController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            try
            {
                var principal = HttpContext.Current.User.Identity as ClaimsIdentity;
                OGE450User = UserInfo.GetUser(principal, true);

                return Json(OGE450User, CamelCase);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
