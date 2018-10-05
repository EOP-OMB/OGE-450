using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.SharePoint.Client;
using OMB.SharePoint.Infrastructure;
using System.Text.RegularExpressions;

namespace OGC.Data.SharePoint.Models
{
    public class EthicsClearance : SPListBase<EthicsClearance>, ISPList
    {
        #region properties
        public string OMBEthicsFormId { get; set; }
        public string Submitter { get; set; }
        public string Attendees { get; set; }
        public string AttendingCapacity { get; set; }
        public string EmployeeType { get; set; }
        public string EventName { get; set; }
        public string EventPurpose { get; set; }
        public string GuestInvited { get; set; }
        public string EventLocation { get; set; }
        public string EventCrowd { get; set; }
        public string EmployeesTotal { get; set; }
        public string OpenToMedia { get; set; }
        public DateTime? EventStartDate { get; set; }
        public DateTime? EventEndDate { get; set; }
        public string EventContactName { get; set; }
        public string EventContactEmail { get; set; }
        public string EventContactPhone { get; set; }
        public string AttachedInvitation { get; set; }
        public string IndividualExtendingInvite { get; set; }
        public string OrganizationExtendingInvite { get; set; }
        public string OrganizerHostingEvent { get; set; }
        public string Speaker { get; set; }
        public string PlannedRemarks { get; set; }
        public string Fundraiser { get; set; }
        public string ReasonForAttending { get; set; }
        public string WhoIsPaying { get; set; }
        public string FairMarketValue { get; set; }
        public string RequiresTravel { get; set; }
        public string InternationalTravel { get; set; }
        public string AttachedTravelForm { get; set; }
        public string ContactNumber { get; set; }
        public string ContactEmail { get; set; }
        public string ContactSupervisor { get; set; }
        public string SupervisorApproval { get; set; }
        public string ContactComponent { get; set; }
        public string AdditionalInformation { get; set; }
        public string GuidanceGiven { get; set; }
        public string AssignedFlag { get; set; }
        public string AssignedToUserOBJECT { get; set; }
        public string AssignedTo { get; set; }
        public string AssignedToEmailNotificationSent { get; set; }
        public string EmailNotificationSent { get; set; }
        public string Closed { get; set; }
        public string ClosedReason { get; set; }
        public string ClosedBy { get; set; }
        public DateTime? ClosedDate { get; set; }
        public string ClosedEmailNotificationSent { get; set; }
        #endregion

        public EthicsClearance()
        {
            this.ListName = "OMB Ethics Submitted Forms";
        }

