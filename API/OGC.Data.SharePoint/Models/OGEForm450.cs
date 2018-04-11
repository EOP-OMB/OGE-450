using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OMB.SharePoint.Infrastructure;

namespace OGC.Data.SharePoint.Models
{
    public class OGEForm450 : SPListBase<OGEForm450>, ISPList
    {

        #region Properties
        public string Filer { get; set; }
        public int Year { get; set; }
        public string ReportingStatus { get; set; }
        public DateTime DueDate { get; set; }
        public string FormStatus { get; set; }

        public DateTime? DateReceivedByAgency { get; set; }
        public string EmployeesName { get; set; }
        public string EmailAddress { get; set; }
        public string PositionTitle { get; set; }
        public string Grade { get; set; }
        public string Agency { get; set; }
        public string BranchUnitAndAddress { get; set; }
        public string WorkPhone { get; set; }
        public DateTime? DateOfAppointment { get; set; }
        public bool IsSpecialGovernmentEmployee { get; set; }
        public string MailingAddress { get; set; }
        public bool HasAssetsOrIncome { get; set; }
        public bool HasLiabilities { get; set; }
        public bool HasOutsidePositions { get; set; }
        public bool HasAgreementsOrArrangements { get; set; }
        public bool HasGiftsOrTravelReimbursements { get; set; }

        public string EmployeeSignature { get; set; }
        public DateTime? DateOfEmployeeSignature { get; set; }

        public string ReviewingOfficialSignature { get; set; }
        public DateTime? DateOfReviewerSignature { get; set; }

        public string CommentsOfReviewingOfficial { get; set; }

        public bool isRejected { get; set; }
        public string RejectionNotes { get; set; }

        public int DaysExtended { get; set; }
        public string ExtendedText { get; set; }

        public Boolean IsOverdue { get; set; }
        public bool IsBlank { get; set; }

        public string AppUser { get; set; }
        public string CorrelationId { get; set; }

        public string FormFlags { get; set; }

        public List<ReportableInformation> ReportableInformationList { get; set; }

        private List<Notifications> _pendingEmails;
        

        public string Note { get; set; }
        public bool SubmittedPaperCopy { get; set; }
        #endregion

        public OGEForm450()
        {
            this.ListName = this.GetType().Name;
            _pendingEmails = new List<Notifications>();
        }

        #region Business Rules
        public void RunBusinessRules(UserInfo user, UserInfo filer, OGEForm450 oldItem)
        {
            // Check to see if we should even allow save
            if (!canSave(user, filer, oldItem))
                throw new Exception("Unauthorized:  Cannot save form.");

            _pendingEmails = new List<Notifications>();
            var emailData = this.GetEmailData(filer);

            if ((this.DaysExtended != oldItem.DaysExtended && this.DueDate != oldItem.DueDate) || (this.DueDate != oldItem.DueDate && !user.IsAdmin))
            {
                // if here, form has been extended after form was loaded and subsequently saved.  Use the old item's due date.
                // Or, due date was changed by a non-admin 
                this.DueDate = oldItem.DueDate;
            }

            // Set status back to oldItem (do not update FormStatus, or DaysExtended, set elsewhere)
            this.FormStatus = oldItem.FormStatus;
            this.DaysExtended = oldItem.DaysExtended;

            if (user.Upn == filer.Upn && !string.IsNullOrEmpty(this.EmployeeSignature) && oldItem.EmployeeSignature != this.EmployeeSignature)
            {
                // If the Employee signed the form, stamp the date/time and update the FormStatus to Submitted
                this.DateReceivedByAgency = DateTime.Now;
                this.DateOfEmployeeSignature = DateTime.Now.Date;
                this.FormStatus = oldItem.FormStatus == Constants.FormStatus.MISSING_INFORMATION ? Constants.FormStatus.RE_SUBMITTED : Constants.FormStatus.SUBMITTED;
                _pendingEmails.Add(EmailHelper.GetEmail(NotificationTemplates.NotificationTypes.OGE_FORM_450_SUBMITTED, filer, emailData));
                _pendingEmails.Add(EmailHelper.GetEmail(NotificationTemplates.NotificationTypes.OGE_FORM_450_CONFIRMATION, filer, emailData));
            }

            if ((this.FormStatus == Constants.FormStatus.NOT_STARTED || string.IsNullOrEmpty(this.FormStatus)) && this.DueDate == oldItem.DueDate && user.Upn == filer.Upn)
            {
                // If the form is being saved for the first time (ie status is "Not Started"), update it to Draft
                // Only if it's the filer making the change and not an admin changing the due date.
                this.FormStatus = Constants.FormStatus.DRAFT;
            }

            if (user.IsReviewer || user.IsAdmin)
            {
                if (this.isRejected)
                {
                    this.FormStatus = Constants.FormStatus.MISSING_INFORMATION;
                    this.EmployeeSignature = "";
                    this.DateOfEmployeeSignature = null;
                    _pendingEmails.Add(EmailHelper.GetEmail(NotificationTemplates.NotificationTypes.OGE_FORM_450_MISSING_INFO, filer, emailData));
                }

                if (!string.IsNullOrEmpty(this.ReviewingOfficialSignature) && oldItem.ReviewingOfficialSignature != this.ReviewingOfficialSignature)
                {
                    // If the reviewing official signed the form, stamp the date/time and update the FormStatus to Certified
                    this.DateOfReviewerSignature = DateTime.Now;
                    this.FormStatus = Constants.FormStatus.CERTIFIED;
                    _pendingEmails.Add(EmailHelper.GetEmail(NotificationTemplates.NotificationTypes.OGE_FORM_450_CERTIFIED, filer, emailData));
                }
            }
            else
                this.CommentsOfReviewingOfficial = oldItem.CommentsOfReviewingOfficial;
        }

