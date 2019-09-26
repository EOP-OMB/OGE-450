using Microsoft.SharePoint.Client;
using OMB.SharePoint.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OGC.Data.SharePoint.Models
{
    public class OutsidePosition : SPListBase<OutsidePosition>, ISPList
    {
        public string Status{ get; set; }
        public string EmployeesName { get; set; }
        public string EmployeeUpn { get; set; }
        public string Grade { get; set; }
        public string Branch { get; set; }
        public string Division { get; set; }
        public string Supervisor { get; set; }
        public string Duties { get; set; }
        public string ProposedActivity { get; set; }
        public string PerformedFor { get; set; }
        public string TypeOfWork { get; set; }
        public string NumberOfHours { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate{ get; set; }
        public bool IsPaid{ get; set; }
        public bool IsProfessionalService { get; set; }
        public bool AffirmDependantInformation { get; set; }
        public bool AffirmOfficialDuty { get; set; }
        public bool IsNonProfitEtc { get; set; }
        public DateTime? ClosedDate { get; private set; }

        private List<Notifications> _pendingEmails;

        public OutsidePosition()
        {
            this.ListName = this.GetType().Name;
            _pendingEmails = new List<Notifications>();
        }

        #region Mapping
        public override void MapToList(ListItem dest)
        {
            if (!string.IsNullOrEmpty(EmployeeUpn))
            {
                dest["EmployeeUpn"] = SharePointHelper.GetFieldUser(EmployeeUpn);
            }

            base.MapToList(dest);

            dest["Status"] = Status;
            dest["EmployeesName"] = EmployeesName;
            dest["Grade"] = Grade;
            dest["Branch"] = Branch;
            dest["Division"] = Division;
            dest["Supervisor"] = Supervisor;
            dest["Duties"] = Duties;
            dest["ProposedActivity"] = ProposedActivity;
            dest["PerformedFor"] = PerformedFor;
            dest["TypeOfWork"] = TypeOfWork;
            dest["NumberOfHours"] = NumberOfHours;

            dest["StartDate"] = SharePointHelper.ToDateTimeNullIfMin(StartDate);
            dest["EndDate"] = SharePointHelper.ToDateTimeNullIfMin(EndDate);
            
            dest["IsPaid"] = IsPaid;
            dest["IsProfessionalService"] = IsProfessionalService;
            dest["AffirmDependantInformation"] = AffirmDependantInformation;
            dest["AffirmOfficialDuty"] = AffirmOfficialDuty;
            dest["IsNonProfitEtc"] = IsNonProfitEtc;

            dest["ClosedDate"] = SharePointHelper.ToDateTimeNullIfMin(ClosedDate);
        }

        public override void MapFromList(ListItem item, bool includeChildren = false)
        {
            base.MapFromList(item);

            if (item["EmployeeUpn"] != null)
                EmployeeUpn = ((FieldUserValue)item["EmployeeUpn"]).LookupValue;

            Status = SharePointHelper.ToStringNullSafe(item["Status"]);
            EmployeesName = SharePointHelper.ToStringNullSafe(item["EmployeesName"]);
            Grade = SharePointHelper.ToStringNullSafe(item["Grade"]);
            Branch = SharePointHelper.ToStringNullSafe(item["Branch"]);
            Division = SharePointHelper.ToStringNullSafe(item["Division"]);
            Supervisor = SharePointHelper.ToStringNullSafe(item["Supervisor"]);
            Duties = SharePointHelper.ToStringNullSafe(item["Duties"]);
            ProposedActivity = SharePointHelper.ToStringNullSafe(item["ProposedActivity"]);
            PerformedFor = SharePointHelper.ToStringNullSafe(item["PerformedFor"]);
            TypeOfWork = SharePointHelper.ToStringNullSafe(item["TypeOfWork"]);
            NumberOfHours = SharePointHelper.ToStringNullSafe(item["NumberOfHours"]);
            StartDate = SharePointHelper.ToDateTimeNullIfMin(Convert.ToDateTime(item["StartDate"]));
            EndDate = SharePointHelper.ToDateTimeNullIfMin(Convert.ToDateTime(item["EndDate"]));
            IsPaid = SharePointHelper.ToStringNullSafe(item["IsPaid"]) == "True";
            IsProfessionalService = SharePointHelper.ToStringNullSafe(item["IsProfessionalService"]) == "True";
            AffirmDependantInformation = SharePointHelper.ToStringNullSafe(item["AffirmDependantInformation"]) == "True";
            AffirmOfficialDuty = SharePointHelper.ToStringNullSafe(item["AffirmOfficialDuty"]) == "True";
            IsNonProfitEtc = SharePointHelper.ToStringNullSafe(item["IsNonProfitEtc"]) == "True";
            ClosedDate = SharePointHelper.ToDateTimeNullIfMin(Convert.ToDateTime(item["ClosedDate"]));
        }
        #endregion

        public string RunBusinessRules(UserInfo appUser, UserInfo submitter, OutsidePosition oldItem)
        {
            var result = "";
            this.Title = string.IsNullOrEmpty(ProposedActivity) ? "New Position" : ProposedActivity;

            if (string.IsNullOrEmpty(this.Status))
                this.Status = Constants.EventRequestStatus.DRAFT;


            //if (this.Status == Constants.EventRequestStatus.UNASSIGNED && (oldItem == null || oldItem.Status == Constants.EventRequestStatus.DRAFT))
            //{
            //    this.SubmittedBy = appUser.Upn;
            //    this.Submitter = appUser.DisplayName;

            //    // EventRequest is being submitted, trigger email notification
            //    var emailData = this.GetEmailData(submitter);

            //    _pendingEmails.Add(EmailHelper.GetEmail(NotificationTemplates.NotificationTypes.EVENT_REQUEST_CONFIRMATION, submitter, emailData));
            //    _pendingEmails.Add(EmailHelper.GetEmail(NotificationTemplates.NotificationTypes.EVENT_REQUEST_SUBMITTED, submitter, emailData));
            //}

            //if (!string.IsNullOrEmpty(this.AssignedToUpn) && oldItem != null && oldItem.AssignedToUpn != this.AssignedToUpn)
            //{
            //    // Attempting to assign to a new reviewer, if admin or reviewer, accept assignment, set status to Open, else return error
            //    if (appUser.IsAdmin || appUser.IsReviewer)
            //    {
            //        this.AssignedTo = SharePointHelper.EnsureUser(this.AssignedToUpn).Title;
            //        this.Status = Constants.EventRequestStatus.OPEN;

            //        var emailData = this.GetEmailData(submitter);
            //        _pendingEmails.Add(EmailHelper.GetEmail(NotificationTemplates.NotificationTypes.EVENT_REQUEST_ASSIGNED, submitter, emailData));
            //    }
            //    else
            //        result = "Unable to assign event request, you do not have permission to perform this action.";
            //}

            if (oldItem != null && !oldItem.Status.Contains(Constants.EventRequestStatus.CLOSED) && this.Status.Contains(Constants.EventRequestStatus.CLOSED))
            {
                // Attempting to close, only let admin or reviewer close, otherwise return error
                if (appUser.IsAdmin/* || appUser.IsReviewer*/)
                    this.ClosedDate = DateTime.Now;
                else
                    result = "Unable to close event request, you do not have permission to perform this action.";
            }

            return result;
        }

        public void ProcessEmails()
        {
            
        }
    }
}
