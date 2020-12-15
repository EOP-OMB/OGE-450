using System;
using System.Linq;
using System.Web.Http;

using OGC.Data.SharePoint.Models;
using System.Collections.Generic;
using System.Web;
using System.Security.Claims;
using OMB.SharePoint.Infrastructure;

namespace OGC.Event.API.Controllers
{
    public class AdminController : BaseController
    {
        [HttpGet]
        public IHttpActionResult Get(string a)
        {
            //var identity = HttpContext.Current.User.Identity as ClaimsIdentity;
            //AppUser = UserInfo.GetUser(identity);

            try
            {

                if (a == "reviewers")
                {
                    //var principal = HttpContext.Current.User.Identity as ClaimsIdentity;
                    var results = UserInfo.GetUserByGroup(SharePointHelper.ReviewerGroup);

                    return Json(results, CamelCase);
                }
                //else if (a =="migration")
                //{
                //    var results = EthicsClearance.Migrate();

                //    return Json(results, CamelCase);
                //}
                //else if (a.StartsWith("submitter"))
                //{
                //    FixSubmitterInfo();
                //    Attendee.FixAttendee();

                //    return Ok("OK");
                //}
                //else if (a.StartsWith("test"))
                //{
                //    return Ok("Goodbye World!");
                //}

                return BadRequest("No such action.");
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        //public void FixSubmitterInfo()
        //{
        //    var events = EventRequest.GetAll();
        //    events = events.Where(x => x.Modified > new DateTime(2018, 9, 11)).ToList();

        //    foreach (EventRequest e in events)
        //    {
        //        if (!string.IsNullOrEmpty(e.SubmittedBy))
        //        {
        //            e.Submitter = e.SubmittedBy;
        //            e.SubmittedBy = UserInfo.GetUserByName(e.SubmittedBy);
        //            e.Save();
        //        }
        //        else if (!string.IsNullOrEmpty(e.Submitter))
        //        {
        //            e.SubmittedBy = UserInfo.GetUserByName(e.Submitter);
        //            e.Save();
        //        }
        //    }
        //}
    }
}
