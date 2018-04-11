using System;
using System.Linq;
using System.Web.Http;

using OGC.Data.SharePoint.Models;
using System.Collections.Generic;
using System.Web;
using System.Security.Claims;

namespace OGC.Form450.API.Controllers
{
    [Form450Authorize("requireAuthorization")]
    public class SupportContactsController : BaseController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            try
            {
                var contacts = SupportContacts.GetAll().OrderBy(x => x.SortOrder);

                return Json(contacts, CamelCase);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut]
        public IHttpActionResult Update(List<SupportContacts> items)
        {
            try
            {
                var identity = HttpContext.Current.User.Identity as ClaimsIdentity;
                OGE450User = UserInfo.GetUser(identity);

                if (OGE450User.IsAdmin)
                {
                    var list = new List<SupportContacts>();

                    foreach (SupportContacts item in items)
                    {
                        if (string.IsNullOrEmpty(item.Title) && string.IsNullOrEmpty(item.Info))
                        {
                            if (item.Id > 0)
                            {
                                item.Delete();
                            }
                            else
                            {
                                // Ignore empty add
                            }
                        }
                        else
                        {
                            list.Add(item.Save());
                        }
                    }

                    return Json(list, CamelCase);
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