        public override void MapToList(ListItem dest)
        {
            base.MapToList(dest);

            dest["OMB_x0020_Ethics_x0020_Form_x0020_ID"] = OMBEthicsFormId;
            dest["Submitter"] = Submitter;
            dest["Attendees"] = Attendees;
            dest["Attending_x0020_Capacity"] = AttendingCapacity;
            dest["Employee_x0020_Type"] = EmployeeType;
            dest["Event_x0020_Name"] = EventName;
            dest["Event_x0020_Purpose"] = EventPurpose;
            dest["Guest_x0020_Invited"] = GuestInvited;
            dest["Event_x0020_Location"] = EventLocation;
            dest["Event_x0020_Crowd"] = EventCrowd;
            dest["Employees_x0020_Total"] = EmployeesTotal;
            dest["Open_x0020_To_x0020_Media"] = OpenToMedia;
            dest["Event_x0020_Start_x0020_Date"] = EventStartDate;
            dest["Event_x0020_End_x0020_Date"] = EventEndDate;
            dest["Event_x0020_Contact_x0020_Name"] = EventContactName;
            dest["Event_x0020_Contact_x0020_Email"] = EventContactEmail;
            dest["Event_x0020_Contact_x0020_Phone"] = EventContactPhone;
            dest["Attached_x0020_Invitation"] = AttachedInvitation;
            dest["Individual_x0020_Extending_x0020_Invite"] = IndividualExtendingInvite;
            dest["Organization_x0020_Extending_x0020_Invite"] = OrganizationExtendingInvite;
            dest["Organizer_x0020_Hosting_x0020_Event"] = OrganizerHostingEvent;
            dest["Speaker"] = Speaker;
            dest["Planned_x0020_Remarks"] = PlannedRemarks;
            dest["Fundraiser"] = Fundraiser;
            dest["Reason_x0020_For_x0020_Attending"] = ReasonForAttending;
            dest["Who_x0020_Is_x0020_Paying"] = WhoIsPaying;
            dest["Fair_x0020_Market_x0020_Value"] = FairMarketValue;
            dest["Requires_x0020_Travel"] = RequiresTravel;
            dest["International_x0020_Travel"] = InternationalTravel;
            dest["Attached_x0020_Travel_x0020_Form"] = AttachedTravelForm;
            dest["Contact_x0020_Number"] = ContactNumber;
            dest["Contact_x0020_Email"] = ContactEmail;
            dest["Contact_x0020_Supervisor"] = ContactSupervisor;
            dest["Supervisor_x0020_Approval"] = SupervisorApproval;
            dest["Contact_x0020_Component"] = ContactComponent;
            dest["Additional_x0020_Information"] = AdditionalInformation;
            dest["Guidance_x0020_Given"] = GuidanceGiven;
            dest["Assigned_x0020_Flag"] = AssignedFlag;
            dest["AssignedToUserOBJECT"] = AssignedToUserOBJECT;
            dest["Assigned_x0020_To"] = AssignedTo;
            dest["Assigned_x0020_To_x0020_Email_x0020_Notification_x0020_Sent"] = AssignedToEmailNotificationSent;
            dest["Email_x0020_Notification_x0020_Sent"] = EmailNotificationSent;
            dest["Closed"] = Closed;
            dest["Closed_x0020_Reason"] = ClosedReason;
            dest["Closed_x0020_By"] = ClosedBy;
            dest["Closed_x0020_Date"] = ClosedDate;
            dest["Closed_x0020_Email_x0020_Notification_x0020_Sent"] = ClosedEmailNotificationSent;
    }

