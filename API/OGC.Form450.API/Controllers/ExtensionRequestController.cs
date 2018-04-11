using System;
using System.Linq;
using System.Security.Claims;
using System.Web.Http;
using System.Web;

using OGC.Data.SharePoint.Models;

namespace OGC.Form450.API.Controllers
{
    [Form450Authorize("requireAuthorization")]
    public class ExtensionRequestController : BaseController
    {
        [HttpPost]
        public IHttpActionResult Add(ExtensionRequest item)
        {
            var identity = HttpContext.Current.User.Identity as ClaimsIdentity;
            OGE450User = UserInfo.GetUser(identity);

            try
            {
                var result = item.RunBusinessRules(OGE450User, null);
                if (result == "")
                {
                    var ext = item.Save();
                    item.ProcessEmails();

                    return Json(ext, CamelCase); 
                }
                else
                    throw new Exception(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet]
        public IHttpActionResult Get()
        {
            try
            {
                var identity = HttpContext.Current.User.Identity as ClaimsIdentity;
                OGE450User = UserInfo.GetUser(identity);

                if (OGE450User.IsAdmin || OGE450User.IsReviewer)
                {
                    var extensions = ExtensionRequest.GetAll();

                    return Json(extensions, CamelCase);
                }
                else
                    return Unauthorized();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet]
        public IHttpActionResult Get(string a)
        {
            try
            {
                var identity = HttpContext.Current.User.Identity as ClaimsIdentity;
                OGE450User = UserInfo.GetUser(identity);

                if (a == "pending")
                {
                    if (OGE450User.IsAdmin || OGE450User.IsReviewer)
                    {
                        var extensions = ExtensionRequest.GetAllBy("Status", "Pending");

                        return Json(extensions, CamelCase);
                    }
                    else
                        return Unauthorized();
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

        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            try
            {
                var identity = HttpContext.Current.User.Identity as ClaimsIdentity;
                OGE450User = UserInfo.GetUser(identity);

                // Can only access extensions if is a reviewer or admin or if it is your extension request
                if (OGE450User.IsReviewer || OGE450User.IsAdmin || OGE450User.CurrentFormId == id)
                {
                    var pendingExtensions = ExtensionRequest.GetPendingExtensions(id);

                    // Should only be one.
                    var extension = pendingExtensions.FirstOrDefault();

                    if (extension == null)
                    {
                        if (OGE450User.CurrentFormId == id)
                        {
                            // If no extension for form and is user's current form, create a new extension
                            extension = new ExtensionRequest();
                            extension.OGEForm450Id = id;
                            extension.DaysRequested = 0;
                        }
                        else
                            throw new Exception("No extension found for form id " + id.ToString());
                    }

                    return Json(extension, CamelCase);
                }
                else
                    return Unauthorized();
                
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut]
        public IHttpActionResult Update(ExtensionRequest item)
        {
            var oldItem = ExtensionRequest.Get(item.Id);

            var identity = HttpContext.Current.User.Identity as ClaimsIdentity;
            OGE450User = UserInfo.GetUser(identity);

            try
            {
                // Can only access extensions if is a reviewer or admin or if it is your extension request
                if (OGE450User.IsReviewer || OGE450User.IsAdmin || OGE450User.CurrentFormId == item.OGEForm450Id)
                {
                    var result = item.RunBusinessRules(OGE450User, oldItem);
                    if (result == "")
                    {
                        var ext = item.Save();

                        if (item.Status == "Approved")
                        {
                            var form = OGEForm450.Get(item.OGEForm450Id);

                            form.Extend(item);
                        }

                        item.ProcessEmails();

                        return Json(ext, CamelCase);
                    }
                    else
                        throw new Exception(result);
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
