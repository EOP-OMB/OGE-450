using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.SharePoint.Client;
using OMB.SharePoint.Infrastructure;

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
            this.ListName = this.GetType().Name;
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