        public override void MapFromList(ListItem item, bool includeChildren = false)
        {
            base.MapFromList(item);

            OMBEthicsFormId = SharePointHelper.ToStringNullSafe(item["OMB_x0020_Ethics_x0020_Form_x002"]);
            Submitter = SharePointHelper.ToStringNullSafe(item["Submitter"]);
            Attendees = SharePointHelper.ToStringNullSafe(item["Attendee"]);
            AttendingCapacity = SharePointHelper.ToStringNullSafe(item["Attending_x0020_Capacity"]);
            EmployeeType = SharePointHelper.ToStringNullSafe(item["Employee_x0020_Type"]);
            EventName = SharePointHelper.ToStringNullSafe(item["Event_x0020_Name"]);
            EventPurpose = SharePointHelper.ToStringNullSafe(item["Event_x0020_Purpose"]);
            GuestInvited = SharePointHelper.ToStringNullSafe(item["Guest_x0020_Invited"]);
            EventLocation = SharePointHelper.ToStringNullSafe(item["Event_x0020_Location"]);
            EventCrowd = SharePointHelper.ToStringNullSafe(item["Event_x0020_Crowd"]);
            EmployeesTotal = SharePointHelper.ToStringNullSafe(item["Employees_x0020_Total"]);
            OpenToMedia = SharePointHelper.ToStringNullSafe(item["Open_x0020_Media_x0020_Present"]);
            EventStartDate = SharePointHelper.ToDateTimeNullIfMin(Convert.ToDateTime(item["Event_x0020_Date"]));
            EventEndDate = SharePointHelper.ToDateTimeNullIfMin(Convert.ToDateTime(item["Event_x0020_End_x0020_Date"]));
            EventContactName = SharePointHelper.ToStringNullSafe(item["Event_x0020_Contact_x0020_Name"]);
            EventContactEmail = SharePointHelper.ToStringNullSafe(item["Event_x0020_Contact_x0020_Email"]);
            EventContactPhone = SharePointHelper.ToStringNullSafe(item["Event_x0020_Contact_x0020_Phone"]);
            AttachedInvitation = SharePointHelper.ToStringNullSafe(item["Attached_x0020_Invitation"]);
            IndividualExtendingInvite = SharePointHelper.ToStringNullSafe(item["Individual_x0020_Extending_x0020"]);
            OrganizationExtendingInvite = SharePointHelper.ToStringNullSafe(item["Organization_x0020_Extending_x00"]);
            OrganizerHostingEvent = SharePointHelper.ToStringNullSafe(item["Organizer_x0020_Hosting_x0020_Ev"]);
            Speaker = SharePointHelper.ToStringNullSafe(item["Speaker"]);
            PlannedRemarks = SharePointHelper.ToStringNullSafe(item["Planned_x0020_Remarks"]);
            Fundraiser = SharePointHelper.ToStringNullSafe(item["Fundraiser"]);
            ReasonForAttending = SharePointHelper.ToStringNullSafe(item["Reason_x0020_For_x0020_Attending"]);
            WhoIsPaying = SharePointHelper.ToStringNullSafe(item["Who_x0020_Is_x0020_Paying"]);
            FairMarketValue = SharePointHelper.ToStringNullSafe(item["Fair_x0020_Market_x0020_Value"]);
            RequiresTravel = SharePointHelper.ToStringNullSafe(item["Requires_x0020_Travel"]);
            InternationalTravel = SharePointHelper.ToStringNullSafe(item["International_x0020_Travel"]);
            AttachedTravelForm = SharePointHelper.ToStringNullSafe(item["Attached_x0020_Travel_x0020_Form"]);
            ContactNumber = SharePointHelper.ToStringNullSafe(item["Contact_x0020_Number"]);
            ContactEmail = SharePointHelper.ToStringNullSafe(item["Contact_x0020_Email"]);
            ContactSupervisor = SharePointHelper.ToStringNullSafe(item["Contact_x0020_Supervisor"]);
            SupervisorApproval = SharePointHelper.ToStringNullSafe(item["Supervisor_x0020_Approval"]);
            ContactComponent = SharePointHelper.ToStringNullSafe(item["Contact_x0020_Component"]);
            AdditionalInformation = SharePointHelper.ToStringNullSafe(item["Additional_x0020_Information"]);
            GuidanceGiven = SharePointHelper.ToStringNullSafe(item["Guidance_x0020_Given"]);
            AssignedFlag = SharePointHelper.ToStringNullSafe(item["Assigned_x0020_Flag"]);
            AssignedToUserOBJECT = SharePointHelper.ToStringNullSafe(item["AssignedToUserOBJECT"]);
            AssignedTo = SharePointHelper.ToStringNullSafe(item["Assigned_x0020_To"]);
            AssignedToEmailNotificationSent = SharePointHelper.ToStringNullSafe(item["Assigned_x0020_To_x0020_Email_x0"]);
            EmailNotificationSent = SharePointHelper.ToStringNullSafe(item["Email_x0020_Notification_x0020_S"]);
            Closed = SharePointHelper.ToStringNullSafe(item["Closed"]);
            ClosedReason = SharePointHelper.ToStringNullSafe(item["Closed_x0020_Reason"]);
            ClosedBy = SharePointHelper.ToStringNullSafe(item["Closed_x0020_By"]);
            ClosedDate = SharePointHelper.ToDateTimeNullIfMin(Convert.ToDateTime(item["Closed_x0020_Date"]));
            ClosedEmailNotificationSent = SharePointHelper.ToStringNullSafe(item["Closed_x0020_Email_x0020_Notific"]);
        }


