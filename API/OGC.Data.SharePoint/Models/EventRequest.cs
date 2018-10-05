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
        public bool GuestsInvited { get; set; }
        public DateTime? EventStartDate { get; set; }
        public DateTime? EventEndDate { get; set; }
        public string EventContactName { get; set; }
        public string EventContactPhone { get; set; }
        public string IndividualExtendingInvite { get; set; }
        public bool IsIndividualLobbyist { get; set; }
        public string OrgExtendingInvite { get; set; }
        public bool IsOrgLobbyist { get; set; }
        public int TypeOfOrg { get; set; }
        public string OrgOther { get; set; }
        public string OrgHostingEvent { get; set; }
        public bool IsHostLobbyist { get; set; }
        public int TypeOfHost { get; set; }
        public string HostOther { get; set; }
        public bool IsFundraiser { get; set; }
        public string WhoIsPaying { get; set; }
        public string FairMarketValue { get; set; }
        public bool RequiresTravel { get; set; }
        public bool InternationalTravel { get; set; }
        public string AdditionalInformation { get; set; }
        public string EventLocation { get; set; }
        public string CrowdDescription { get; set; }
        public string ApproximateAttendees{ get; set; }
        public bool IsOpenToMedia { get; set; }
        public string GuidanceGiven { get; set; }
        public string AssignedTo { get; set; }
        public string AssignedToUpn { get; set; }
        public string Status { get; set; }
        public string ClosedReason { get; set; }
        public string ClosedBy { get; set; }
        public DateTime? ClosedDate { get; set; }
        public string ContactNumber { get; set; }

        public string ContactEmail { get; set; }
        public string ContactComponent { get; set; }

        public List<Attendee> Attendees { get; set; }

        public List<Attachment> Attachments { get; set; }

        public List<SelectList> Reviewers { get; set; }

        public string AttachmentGuid { get; set; }

        public string Submitter { get; set; }
        #endregion

        private List<Notifications> _pendingEmails;

        public EventRequest()
        {
            this.ListName = this.GetType().Name;
            _pendingEmails = new List<Notifications>();
        }

        #region Mapping
        public override void MapToList(ListItem dest)
        {
            if (!string.IsNullOrEmpty(SubmittedBy))
            {
                dest["SubmittedBy"] = SharePointHelper.GetFieldUser(SubmittedBy);
            }

            if (!string.IsNullOrEmpty(AssignedToUpn))
            {
                dest["AssignedTo"] = SharePointHelper.GetFieldUser(AssignedToUpn);
                dest["AssignedToUpn"] = AssignedToUpn;
            }

            Title = EventName + ((EventStartDate != null) ? " (" + EventStartDate.Value.Year.ToString() + ")" : "");

            base.MapToList(dest);

            dest["EventName"] = EventName;
            dest["GuestsInvited"] = GuestsInvited;
            dest["EventLocation"] = EventLocation;
            dest["CrowdDescription"] = CrowdDescription;
            dest["ApproximateAttendees"] = ApproximateAttendees;
            dest["IsOpenToMedia"] = IsOpenToMedia;
            dest["EventStartDate"] = SharePointHelper.ToDateTimeNullIfMin(EventStartDate);
            dest["EventEndDate"] = SharePointHelper.ToDateTimeNullIfMin(EventEndDate);
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
            dest["Status"] = Status;
            dest["ClosedReason"] = ClosedReason;
            dest["ClosedBy"] = ClosedBy;
            dest["ClosedDate"] = SharePointHelper.ToDateTimeNullIfMin(ClosedDate);
            dest["AttachmentGuid"] = AttachmentGuid;
            dest["Submitter"] = Submitter;
            dest["OrgOther"] = OrgOther;
            dest["HostOther"] = HostOther;
        }

        public override void MapFromList(ListItem item, bool includeChildren = false)
        {
            base.MapFromList(item);

            if (item["SubmittedBy"] != null)
                SubmittedBy =  ((FieldUserValue)item["SubmittedBy"]).LookupValue;

            Submitter = SharePointHelper.ToStringNullSafe(item["Submitter"]);
            EventName = SharePointHelper.ToStringNullSafe(item["EventName"]);
            GuestsInvited = SharePointHelper.ToStringNullSafe(item["GuestsInvited"]) == "True";
            EventLocation = SharePointHelper.ToStringNullSafe(item["EventLocation"]);
            CrowdDescription = SharePointHelper.ToStringNullSafe(item["CrowdDescription"]);
            ApproximateAttendees = SharePointHelper.ToStringNullSafe(item["ApproximateAttendees"]);
            IsOpenToMedia = SharePointHelper.ToStringNullSafe(item["IsOpenToMedia"]) == "True";
            EventStartDate = SharePointHelper.ToDateTimeNullIfMin(Convert.ToDateTime(item["EventStartDate"]));
            EventEndDate = SharePointHelper.ToDateTimeNullIfMin(Convert.ToDateTime(item["EventEndDate"]));
            EventContactName = SharePointHelper.ToStringNullSafe(item["EventContactName"]);
            EventContactPhone = SharePointHelper.ToStringNullSafe(item["EventContactPhone"]);
            IndividualExtendingInvite = SharePointHelper.ToStringNullSafe(item["IndividualExtendingInvite"]);
            IsIndividualLobbyist = SharePointHelper.ToStringNullSafe(item["IsIndividualLobbyist"]) == "True";
            OrgExtendingInvite = SharePointHelper.ToStringNullSafe(item["OrgExtendingInvite"]);
            IsOrgLobbyist = SharePointHelper.ToStringNullSafe(item["IsOrgLobbyist"]) == "True";
            TypeOfOrg = Convert.ToInt32(item["TypeOfOrg"]);
            OrgHostingEvent = SharePointHelper.ToStringNullSafe(item["OrgHostingEvent"]);
            IsHostLobbyist = SharePointHelper.ToStringNullSafe(item["IsHostLobbyist"]) == "True";
            TypeOfHost = Convert.ToInt32(item["TypeOfHost"]);
            IsFundraiser = SharePointHelper.ToStringNullSafe(item["IsFundraiser"]) == "True";
            WhoIsPaying = SharePointHelper.ToStringNullSafe(item["WhoIsPaying"]);
            FairMarketValue = SharePointHelper.ToStringNullSafe(item["FairMarketValue"]);
            RequiresTravel = SharePointHelper.ToStringNullSafe(item["RequiresTravel"]) == "True";
            InternationalTravel = SharePointHelper.ToStringNullSafe(item["InternationalTravel"]) == "True";
            ContactNumber = SharePointHelper.ToStringNullSafe(item["ContactNumber"]);
            ContactEmail = SharePointHelper.ToStringNullSafe(item["ContactEmail"]);
            ContactComponent = SharePointHelper.ToStringNullSafe(item["ContactComponent"]);
            AdditionalInformation = SharePointHelper.ToStringNullSafe(item["AdditionalInformation"]);
            GuidanceGiven = SharePointHelper.ToStringNullSafe(item["GuidanceGiven"]);

            if (item["AssignedTo"] != null)
            {
                AssignedTo = ((FieldUserValue)item["AssignedTo"]).LookupValue;
            }

            AssignedToUpn = SharePointHelper.ToStringNullSafe(item["AssignedToUpn"]);

            Status = SharePointHelper.ToStringNullSafe(item["Status"]);
            ClosedReason = SharePointHelper.ToStringNullSafe(item["ClosedReason"]);
            ClosedBy = SharePointHelper.ToStringNullSafe(item["ClosedBy"]);
            ClosedDate = SharePointHelper.ToDateTimeNullIfMin(Convert.ToDateTime(item["ClosedDate"]));
            AttachmentGuid = SharePointHelper.ToStringNullSafe(item["AttachmentGuid"]);
            Submitter = SharePointHelper.ToStringNullSafe(item["Submitter"]);
            OrgOther = SharePointHelper.ToStringNullSafe(item["OrgOther"]);
            HostOther = SharePointHelper.ToStringNullSafe(item["HostOther"]);

            if (includeChildren)
            {
                this.Attendees = Attendee.GetAllBy(ListName + "Id", Id);
                this.Attachments = Attachment.GetAllBy(ListName + "Id", Id);
                //this.Reviewers = GetEventReviewers();
            }
            else
            {
                this.Attachments = new List<Attachment>();
                this.Attendees = new List<Attendee>();
            }
        }
        #endregion

        public void UpdateAttachments(int id)
        {
            var existingAttachments = Attachment.GetAllBy("AttachmentGuid", AttachmentGuid);
            
            foreach(Attachment att in Attachments)
            {
                var exAtt = existingAttachments.Where(x => x.FileName == att.FileName).FirstOrDefault();

                if (exAtt != null)
                {
                    exAtt.EventRequestId = id;
                    exAtt.Save();
                    existingAttachments.Remove(exAtt);
                }
            }

            foreach (Attachment att in existingAttachments)
            {
                att.Delete();
            }
        }

        public void SaveAttendees(int id)
        {
            foreach (Attendee att in Attendees)
            {
                if (att.Employee != null && !string.IsNullOrEmpty(att.Employee.Upn))
                {
                    if (att.EventRequestId != id)
                        att.EventRequestId = id;

                    att.Save();
                }
            }
        }

        public void ResendAssignedToEmail(UserInfo who)
        {
            if (!string.IsNullOrEmpty(this.AssignedTo))
            {
                var emailData = this.GetEmailData(who);
                _pendingEmails.Add(EmailHelper.GetEmail(NotificationTemplates.NotificationTypes.EVENT_REQUEST_ASSIGNED, who, emailData));
            }

            ProcessEmails();
        }

        public string RunBusinessRules(UserInfo appUser, UserInfo submitter, EventRequest oldItem)
        {
            var result = "";
            this.Title = string.IsNullOrEmpty(this.EventName) ? "New Event" : EventName;

            if (string.IsNullOrEmpty(this.Status))
                this.Status = Constants.EventRequestStatus.DRAFT;


            if (this.Status == Constants.EventRequestStatus.UNASSIGNED && (oldItem == null || oldItem.Status == Constants.EventRequestStatus.DRAFT))
            {
                this.SubmittedBy = appUser.Upn;
                this.Submitter = appUser.DisplayName;

                // EventRequest is being submitted, trigger email notification
                var emailData = this.GetEmailData(submitter);

                _pendingEmails.Add(EmailHelper.GetEmail(NotificationTemplates.NotificationTypes.EVENT_REQUEST_CONFIRMATION, submitter, emailData));
                _pendingEmails.Add(EmailHelper.GetEmail(NotificationTemplates.NotificationTypes.EVENT_REQUEST_SUBMITTED, submitter, emailData));
            }

            if (!string.IsNullOrEmpty(this.AssignedToUpn) && oldItem != null && oldItem.AssignedToUpn != this.AssignedToUpn)
            {
                // Attempting to assign to a new reviewer, if admin or reviewer, accept assignment, set status to Open, else return error
                if (appUser.IsAdmin || appUser.IsReviewer)
                {
                    this.AssignedTo = SharePointHelper.EnsureUser(this.AssignedToUpn).Title;
                    this.Status = Constants.EventRequestStatus.OPEN;

                    var emailData = this.GetEmailData(submitter);
                    _pendingEmails.Add(EmailHelper.GetEmail(NotificationTemplates.NotificationTypes.EVENT_REQUEST_ASSIGNED, submitter, emailData));
                }
                else
                    result = "Unable to assign event request, you do not have permission to perform this action.";
            }

            if (oldItem != null && !oldItem.Status.Contains(Constants.EventRequestStatus.CLOSED) && this.Status.Contains(Constants.EventRequestStatus.CLOSED))
            {
                // Attempting to close, only let admin or reviewer close, otherwise return error
                if (appUser.IsAdmin || appUser.IsReviewer)
                    this.ClosedDate = DateTime.Now;
                else
                    result = "Unable to close event request, you do not have permission to perform this action.";
            }
                
            return result;
        }

        #region Emails
        public void AddEmail(Notifications item)
        {
            _pendingEmails.Add(item);
        }

        public void ProcessEmails()
        {
            foreach (Notifications notification in _pendingEmails)
                EmailHelper.AddEmail(notification);
        }

        public Dictionary<string, string> GetEmailData(UserInfo user)
        {
            var dict = new Dictionary<string, string>();

            dict = Settings.GetAppSettings(dict);

            dict.Add("User", user.DisplayName);
            dict.Add("Email", user.Email);

            dict.Add("EventName", this.EventName);
            dict.Add("EventStartDate", this.EventStartDate.Value.ToShortDateString());
            dict.Add("EventEndDate", this.EventEndDate.Value.ToShortDateString());
            dict.Add("EventLocation", this.EventLocation);
            dict.Add("ApproximateAttendees", this.ApproximateAttendees);
            dict.Add("CrowdDescription", this.CrowdDescription);
            dict.Add("IsFundraiser", this.IsFundraiser ? "Yes" : "No");
            dict.Add("IsOpenToMedia", this.IsOpenToMedia ? "Yes" : "No");
            dict.Add("RequiresTravel", this.RequiresTravel ? "Yes" : "No");
            dict.Add("InternationalTravel", this.InternationalTravel ? "Yes" : "No");

            var travelForms = "";
            var invitations = "";
            var otherAttachments = "";

            foreach (Attachment att in this.Attachments)
            {
                if (att.TypeOfAttachment == Constants.AttachmentType.TRAVEL_FORMS)
                    travelForms += att.FileName + ", ";
                if (att.TypeOfAttachment == Constants.AttachmentType.INVITATIONS)
                    invitations += att.FileName + ", ";
                if (att.TypeOfAttachment == Constants.AttachmentType.OTHER)
                    otherAttachments += att.FileName + ", ";
            }

            travelForms = travelForms.TrimEnd(new char[] { ',', ' ' });
            invitations = invitations.TrimEnd(new char[] { ',', ' ' });
            otherAttachments = otherAttachments.TrimEnd(new char[] { ',', ' ' });

            dict.Add("TravelForms", travelForms);

            var attendeeData = "";
            var attendeeString = "";

            foreach (Attendee att in this.Attendees)
            {
                var data = "​​​​Name of Attendee: {0}<br />";

                data += "Political or Career Employee: {1}<br />";
                data += "Attending Capacity: {2}<br />";
                data += "Attendee Giving Remarks: {3}<br />";
                data += "Remarks: {4}<br />";
                data += "Reason for Attending: {5}";

                
                data = string.Format(data, new string[] { att.Employee == null ? "" : att.Employee.DisplayName, att.EmployeeType, att.Capacity, att.IsGivingRemarks ? "Yes" : "No", att.Remarks, att.ReasonForAttending});

                attendeeData += data + "<br /><br />";
                attendeeString += att.Employee.DisplayName + ", ";
            }

            if (attendeeData.Length > 12) 
                attendeeData = attendeeData.Substring(0, attendeeData.Length - 12);

            attendeeString = attendeeString.TrimEnd(',', ' ');

            dict.Add("Attendees", attendeeData);
            dict.Add("AttendeeString", attendeeString);
            dict.Add("GuestsInvited", this.GuestsInvited ? "Yes" : "No");
            dict.Add("IndividualExtendingInvite", this.IndividualExtendingInvite);
            dict.Add("IsIndividualLobbyist", this.IsIndividualLobbyist ? "Yes" : "No");
            dict.Add("OrgExtendingInvite", this.OrgExtendingInvite);
            dict.Add("IsOrgLobbyist", this.IsOrgLobbyist ? "Yes" : "No");
            dict.Add("TypeOfOrg", GetOrgDescription(this.TypeOfOrg));
            dict.Add("OrgHostingEvent", this.OrgHostingEvent);
            dict.Add("IsHostLobbyist", this.IsHostLobbyist ? "Yes" : "No");
            dict.Add("TypeOfHost", GetOrgDescription(this.TypeOfHost));

            dict.Add("Invitations", invitations);

            dict.Add("EventContactName", this.EventContactName);
            dict.Add("EventContactPhone", this.EventContactPhone);
            dict.Add("FairMarketValue", this.FairMarketValue);
            dict.Add("WhoIsPaying", this.WhoIsPaying);
            dict.Add("AdditionalInformation", this.AdditionalInformation);
            dict.Add("AdditionalDocuments", otherAttachments);

            dict.Add("Submitter", Submitter);

            dict.Add("ContactEmail", this.ContactEmail);
            dict.Add("ContactNumber", this.ContactNumber);
            dict.Add("ContactComponent", this.ContactComponent);

            dict.Add("AssignedTo", this.AssignedTo);

            return dict;
        }

        private string GetOrgDescription(int typeOfOrg)
        {
            var desc = "";

            if ((typeOfOrg & 1) == 1)
                desc += "Non-Profit/501(c)(3), ";
            if ((typeOfOrg & 2) == 2)
                desc += "Media Org., ";
            if ((typeOfOrg & 4) == 4)
                desc += "Lobbying Org., ";
            if ((typeOfOrg & 8) == 8)
                desc += "USG Entity, ";
            if ((typeOfOrg & 16) == 16)
                desc += "Foreign Gov., ";

            desc = desc.TrimEnd(new char[] { ',', ' ' });

            return desc;
        }

        public static Dictionary<string, string> GetEmailFieldsDef(Dictionary<string, string> dict)
        {
            dict.Add("[User]", "The Display Name of the filer.");
            dict.Add("[Email]", "The filer's email address.");
             
            dict.Add("[EventName]", "The name of the event.");
            dict.Add("[EventStartDate]", "The start date of the event.");
            dict.Add("[EventEndDate]", "The end date of the event.");
            dict.Add("[EventLocation]", "The location of the event.");
            dict.Add("[ApproximateAttendees]", "The approximate number of attendees for the event.");
            dict.Add("[CrowdDescription]", "The description of the crowd at the event.");
            dict.Add("[IsFundraiser]", "Is the event a fundraiser?");
            dict.Add("[IsOpenToMedia]", "Is the event open to the media?");
            dict.Add("[RequiresTravel]", "Does the event require travel?");
            dict.Add("[InternationalTravel]", "Does the event require international travel?");
            dict.Add("[TravelForms]", "The attached travel forms.");

            dict.Add("[Attendees]", "Attendee information for the event.");
            dict.Add("[AttendeeString]", "Comma delimited list of attendees.");

            dict.Add("[GuestsInvited]", "Are guests invited?");
            dict.Add("[IndividualExtendingInvite]", "Individual extending the invite.");
            dict.Add("[IndividualLobbyist]", "Is this person a lobbyist?");
            dict.Add("[OrgExtendingInvite]", "Organization extending the invite.");
            dict.Add("[IsOrgLobbyist]", "Is this organization a lobbyist?");
            dict.Add("[TypeOfOrg]", "Type of organization.");
            dict.Add("[OrgHostingEvent]", "Organization hosting the event.");
            dict.Add("[IsHostLobbyist]", "Is the host a lobbyist.");
            dict.Add("[TypeOfHost]", "The type of oranization of the host.");
            dict.Add("[Invitations]", "The attached invitations.");

            dict.Add("[EventContactName]", "Person to contact at the event.");
            dict.Add("[EventContactPhone]", "The event contact's phone number.");
            dict.Add("[FairMarketValue]", "The fair market value of the event.");
            dict.Add("[WhoIsPaying]", "The person paying for the event.");
            dict.Add("[AdditionalInformation]", "Additional information submitted.");
            dict.Add("[AdditionalDocuments]", "Any additional attachments.");

            dict.Add("[Submitter]", "The submitter of the event request.");
            dict.Add("[ContactEmail]", "The submitter's email.");
            dict.Add("[ContactNumber]", "The submitter's phone number.");
            dict.Add("[ContactComponent]", "The component of the submitter.");

            dict.Add("[AssignedTo]", "Who the event request was assigned to.");

            return dict;
        }
        #endregion
    }
}