using System;
using System.Linq;
using System.Web.Http;

using OGC.Data.SharePoint.Models;
using System.Collections.Generic;
using System.Web;
using System.Security.Claims;

namespace OGC.Training.API.Controllers
{
    [Authorize]
    public class EventRequestController : BaseController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            try
            {
                var identity = HttpContext.Current.User.Identity as ClaimsIdentity;
                AppUser = UserInfo.GetUser(identity);
                List<EventRequest> list;

                if (AppUser.IsAdmin)
                {
                    list = EventRequest.GetAll().OrderBy(x => x.EventStartDate).ToList();

                    GetAttendeesForList(list);
                }
                else if (AppUser.IsReviewer)
                {
                    list = EventRequest.GetAll().Where(x => x.Status.Contains("Open")).OrderBy(x => x.EventStartDate).ToList();

                    GetAttendeesForList(list);
                }
                else
                    list = GetMyEvents(AppUser);

                return Json(list, CamelCase);
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
                EventRequest item;

                item = EventRequest.Get(id);

                return Json(item, CamelCase);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }


        [HttpGet]
        public IHttpActionResult Get(string a)
        {
            var identity = HttpContext.Current.User.Identity as ClaimsIdentity;
            AppUser = UserInfo.GetUser(identity);

            try
            {
                if (a == "myevents")
                    return Json(GetMyEvents(AppUser), CamelCase);
                else if (a == "open")
                    return Json(GetOpenEvents(AppUser), CamelCase);

                var split = a.Split(':');

                if (split.Length > 0)
                {
                    if (split[0] == "assignedTo")
                    {
                        var id = Convert.ToInt32(split[1]);
                        var request = EventRequest.Get(id);

                        request.ResendAssignedToEmail(AppUser);

                        return Ok();
                    }
                }

                return BadRequest("No such action.");
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        private List<EventRequest> GetOpenEvents(UserInfo appUser)
        {
            List<EventRequest> list;

            if (AppUser.IsAdmin || AppUser.IsReviewer)
            {
                list = EventRequest.GetAllBy("Status", Constants.EventRequestStatus.OPEN).ToList();

                var list2 = EventRequest.GetAllBy("Status", Constants.EventRequestStatus.UNASSIGNED).ToList();

                list.AddRange(list2);

                GetAttendeesForList(list);
            }
            else
                list = GetMyEvents(AppUser);

            return list.OrderBy(x => x.EventStartDate).ToList();

        }

        private List<EventRequest> GetMyEvents(UserInfo user)
        {
            var list = EventRequest.GetAllByUser("SubmittedBy", user.Id).ToList();

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

            GetAttendeesForList(list);

            return list.OrderBy(x => x.Status == Constants.EventRequestStatus.DRAFT ? 0 : 1).ThenByDescending(x => x.EventStartDate).ToList();
        }

        private void GetAttendeesForList(List<EventRequest> list)
        {
            var attendees = Attendee.GetAll();

            foreach (EventRequest req in list)
            {
                req.Attendees = attendees.Where(x => x.EventRequestId == req.Id).ToList();
            }
        }

        //private List<Data.SharePoint.Models.Training> GetMyTrainings(UserInfo user)
        //{
        //    var list = Data.SharePoint.Models.Training.GetAllByUser("Employee", user.Id).OrderByDescending(x => x.DateAndTime).ToList();

        //    return list;
        //}

        [HttpPut]
        public IHttpActionResult Update(EventRequest item)
        {
            try
            {
                var oldItem = EventRequest.Get(item.Id);
                return UpdateEventRequest(item, oldItem);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost]
        public IHttpActionResult Create(EventRequest item)
        {
            try
            {
                return UpdateEventRequest(item, null);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        private IHttpActionResult UpdateEventRequest(EventRequest item, EventRequest oldItem)
        {
            var identity = HttpContext.Current.User.Identity as ClaimsIdentity;
            AppUser = UserInfo.GetUser(identity);

            var result = item.RunBusinessRules(AppUser, AppUser, oldItem);
            if (string.IsNullOrEmpty(result))
            {

                var newItem = item.Save();

                item.SaveAttendees(newItem.Id);
                item.UpdateAttachments(newItem.Id);

                newItem = EventRequest.Get(newItem.Id);

                item.ProcessEmails();

                return Json(newItem, CamelCase);
            }
            else
                return BadRequest(result);
        }
    }
}