        public static List<EventRequest> Migrate()
        {
            var oldData = EthicsClearance.GetAll();

            foreach (EthicsClearance ec in oldData)
            {
                try
                {
                    var request = MigrateEthicsClearance(ec);

                    MigrateAttendees(request.Id, ec);

                    MigrateAttachments(request.Id, ec);
                }
                catch (Exception ex)
                {
                    SharePointHelper.HandleException(ex, "admin", "EthicsClearance.Migrate");
                }
            }

            return EventRequest.GetAll();
        }

        private static EventRequest MigrateEthicsClearance(EthicsClearance ec)
        {
            var request = new EventRequest();

            request.Title = ec.EventName + " (" + ec.EventStartDate.Value.Year + ")";
            request.SubmittedBy = UserInfo.GetUserByName(ec.Submitter);
            request.EventName = ec.EventName;
            request.GuestsInvited = ec.GuestInvited == "Yes";
            request.EventStartDate = ec.EventStartDate;
            request.EventEndDate = ec.EventEndDate;
            request.EventContactName = ec.EventContactName;

            var individualData = ec.IndividualExtendingInvite.Split('|');

            request.IndividualExtendingInvite = individualData.Length > 0 ? individualData[0] : "";
            request.IsIndividualLobbyist = individualData.Length > 1 ? (individualData[1] == "Yes") : false;

            var orgData = ec.OrganizationExtendingInvite.Split('|');
            var other = "";

            request.OrgExtendingInvite = orgData.Length > 0 ? orgData[0] : "";
            request.IsOrgLobbyist = orgData.Length > 1 ? (orgData[1] == "Yes") : false;
            request.TypeOfOrg = orgData.Length > 2 ? GetOrgType(orgData[2], ref other) : 0;
            request.OrgOther = other;

            var hostData = ec.OrganizerHostingEvent.Split('|');

            request.OrgHostingEvent = hostData.Length > 0 ? hostData[0] : "";
            request.IsHostLobbyist = hostData.Length > 1 ? (hostData[1] == "Yes") : false;
            other = "";
            request.TypeOfHost = hostData.Length > 2 ? GetOrgType(hostData[2], ref other) : 0;
            request.HostOther = other;

            request.IsFundraiser = ec.Fundraiser == "Yes";
            request.WhoIsPaying = ec.WhoIsPaying;
            request.FairMarketValue = ec.FairMarketValue;

            request.RequiresTravel = ec.RequiresTravel == "Yes";
            request.InternationalTravel = ec.InternationalTravel == "Yes";
            request.AdditionalInformation = ec.AdditionalInformation;

            request.EventLocation = ec.EventLocation;
            request.CrowdDescription = ec.EventCrowd;
            request.ApproximateAttendees = ec.EmployeesTotal;
            request.IsOpenToMedia = ec.OpenToMedia == "Yes";

            request.GuidanceGiven = ec.GuidanceGiven;
            request.AssignedToUpn = UserInfo.GetUserByName(ec.AssignedTo);

            if (ec.Closed == "Yes")
            {
                if (!string.IsNullOrEmpty(ec.GuidanceGiven))
                {
                    // if it's closed and guidance was given, mark as closed approved
                    request.Status = Constants.EventRequestStatus.APPROVED;
                }
                else if (ec.ClosedReason.ToLower().Contains("canceled") || ec.ClosedReason.ToLower().Contains("cancelled"))
                {
                    // if it's closed and a reason was given, mark as closed - other
                    request.Status = Constants.EventRequestStatus.CANCELED;
                }
                else if (ec.ClosedReason.ToLower().Contains("withdrew"))
                {
                    // if it's closed and a reason was given, mark as closed - other
                    request.Status = Constants.EventRequestStatus.WITHDRAWN;
                }
                else
                {
                    request.Status = Constants.EventRequestStatus.CLOSED;
                }
            }
            else if (!string.IsNullOrEmpty(request.AssignedToUpn))
                request.Status = Constants.EventRequestStatus.OPEN;
            else
                request.Status = Constants.EventRequestStatus.UNASSIGNED;


            request.ClosedReason = ec.ClosedReason;
            request.ClosedDate = ec.ClosedDate;

            request.ContactNumber = ec.ContactNumber;
            request.ContactEmail = ec.ContactEmail;
            request.ContactComponent = ec.ContactComponent;
            request.EventContactPhone = ec.EventContactPhone;

            request.AttachmentGuid = ec.OMBEthicsFormId;

            request.Submitter = ec.Submitter;

            return request.Save();
        }