        public static OGEForm450 Create(Employee emp)
        {
            var settings = Settings.GetAll().FirstOrDefault();

            var form = GetCurrentFormByUser(emp.AccountName);

            if (form != null)
                throw new Exception("Cannot create a new form for " + emp.DisplayName + ".  A current form already exists.");

            form = new OGEForm450();

            form.Filer = emp.AccountName;
            form.Agency = emp.Agency;
            form.BranchUnitAndAddress = emp.Branch;
            form.DateOfAppointment = emp.AppointmentDate;
            form.DueDate = emp.DueDate ?? GetNextBusinessDay(DateTime.Now, 30);
            form.EmailAddress = emp.EmailAddress;
            form.EmployeesName = emp.DisplayName;
            form.FormStatus = Constants.FormStatus.NOT_STARTED;
            form.PositionTitle = emp.Position;
            form.ReportingStatus = emp.ReportingStatus;
            form.Title = emp.DisplayName + " (" + settings.CurrentFilingYear.ToString() + ")";
            form.WorkPhone = emp.WorkPhone;
            form.Year = settings.CurrentFilingYear;
            form.AppUser = emp.DisplayName;
            form.CorrelationId = Guid.NewGuid().ToString();
            form.SubmittedPaperCopy = false;

            if (emp.ReportingStatus == Constants.ReportingStatus.NEW_ENTRANT)
                form.DateOfAppointment = emp.AppointmentDate;

            var newForm = form.Save();

            // Set up email data
            newForm.Note = emp.Notes;

            var user = UserInfo.GetUser(emp.AccountName);
            var emailData = newForm.GetEmailData(user);

            if (form.ReportingStatus == Constants.ReportingStatus.NEW_ENTRANT)
                newForm.AddEmail(EmailHelper.GetEmail(NotificationTemplates.NotificationTypes.OGE_FORM_450_NEW_ENTRANT, user, emailData, emp.NewEntrantEmailText));
            else
                newForm.AddEmail(EmailHelper.GetEmail(NotificationTemplates.NotificationTypes.OGE_FORM_450_NEW_ANNUAL, user, emailData, emp.AnnualEmailText));

            return newForm;
        }

        public static DateTime GetNextBusinessDay(DateTime dt, int days)
        {
             var tmp = dt.AddDays(30);

            while (tmp.DayOfWeek == DayOfWeek.Saturday || tmp.DayOfWeek == DayOfWeek.Sunday)
                tmp = tmp.AddDays(1);

            return tmp;
        }

        public static OGEForm450 GetCurrentFormByUser(string accountName)
        {
            var settings = Settings.GetAll().FirstOrDefault();
            var forms = OGEForm450.GetAllBy("Filer", accountName);

            return forms.Where(x => x.Year == settings.CurrentFilingYear && x.FormStatus != Constants.FormStatus.CANCELED).OrderByDescending(x => x.DueDate).FirstOrDefault();
        }

