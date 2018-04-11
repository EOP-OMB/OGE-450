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
    public class AdminController : BaseController
    {
        [HttpGet]
        public IHttpActionResult Get(string a)
        {
            try
            {
                if (a.ToLower() == "sync")
                {
                    Employee.SyncUserProfilesToEmployees();

                    return Ok("OK");
                }
                else
                {
                    return BadRequest("No such action.");
                }
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

    }
}
