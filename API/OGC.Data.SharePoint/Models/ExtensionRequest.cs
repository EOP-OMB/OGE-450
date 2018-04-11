using System;
using System.Collections.Generic;
using Microsoft.SharePoint.Client;
using OMB.SharePoint.Infrastructure;

namespace OGC.Data.SharePoint.Models
{
    public class ExtensionRequest : SPListBase<ExtensionRequest>, ISPList
    {
        #region properties
        public int OGEForm450Id { get; set; }
        public string Reason { get; set; }
        public int DaysRequested { get; set; }
        public string Status { get; set; }
        public DateTime ExtensionDate { get; set; }

        public string ReviewerComments { get; set; }

        public string FilerName { get; set; }
        public string Year { get; set; }
        public DateTime DueDate { get; set; }
        #endregion

        private List<Notifications> _pendingEmails;

        public ExtensionRequest()
        {
            this.ListName = this.GetType().Name;
        }

        public static List<ExtensionRequest> GetPendingExtensions(int ogeForm450Id)
        {
            var ctx = new ClientContext(SharePointHelper.Url);
            var type = new ExtensionRequest();
            var results = new List<ExtensionRequest>();

            try
            {
                var web = SharePointHelper.GetWeb(ctx);
                var list = SharePointHelper.GetList(ctx, web, type.ListName);

                var caml = GetPendingRequestsCaml(ogeForm450Id);
                var items = list.GetItems(caml);
                ctx.Load(items);
                ctx.ExecuteQuery();

                foreach (ListItem item in items)
                {
                    var t = new ExtensionRequest();

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

        private static CamlQuery GetPendingRequestsCaml(int id)
        {
            var caml = new CamlQuery();

            caml.ViewXml = "<View><Query><Where><And><Eq><FieldRef Name='OGEForm450Id' /><Value Type='Integer'>" + id + "</Value></Eq><Eq><FieldRef Name='Status' /><Value Type='Text'>Pending</Value></Eq></And></Where></Query></View>";

            return caml;
        }

        public override void MapToList(ListItem dest)
        {
            base.MapToList(dest);

            var flv = new FieldLookupValue();
            flv.LookupId = OGEForm450Id;

            dest["Title"] = DaysRequested + " days requested";
            dest["OGEForm450Id"] = flv;
            dest["Reason"] = Reason;
            dest["DaysRequested"] = DaysRequested;
            dest["ReviewerComments"] = ReviewerComments;
            dest["Status"] = Status;

            dest["ExtensionDate"] = GetExtensionDate();
        }

        public override void MapFromList(ListItem item, bool includeChildren = false)
        {
            base.MapFromList(item);

            OGEForm450Id = ((FieldLookupValue)item["OGEForm450Id"]).LookupId;
            Reason = SharePointHelper.ToStringNullSafe(item["Reason"]);
            DaysRequested = Convert.ToInt32(item["DaysRequested"]);
            Status = SharePointHelper.ToStringNullSafe(item["Status"]);
            ExtensionDate = Convert.ToDateTime(item["ExtensionDate"]);
            ReviewerComments = SharePointHelper.ToStringNullSafe(item["ReviewerComments"]);

            FilerName = SharePointHelper.ToStringNullSafe(((FieldLookupValue)item["OGEForm450Id_x003a_EmployeesName"]).LookupValue);
            Year = Convert.ToInt32(Convert.ToDouble(((FieldLookupValue)item["OGEForm450Id_x003a_Year"]).LookupValue)).ToString();
            DueDate = Convert.ToDateTime(((FieldLookupValue)item["OGEForm450Id_x003a_DueDate"]).LookupValue);
        }

        private DateTime GetExtensionDate()
        {
            var form = OGEForm450.Get(OGEForm450Id);

            return form.DueDate.AddDays(DaysRequested);
        }

        public Dictionary<string, string> GetEmailData(UserInfo user)
        {
            var dict = new Dictionary<string, string>();

            dict = Settings.GetAppSettings(dict);

            dict.Add("User", user.DisplayName);
            dict.Add("Email", user.Email);

            dict.Add("Status", this.Status);
            dict.Add("DaysRequested", this.DaysRequested.ToString());
            dict.Add("Reason", this.Reason);
            dict.Add("ReviewerComments", this.ReviewerComments);

            return dict;
        }

        public static Dictionary<string, string> GetEmailFieldsDef(Dictionary<string, string> dict)
        {
            dict.Add("[User]", "The Display Name of the requestor.");
            dict.Add("[Email]", "The requestor's email address.");

            dict.Add("[Status]", "The status of the extension request (" + Constants.ExtensionStatus.PENDING + ", " + Constants.ExtensionStatus.APPROVED + ", " + Constants.ExtensionStatus.REJECTED + ", " + Constants.ExtensionStatus.CANCELED + ")");
            dict.Add("[DaysRequested]", "The number of days requested.");
            dict.Add("[Reason]", "The reason given by the requestor.");
            dict.Add("[ReviewerComments]", "The comments provided by the reviewer.");

            return dict;
        }

        public string RunBusinessRules(UserInfo user, ExtensionRequest oldItem)
        {
            var form = OGEForm450.Get(this.OGEForm450Id);
            var filer = UserInfo.GetUser(form.Filer);
            
            _pendingEmails = new List<Notifications>();

            if (Id == 0 && !(form.FormStatus == Constants.FormStatus.NOT_STARTED || form.FormStatus == Constants.FormStatus.DRAFT || form.FormStatus == Constants.FormStatus.MISSING_INFORMATION))
            {
                return "Cannot make a request for a form that has been submitted or certified or cancelled";
            }
            
            // ensure 90 day rule
            if (form.DaysExtended + this.DaysRequested > 90)
            {
                return "Cannot make a request for more than 90 days";
            }
            
            if (oldItem != null && !user.IsReviewer)
            {
                return "Unauthorised: Cannot update record, you must be a reviewer to approve or deny an extension.";
            }

            if (this.DaysRequested == 0)
            {
                return "Must request at least one day";
            }

            if (this.Reason == "")
            {
                return "Must provide a reason for extension.";
            }

            var data = this.GetEmailData(user);

            if (oldItem == null)
            { 
                this.Status = Constants.ExtensionStatus.PENDING;
                _pendingEmails.Add(EmailHelper.GetEmail(NotificationTemplates.NotificationTypes.EXTENSION_RECEIVED, filer, data));
                _pendingEmails.Add(EmailHelper.GetEmail(NotificationTemplates.NotificationTypes.EXTENSION_REQUEST, filer, data));
            }
            else if (user.IsReviewer)
            {
                if (oldItem.Status == Constants.ExtensionStatus.PENDING && this.Status != Constants.ExtensionStatus.CANCELED && this.Status != Constants.ExtensionStatus.PENDING)
                {
                    _pendingEmails.Add(EmailHelper.GetEmail(NotificationTemplates.NotificationTypes.EXTENSION_DECISION, filer, data));
                }
            }

            // made it to the end, return empty string as success
            return "";
        }

        public void ProcessEmails()
        {
            if (_pendingEmails != null)
            {
                foreach (Notifications notification in _pendingEmails)
                    EmailHelper.AddEmail(notification);

                _pendingEmails.Clear();
            }
        }
    }
}