        private static void MigrateAttachments(int id, EthicsClearance ec)
        {
            var oldAttachments = OMBEthicsAttachments.GetAllBy("Associated_x0020_Ethics_x0020_Form_x0020_ID", ec.OMBEthicsFormId);

            foreach (OMBEthicsAttachments att in oldAttachments)
            {
                try
                {
                    var attachmentWithContent = OMBEthicsAttachments.Get(att.Id);
                    var attachmentType = Constants.AttachmentType.OTHER;

                    if (ec.AttachedInvitation.Contains(attachmentWithContent.FileName))
                        attachmentType = Constants.AttachmentType.INVITATIONS;
                    else if (ec.AttachedTravelForm.Contains(attachmentWithContent.FileName))
                        attachmentType = Constants.AttachmentType.TRAVEL_FORMS;

                    var newAttachment = new Attachment();

                    newAttachment.TypeOfAttachment = attachmentType;
                    newAttachment.AttachmentGuid = ec.OMBEthicsFormId;
                    newAttachment.Content = attachmentWithContent.Content;
                    newAttachment.FileName = attachmentWithContent.FileName;
                    newAttachment.EventRequestId = id;
                    newAttachment.Title = attachmentWithContent.FileName;
                    newAttachment.Size = attachmentWithContent.Content.Length;

                    newAttachment.Create();
                }
                catch (Exception ex)
                {
                    SharePointHelper.HandleException(ex, "admin", "EthicsClearance.MigrateAttachments");
                }
            }
        }

        private static void MigrateAttendees(int id, EthicsClearance ec)
        {
            var attendees = ec.Attendees;

            if (attendees.Contains(","))
            {
                string[] employees = Regex.Matches(attendees, "([^,]*,[^,]*)(?:, |$)")
                  .Cast<Match>()
                  .Select(m => m.Groups[1].Value)
                  .ToArray();

                foreach (string emp in employees)
                    MigrateAttendee(id, emp, ec);
            }
            else
            {
                MigrateAttendee(id, attendees, ec);
            }
        }

        private static void MigrateAttendee(int id, string emp, EthicsClearance ec)
        {
            try
            { 
                var attendee = new Attendee();

                attendee.Employee = UserInfo.GetUserInfoByName(emp);
                attendee.Capacity = ec.AttendingCapacity;
                attendee.ReasonForAttending = ec.ReasonForAttending;
                attendee.IsGivingRemarks = ec.Speaker == "Yes";
                attendee.Remarks = ec.PlannedRemarks;
                attendee.EmployeeType = ec.EmployeeType;

                attendee.EventRequestId = id;
                attendee.Title = emp;

                attendee.Save();
            }
            catch (Exception ex)
            {
                SharePointHelper.HandleException(ex, "admin", "EthicsClearance.MigrateAttendee");
            }
        }

        private static int GetOrgType(string orgData, ref string other)
        {
            var orgs = orgData.Split(',');
            var value = 0;

            foreach (string org in orgs)
            {
                if (org == "Non-Profit/501c)(3)")
                    value += 1;
                if (org == "Media Organization")
                    value += 2;
                if (org == "Lobbying Organization")
                    value += 4;
                if (org == "USG Entity")
                    value += 8;
                if (org == "Foreign Government")
                    value += 16;

                if (org.Contains("Other"))
                {
                    var vals = org.Split(':');
                    other = vals.Length > 1 ? vals[1] : "";
                }
            }

            return 0;
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