using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.SharePoint.Client;
using OMB.SharePoint.Infrastructure;

namespace OGC.Data.SharePoint.Models
{
    public class EventRequest : SPListBase<EventRequest>, ISPList
    {
        #region properties
        public string SubmittedBy { get; set; }
        public string EventName { get; set; }
        public string GuestsInvited { get; set; }
        public DateTime? EventStartDate { get; set; }
        public DateTime? EventEndDate { get; set; }
        public string EventContactName { get; set; }
        public string EventContactPhone { get; set; }
        public string IndividualExtendingInvite { get; set; }
        public string IsIndividualLobbyist { get; set; }
        public string OrgExtendingInvite { get; set; }
        public string IsOrgLobbyist { get; set; }
        public int TypeOfOrg { get; set; }
        public string OrgHostingEvent { get; set; }
        public string IsHostLobbyist { get; set; }
        public int TypeOfHost { get; set; }
        public string IsFundraiser { get; set; }
        public string WhoIsPaying { get; set; }
        public decimal FairMarketValue { get; set; }
        public string RequiresTravel { get; set; }
        public string InternationalTravel { get; set; }
        public string AdditionalInformation { get; set; }
        public string EventLocation { get; set; }
        public string CrowdDescription { get; set; }
        public int ApproximateAttendees{ get; set; }
        public string IsOpenToMedia { get; set; }
        public string GuidanceGiven { get; set; }
        public string AssignedTo { get; set; }
        public string Status { get; set; }
        public string ClosedReason { get; set; }
        public string ClosedBy { get; set; }
        public DateTime? ClosedDate { get; set; }
        public string ContactNumber { get; set; }
        public string ContactEmail { get; set; }
        public string ContactComponent { get; set; }

        public List<Attendee> Attendees { get; set; }
        #endregion

        public EventRequest()
        {
            this.ListName = this.GetType().Name;
        }

        public override void MapToList(ListItem dest)
        {
            dest["SubmittedBy"] = SharePointHelper.GetFieldUser(SPContext, this.SubmittedBy);

            base.MapToList(dest);

            dest["EventName"] = EventName;
            dest["GuestsInvited"] = GuestsInvited;
            dest["EventLocation"] = EventLocation;
            dest["CrowdDescription"] = CrowdDescription;
            dest["ApproximateAttendees"] = ApproximateAttendees;
            dest["IsOpenToMedia"] = IsOpenToMedia;
            dest["EventStartDate"] = EventStartDate;
            dest["EventEndDate"] = EventEndDate;
            dest["EventContactName"] = EventContactName;
            dest["EventContactPhone"] = EventContactPhone;
            dest["IndividualExtendingInvite"] = IndividualExtendingInvite;
            dest["IsIndividualLobbyist"] = IsIndividualLobbyist;
            dest["OrgExtendingInvite"] = OrgExtendingInvite;
            dest["IsOrgLobbyist"] = IsOrgLobbyist;
            dest["TypeOfOrg"] = TypeOfOrg;
            dest["OrgHostingEvent"] = OrgHostingEvent;
            dest["IsHostLobbyist"] = IsHostLobbyist;
            dest["TypeOfHost"] = TypeOfHost;
            dest["IsFundraiser"] = IsFundraiser;
            dest["WhoIsPaying"] = WhoIsPaying;
            dest["FairMarketValue"] = FairMarketValue;
            dest["RequiresTravel"] = RequiresTravel;
            dest["InternationalTravel"] = InternationalTravel;
            dest["ContactNumber"] = ContactNumber;
            dest["ContactEmail"] = ContactEmail;
            dest["ContactComponent"] = ContactComponent;
            dest["AdditionalInformation"] = AdditionalInformation;
            dest["GuidanceGiven"] = GuidanceGiven;
            dest["AssignedTo"] = AssignedTo;
            dest["Status"] = Status;
            dest["ClosedReason"] = ClosedReason;
            dest["ClosedBy"] = ClosedBy;
            dest["ClosedDate"] = ClosedDate;
        }