        private bool canSave(UserInfo user, UserInfo filer, OGEForm450 oldForm)
        {
            // can save this form if it's the user's form or if user is an admin or reviewer
            var canUserSave = (user.Upn.ToLower() == filer.Upn.ToLower() && (oldForm.FormStatus == Constants.FormStatus.DRAFT || oldForm.FormStatus == Constants.FormStatus.NOT_STARTED || oldForm.FormStatus == Constants.FormStatus.MISSING_INFORMATION));
            var canReviewerSave = user.IsReviewer && (oldForm.FormStatus == Constants.FormStatus.SUBMITTED || oldForm.FormStatus == Constants.FormStatus.RE_SUBMITTED);

            return canUserSave || canReviewerSave || user.IsAdmin;
        }
        #endregion

        #region Mapping
        public override void MapFromList(ListItem item, bool includeChildren = false)
        {
            base.MapFromList(item);

            this.Agency = SharePointHelper.ToStringNullSafe(item["Agency"]);
            this.BranchUnitAndAddress = SharePointHelper.ToStringNullSafe(item["BranchUnitAndAddress"]);
            this.CommentsOfReviewingOfficial = SharePointHelper.ToStringNullSafe(item["CommentsOfReviewingOfficial"]);
            this.DateOfAppointment = SharePointHelper.ToDateTimeNullIfMin(Convert.ToDateTime(item["DateOfAppointment"]));
            this.DateOfEmployeeSignature = SharePointHelper.ToDateTimeNullIfMin(Convert.ToDateTime(item["DateOfEmployeeSignature"]));
            this.DateOfReviewerSignature = SharePointHelper.ToDateTimeNullIfMin(Convert.ToDateTime(item["DateOfReviewerSignature"]));
            this.DateReceivedByAgency = SharePointHelper.ToDateTimeNullIfMin(Convert.ToDateTime(item["DateReceivedByAgency"]));
            this.EmployeeSignature = SharePointHelper.ToStringNullSafe(item["EmployeeSignature"]);
            this.EmailAddress = SharePointHelper.ToStringNullSafe(item["EmailAddress"]);
            this.EmployeesName = SharePointHelper.ToStringNullSafe(item["EmployeesName"]);
            this.Grade = SharePointHelper.ToStringNullSafe(item["Grade"]);
            this.HasAgreementsOrArrangements = SharePointHelper.ToStringNullSafe(item["HasAgreementsOrArrangements"]) == "True";
            this.HasAssetsOrIncome = SharePointHelper.ToStringNullSafe(item["HasAssetsOrIncome"]) == "True";
            this.HasGiftsOrTravelReimbursements = SharePointHelper.ToStringNullSafe(item["HasGiftsOrTravelReimbursements"]) == "True";
            this.HasLiabilities = SharePointHelper.ToStringNullSafe(item["HasLiabilities"]) == "True";
            this.HasOutsidePositions = SharePointHelper.ToStringNullSafe(item["HasOutsidePositions"]) == "True";
            this.IsSpecialGovernmentEmployee = SharePointHelper.ToStringNullSafe(item["IsSpecialGovernmentEmployee"]) == "True";
            this.MailingAddress = SharePointHelper.ToStringNullSafe(item["MailingAddress"]);
            this.PositionTitle = SharePointHelper.ToStringNullSafe(item["PositionTitle"]);
            this.ReportingStatus = SharePointHelper.ToStringNullSafe(item["ReportingStatus"]);
            this.ReviewingOfficialSignature = SharePointHelper.ToStringNullSafe(item["ReviewingOfficialSignature"]);
            this.FormStatus = SharePointHelper.ToStringNullSafe(item["FormStatus"]);
            this.WorkPhone = SharePointHelper.ToStringNullSafe(item["WorkPhone"]);
            this.Year = Convert.ToInt32(item["Year"]);
            this.Filer = ((FieldUserValue)item["Filer"]).LookupValue;
            this.DueDate = Convert.ToDateTime(item["DueDate"]);
            this.DaysExtended = Convert.ToInt32(item["DaysExtended"]);
            this.ExtendedText = this.DaysExtended > 0 ? "Yes (" + this.DaysExtended.ToString() + ")" : "No";
            this.SubmittedPaperCopy = Convert.ToBoolean(item["SubmittedPaperCopy"]);
            this.FormFlags = GetFormFlags();

            if (includeChildren)
                this.ReportableInformationList = ReportableInformation.GetAllBy(ListName + "Id", Id);
        }

