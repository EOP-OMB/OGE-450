using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.SharePoint.Client;
using OMB.SharePoint.Infrastructure;

namespace OGC.Data.SharePoint.Models
{
    public class Attendee : SPListBase<Attendee>, ISPList
    {
        #region Properties
        public int EventRequestId { get; set; }
        public string EventName { get; set; }
        
        public string Capacity { get; set; }
        public string EmployeeType { get; set; }
        public bool IsGivingRemarks { get; set; }
        public string Remarks { get; set; }
        public string ReasonForAttending { get; set; }

        public UserInfo Employee { get; set; }
        #endregion

        public Attendee()
        {
            this.ListName = "Attendees";
        }

        #region Mapping
        public override void MapToList(ListItem dest)
        {
            if (Employee != null && !string.IsNullOrEmpty(Employee.Upn))
                dest["Attendee"] = SharePointHelper.GetFieldUser(Employee.Upn);
            
            base.MapToList(dest);

            var flv = new FieldLookupValue();
            flv.LookupId = EventRequestId;

            dest["Title"] = Employee.DisplayName; 
            dest["EventRequestId"] = flv;

            dest["Capacity"] = Capacity;
            dest["EmployeeType"] = EmployeeType;
            dest["IsGivingRemarks"] = Convert.ToBoolean(IsGivingRemarks);
            dest["Remarks"] = Remarks;
            dest["ReasonForAttending"] = ReasonForAttending;
        }

        public static void FixAttendee()
        {
            var attendees = Attendee.GetAll();

            var needfix = attendees.Where(x => x.Employee.Upn.Contains("login\\") || x.Employee.Upn == "").ToList();

            foreach (Attendee a in needfix)
            {
                var upn = UserInfo.GetUserByName(a.Title);

                if (SharePointHelper.GetFieldUser(upn) != null)
                {
                    a.Employee.Upn = upn;

                    a.Save();
                }
            }
        }

        public override void MapFromList(ListItem item, bool includeChildren = false)
        {
            base.MapFromList(item);

            var upn = item["Attendee"] != null ? ((FieldUserValue)item["Attendee"]).LookupValue : "";

            Employee = new UserInfo() { Upn = !string.IsNullOrEmpty(upn) ? upn : "", DisplayName = Title };

            EventRequestId = ((FieldLookupValue)item["EventRequestId"]).LookupId;
            Capacity = SharePointHelper.ToStringNullSafe(item["Capacity"]);
            EmployeeType = SharePointHelper.ToStringNullSafe(item["EmployeeType"]);
            IsGivingRemarks = Convert.ToBoolean(item["IsGivingRemarks"]);
            Remarks = SharePointHelper.ToStringNullSafe(item["Remarks"]);
            ReasonForAttending = SharePointHelper.ToStringNullSafe(item["ReasonForAttending"]);
        }
        #endregion
    }
}
