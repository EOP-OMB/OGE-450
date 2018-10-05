using System;
using System.Configuration;
using Microsoft.SharePoint.Client;
using OMB.SharePoint.Infrastructure;
using System.Collections.Generic;

namespace OGC.Data.SharePoint.Models
{
    public class NotificationTemplates : SPListBase<NotificationTemplates>, ISPList
    {
        public class NotificationTypes
        {
            public const string EXTENSION_REQUEST = "Extension Request";
            public const string EXTENSION_RECEIVED = "Extension Received";
            public const string EXTENSION_DECISION = "Extension Decision";
            public const string OGE_FORM_450_SUBMITTED = "OGE Form 450 Submitted";
            public const string OGE_FORM_450_CONFIRMATION = "OGE Form 450 Submission Confirmation";
            public const string OGE_FORM_450_CERTIFIED = "OGE Form 450 Certified";
            public const string OGE_FORM_450_MISSING_INFO = "OGE Form 450 Missing Information";
            public const string OGE_FORM_450_NEW_ANNUAL = "OGE Form 450 New Annual Filing";
            public const string OGE_FORM_450_NEW_ENTRANT = "OGE Form 450 New Entrant Filing";

            public const string EVENT_REQUEST_SUBMITTED = "Event Requested Submitted";
            public const string EVENT_REQUEST_CONFIRMATION = "Event Request Submitted Confirmation";
            public const string EVENT_REQUEST_ASSIGNED = "Event Request Assigned";
        }

        #region properties
        public string SharePointList { get; set; }
        public string ViewName { get; set; }
        public string RecipientType { get; set; }
        public string RecipientColumn { get; set; }
        public string Subject { get; set; }
        public string Frequency { get; set; }
        public string Body { get; set; }
        public DateTime? NextRunDate { get; set; }
        public DateTime? LastRunDate { get; set; }
        public string LastRunStatus { get; set; }
        public bool IncludeCc { get; set; }
        public bool Enabled { get; set; }
        public string Application { get; set; }
        #endregion

        public Dictionary<string, string> TemplateFields { get; set; }

        public NotificationTemplates()
        {
            this.ListName = this.GetType().Name;
        }

        public override void MapToList(ListItem dest)
        {
            base.MapToList(dest);

            dest["RecipientType"] = RecipientType;
            dest["RecipientColumn"] = RecipientColumn;
            dest["Subject"] = Subject;
            dest["Body"] = Body;
            dest["IncludeCC"] = IncludeCc;

            if (Frequency != "Real Time")
            {
                dest["SharePointList"] = SharePointList;
                dest["ViewName"] = ViewName;
                dest["Frequency"] = Frequency;
                dest["NextRunDateTime"] = SharePointHelper.ToDateTimeNullIfMin(NextRunDate);
                dest["LastRunDateTime"] = SharePointHelper.ToDateTimeNullIfMin(LastRunDate);
                dest["LastRunStatus"] = LastRunStatus;
            }

            dest["Application"] = Application;
        }

        public override void MapFromList(ListItem item, bool includeChildren = false)
        {
            base.MapFromList(item);

            this.SharePointList = SharePointHelper.ToStringNullSafe(item["SharePointList"]);
            this.ViewName = SharePointHelper.ToStringNullSafe(item["ViewName"]);
            this.RecipientType = SharePointHelper.ToStringNullSafe(item["RecipientType"]);
            this.RecipientColumn = SharePointHelper.ToStringNullSafe(item["RecipientColumn"]);
            this.Subject = SharePointHelper.ToStringNullSafe(item["Subject"]);
            this.Frequency = SharePointHelper.ToStringNullSafe(item["Frequency"]);
            this.Body = SharePointHelper.ToStringNullSafe(item["Body"]);
            this.NextRunDate = Convert.ToDateTime(item["NextRunDateTime"]);
            this.LastRunDate = Convert.ToDateTime(item["LastRunDateTime"]);
            this.LastRunStatus = SharePointHelper.ToStringNullSafe(item["LastRunStatus"]);
            this.IncludeCc = Convert.ToBoolean(item["IncludeCC"]);
            this.Enabled = Convert.ToBoolean(item["Enabled"]);
            this.Application = SharePointHelper.ToStringNullSafe(item["Application"]);

            var dict = new Dictionary<string, string>();

            dict = Settings.GetAppEmailFieldsDef(dict);

            if (Title == NotificationTypes.EXTENSION_DECISION || Title == NotificationTypes.EXTENSION_RECEIVED || Title == NotificationTypes.EXTENSION_REQUEST)
            {
                dict = ExtensionRequest.GetEmailFieldsDef(dict);
            }
            else if (this.Application == Constants.ApplicationName.OGE_FORM_450)
            {
                dict = OGEForm450.GetEmailFieldsDef(dict);
            }
            else if (this.Application == Constants.ApplicationName.EVENT_CLEARANCE)
            {
                dict = EventRequest.GetEmailFieldsDef(dict);
            }

            this.TemplateFields = dict;
        }
    }
}