        private string GetFormFlags()
        {
            var flags = "";

            this.IsOverdue = this.DueDate < DateTime.Now && this.FormStatus != Constants.FormStatus.CANCELED && this.FormStatus != Constants.FormStatus.CERTIFIED && this.FormStatus != Constants.FormStatus.SUBMITTED && this.FormStatus != Constants.FormStatus.RE_SUBMITTED;
            this.IsBlank = !(this.HasAssetsOrIncome || this.HasLiabilities || this.HasOutsidePositions || this.HasAgreementsOrArrangements || this.HasGiftsOrTravelReimbursements) && !this.SubmittedPaperCopy && (this.FormStatus == Constants.FormStatus.SUBMITTED || this.FormStatus == Constants.FormStatus.RE_SUBMITTED || this.FormStatus == Constants.FormStatus.CERTIFIED);

            if (this.SubmittedPaperCopy && (this.FormStatus == Constants.FormStatus.SUBMITTED || this.FormStatus == Constants.FormStatus.RE_SUBMITTED || this.FormStatus == Constants.FormStatus.CERTIFIED))
            {
                flags += Constants.FormFlags.PAPER_COPY + "|";
                this.SubmittedPaperCopy = true;
            }
            else
                this.SubmittedPaperCopy = false;

            if (this.DaysExtended > 0)
                flags += Constants.FormFlags.EXTENDED + "|";

            if (this.IsBlank)
                flags += Constants.FormFlags.BLANK_SUBMISSION + "|";

            if (this.IsOverdue)
                flags += Constants.FormFlags.OVERDUE + "|";

            return flags;
        }

        public override void MapToList(ListItem dest)
        {
            if (Id == 0)
                dest["Filer"] = SharePointHelper.GetFieldUser(SPContext, this.Filer);

            base.MapToList(dest);

            dest["Agency"] = this.Agency;
            dest["BranchUnitAndAddress"] = this.BranchUnitAndAddress;
            dest["CommentsOfReviewingOfficial"] = this.CommentsOfReviewingOfficial;
            dest["DateOfAppointment"] = SharePointHelper.ToDateTimeNullIfMin(this.DateOfAppointment);
            dest["DateOfEmployeeSignature"] = SharePointHelper.ToDateTimeNullIfMin(DateOfEmployeeSignature);
            dest["DateOfReviewerSignature"] = SharePointHelper.ToDateTimeNullIfMin(this.DateOfReviewerSignature);
            dest["DateReceivedByAgency"] = SharePointHelper.ToDateTimeNullIfMin(this.DateReceivedByAgency);
            dest["EmailAddress"] = this.EmailAddress;
            dest["EmployeesName"] = this.EmployeesName;
            dest["EmployeeSignature"] = this.EmployeeSignature;
            dest["FormStatus"] = this.FormStatus;
            dest["Grade"] = this.Grade;
            dest["HasAgreementsOrArrangements"] = this.HasAgreementsOrArrangements;
            dest["HasAssetsOrIncome"] = this.HasAssetsOrIncome;
            dest["HasGiftsOrTravelReimbursements"] = this.HasGiftsOrTravelReimbursements;
            dest["HasLiabilities"] = this.HasLiabilities;
            dest["HasOutsidePositions"] = this.HasOutsidePositions;
            dest["IsSpecialGovernmentEmployee"] = this.IsSpecialGovernmentEmployee;
            dest["MailingAddress"] = this.MailingAddress;
            dest["PositionTitle"] = this.PositionTitle;
            dest["Year"] = this.Year;

            dest["ReportingStatus"] = this.ReportingStatus;
            dest["ReviewingOfficialSignature"] = this.ReviewingOfficialSignature;
            dest["WorkPhone"] = this.WorkPhone;
            dest["DueDate"] = this.DueDate;

            dest["DaysExtended"] = this.DaysExtended;
            dest["AppUser"] = this.AppUser;
            dest["CorrelationId"] = this.CorrelationId;

            dest["SubmittedPaperCopy"] = Convert.ToBoolean(this.SubmittedPaperCopy);
        }
        #endregion