        public string RunBusinessRules(UserInfo appUser)
        {
            this.Title = string.IsNullOrEmpty(this.EventName) ? "New Event" : EventName;

            return "";
        }

        public override void MapFromList(ListItem item, bool includeChildren = false)
        {
            base.MapFromList(item);

            SubmittedBy = ((FieldUserValue)item["SubmittedBy"]).LookupValue;
            EventName = SharePointHelper.ToStringNullSafe(item["EventName"]);
            GuestsInvited = SharePointHelper.ToStringNullSafe(item["GuestsInvited"]);
            EventLocation = SharePointHelper.ToStringNullSafe(item["EventLocation"]);
            CrowdDescription = SharePointHelper.ToStringNullSafe(item["CrowdDescription"]);
            ApproximateAttendees = Convert.ToInt32(item["ApproximateAttendees"]);
            IsOpenToMedia = SharePointHelper.ToStringNullSafe(item["IsOpenToMedia"]);
            EventStartDate = SharePointHelper.ToDateTimeNullIfMin(Convert.ToDateTime(item["EventStartDate"]));
            EventEndDate = SharePointHelper.ToDateTimeNullIfMin(Convert.ToDateTime(item["EventEndDate"]));
            EventContactName = SharePointHelper.ToStringNullSafe(item["EventContactName"]);
            EventContactPhone = SharePointHelper.ToStringNullSafe(item["EventContactPhone"]);
            IndividualExtendingInvite = SharePointHelper.ToStringNullSafe(item["IndividualExtendingInvite"]);
            IsIndividualLobbyist = SharePointHelper.ToStringNullSafe(item["IsIndividualLobbyist"]);
            OrgExtendingInvite = SharePointHelper.ToStringNullSafe(item["OrgExtendingInvite"]);
            IsOrgLobbyist = SharePointHelper.ToStringNullSafe(item["IsOrgLobbyist"]);
            TypeOfOrg = Convert.ToInt32(item["TypeOfOrg"]);
            OrgHostingEvent = SharePointHelper.ToStringNullSafe(item["OrgHostingEvent"]);
            IsHostLobbyist = SharePointHelper.ToStringNullSafe(item["IsHostLobbyist"]);
            TypeOfHost = Convert.ToInt32(item["TypeOfHost"]);
            IsFundraiser = SharePointHelper.ToStringNullSafe(item["IsFundraiser"]);
            WhoIsPaying = SharePointHelper.ToStringNullSafe(item["WhoIsPaying"]);
            FairMarketValue = Convert.ToDecimal(item["FairMarketValue"]);
            RequiresTravel = SharePointHelper.ToStringNullSafe(item["RequiresTravel"]);
            InternationalTravel = SharePointHelper.ToStringNullSafe(item["InternationalTravel"]);
            ContactNumber = SharePointHelper.ToStringNullSafe(item["ContactNumber"]);
            ContactEmail = SharePointHelper.ToStringNullSafe(item["ContactEmail"]);
            ContactComponent = SharePointHelper.ToStringNullSafe(item["ContactComponent"]);
            AdditionalInformation = SharePointHelper.ToStringNullSafe(item["AdditionalInformation"]);
            GuidanceGiven = SharePointHelper.ToStringNullSafe(item["GuidanceGiven"]);
            AssignedTo = SharePointHelper.ToStringNullSafe(item["AssignedTo"]);
            Status = SharePointHelper.ToStringNullSafe(item["Status"]);
            ClosedReason = SharePointHelper.ToStringNullSafe(item["ClosedReason"]);
            ClosedBy = SharePointHelper.ToStringNullSafe(item["ClosedBy"]);
            ClosedDate = SharePointHelper.ToDateTimeNullIfMin(Convert.ToDateTime(item["ClosedDate"]));

            this.Attendees = new List<Attendee>();
        }

        public static Dictionary<string, string> GetAppSettings(Dictionary<string, string> dict)
        {
            return dict;
        }

        public static Dictionary<string, string> GetAppEmailFieldsDef(Dictionary<string, string> dict)
        {
            return dict;
        }
    }
}