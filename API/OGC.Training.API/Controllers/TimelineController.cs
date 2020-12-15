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
    public class TimelineController : BaseController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            try
            {
                var identity = HttpContext.Current.User.Identity as ClaimsIdentity;
                AppUser = UserInfo.GetUser(identity);

                var timeline = GetTimeline();

                var vm = new TimelineVm();

                vm.TotalRecords = timeline.Count;
                vm.Records = timeline.Count;

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

                var timeline = GetTimeline();
                var vm = new TimelineVm();

                vm.TotalRecords = timeline.Count;
                vm.Records = id < timeline.Count ? id : timeline.Count;

                timeline = timeline.Take(id).ToList();

                vm.timeline = timeline;

                return Json(vm, CamelCase);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        private List<Timeline> GetTimeline()
        {
            var timeline = new List<Timeline>();

            var forms = OGEForm450.GetAllByUser("Filer", AppUser.Id);

            foreach (OGEForm450 form in forms)
            {
                timeline.Add(new Timeline() { Type = "OGEForm450", Title = "OGE Form 450 Assigned", Date = form.Created, Id = form.Id });

                if (form.DateReceivedByAgency != null)
                    timeline.Add(new Timeline() { Type = "OGEForm450", Title = "OGE Form 450 Submitted", Date = form.DateReceivedByAgency, Id = form.Id });

                if (form.DateOfReviewerSignature != null)
                    timeline.Add(new Timeline() { Type = "OGEForm450", Title = "OGE Form 450 Certified", Date = form.DateOfReviewerSignature, Id = form.Id });
            }

            var trainings = Data.SharePoint.Models.Training.GetAllByUser("Employee", AppUser.Id).OrderByDescending(x => x.DateAndTime).ToList();

            foreach (Data.SharePoint.Models.Training t in trainings)
            {
                timeline.Add(new Timeline() { Type = "Training", Title = "Annual Ethics Training Completed", Date = t.DateAndTime, Id = t.Id });
            }

            var events = GetMyEvents(AppUser);

            foreach (Data.SharePoint.Models.EventRequest e in events)
            {
                timeline.Add(new Timeline() { Type = "Event", Title = e.EventName, Date = e.EventStartDate, Id = e.Id });
            }

            timeline = timeline.OrderByDescending(x => x.Date).ToList();

            return timeline;
        }

        private List<EventRequest> GetMyEvents(UserInfo user)
        {
            var list = new List<EventRequest>(); // EventRequest.GetAllByUser("SubmittedBy", user.Id).ToList();

            var attendees = Attendee.GetAllByUser("Attendee", user.Id).ToList();

            foreach (Attendee att in attendees)
            {
                if (!list.Any(x => x.Id == att.EventRequestId))
                {
                    // i'm an attendee on an event submitted by someone else, add the event
                    var req = EventRequest.Get(att.EventRequestId);

                    list.Add(req);
                }
            }

            return list.OrderByDescending(x => x.EventStartDate).ToList();
        }

    }
}