        #region Methods
        public static List<OGEForm450> GetAllReviewable()
        {
            var ctx = new ClientContext(SharePointHelper.Url);
            var type = new OGEForm450();
            var results = new List<OGEForm450>();

            try
            {
                var web = SharePointHelper.GetWeb(ctx);
                var list = SharePointHelper.GetList(ctx, web, type.ListName);

                var caml = GetReviewableCaml();
                var items = list.GetItems(caml);

                ctx.Load(items);
                ctx.ExecuteQuery();

                foreach (ListItem item in items)
                {
                    var t = new OGEForm450();

                    t.MapFromList(item);
                    results.Add(t);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to get " + type.ListName + ". " + ex.Message);
            }

            return results;
        }

        private static CamlQuery GetReviewableCaml()
        {
            var caml = new CamlQuery();

            caml.ViewXml = "<View><Query><Where><Or><And><Lt><FieldRef Name='DueDate' /><Value IncludeTimeValue='TRUE' Type='DateTime'>" + DateTime.UtcNow.ToString("s") + "Z" + "</Value></Lt><And><Neq><FieldRef Name='FormStatus' /><Value Type='Choice'>Certified</Value></Neq><Neq><FieldRef Name='FormStatus' /><Value Type='Choice'>Canceled</Value></Neq></And></And><And><Neq><FieldRef Name='FormStatus' /><Value Type='Text'>" + Constants.FormStatus.CERTIFIED + "</Value></Neq><Neq><FieldRef Name='FormStatus' /><Value Type='Text'>" + Constants.FormStatus.CANCELED + "</Value></Neq></And></Or></Where></Query></View>";

            return caml;
        }

        public void SaveReportableInformation()
        {
            foreach (ReportableInformation info in this.ReportableInformationList)
            {
                if (info.Id > 0 && info.IsEmpty())
                    info.Delete();
                else
                {
                    if (info.OGEForm450Id == 0)
                        info.OGEForm450Id = this.Id;

                    info.Save();
                }
                
            }
        }

        public static List<OGEForm450> CertifyBlankForms(UserInfo user)
        {
            var blankForms = GetAllReviewable();
            blankForms = blankForms.Where(x => x.IsBlank == true && (x.FormStatus == Constants.FormStatus.SUBMITTED || x.FormStatus == Constants.FormStatus.RE_SUBMITTED)).ToList();

            foreach (OGEForm450 form in blankForms)
            {
                var filer = UserInfo.GetUser(form.Filer);
                var oldItem = OGEForm450.Get(form.Id);

                form.ReviewingOfficialSignature = user.DisplayName;

                form.RunBusinessRules(user, filer, oldItem);
                form.Save();

                form.ProcessEmails();
            }

            return blankForms;
        }
        #endregion

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

        public void Extend(ExtensionRequest item)
        {
            this.DueDate = item.ExtensionDate;
            this.DaysExtended += item.DaysRequested;

            this.Save();
        }

        public Dictionary<string, string> GetEmailData(UserInfo user)
        {
            var dict = new Dictionary<string, string>();

            dict = Settings.GetAppSettings(dict);

            dict.Add("User", user.DisplayName);
            dict.Add("Email", user.Email);
            
            dict.Add("Note", this.Note);
            dict.Add("Filer", this.EmployeesName);
            dict.Add("ReviewerNote", this.CommentsOfReviewingOfficial);
            dict.Add("RejectionNotes", this.RejectionNotes);
            dict.Add("DueDate", this.DueDate.ToShortDateString());

            return dict;
        }

        public static Dictionary<string, string> GetEmailFieldsDef(Dictionary<string, string> dict)
        {
            dict.Add("[User]", "The Display Name of the filer.");
            dict.Add("[Email]", "The filer's email address.");
            dict.Add("[Filer]", "The Employees Name field from the OGE Form 450");
            dict.Add("[ReviewerNote]", "The Comments Of Reviewing Official from the OGE Form 450");
            dict.Add("[RejectionNotes]", "Notes left by reviewer when Rejecting an OGE Form 450");
            dict.Add("[DueDate]", "The Due Date of the filer's OGE Form 450");

            return dict;
        }
        #endregion
    }
}
