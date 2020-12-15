using System;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

using OGC.Data.SharePoint.Models;
using System.Collections.Generic;
using OGC.Data.SharePoint;
using OGC.Training.API.Models;

namespace OGC.Training.API.Controllers
{
    [Authorize]
    public class NotificationsController : BaseController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            try
            {
                var identity = HttpContext.Current.User.Identity as ClaimsIdentity;
                AppUser = UserInfo.GetUser(identity);
                
                var notifications = Notifications.GetAllBy("Recipient", AppUser.Email);
                var announcements = notifications.Where(x => x.IsAnnouncment == true).ToList();

                notifications = Notifications.GetAllBy("Recipient", "Global");
                announcements.AddRange(notifications);

                var vm = new NotificationsVm();
                vm.Records = announcements.Count;
                vm.TotalRecords = announcements.Count;
                vm.notifications = announcements.OrderByDescending(x => x.Created).ToList();

                return Json(vm, CamelCase);

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

                var notifications = Notifications.GetAllBy("Recipient", AppUser.Email);
                var announcements = notifications.Where(x => x.IsAnnouncment == true).ToList();

                notifications = Notifications.GetAllBy("Recipient", "Global");
                announcements.AddRange(notifications);

                var vm = new NotificationsVm();
                vm.Records = id < announcements.Count ? id : announcements.Count;
                vm.TotalRecords = announcements.Count;
                vm.notifications = announcements.OrderByDescending(x => x.Created).Take(id).ToList();

                return Json(vm, CamelCase);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
