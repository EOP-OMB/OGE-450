using System;
using System.Linq;
using System.Configuration;
using Microsoft.SharePoint.Client;
using OMB.SharePoint.Infrastructure;

namespace OGC.Data.SharePoint.Models
{
    public class Notifications : SPListBase<Notifications>, ISPList
    {
        #region properties
        public string Recipient { get; set; }
        public string Cc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime? SentDateTime { get; set; }
        public string Status { get; set; }
        public string ErrorMessage { get; set; }
        public string Application { get; set; }
        public bool IsAnnouncment { get; set; }
        //public string AttachmentListName { get; set; }
        //public string AttachmentKey { get; set; }
        //public int AttachmentKeyValue { get; set; }
        public int Year
        {
            get
            {
                return SentDateTime == null ? (Created == null ? 1900 : Created.Value.Year) : SentDateTime.Value.Year;
            }
        }
        #endregion

        public Notifications()
        {
            this.ListName = this.GetType().Name;
        }

        public override void MapFromList(ListItem item, bool includeChildren = false)
        {
            base.MapFromList(item);

            this.Subject = SharePointHelper.ToStringNullSafe(item["Subject"]);
            this.Recipient = SharePointHelper.ToStringNullSafe(item["Recipient"]);
            this.Cc = SharePointHelper.ToStringNullSafe(item["Cc"]);
            this.Body = SharePointHelper.ToStringNullSafe(item["Body"]);
            this.SentDateTime = Convert.ToDateTime(item["SentDateTime"]);
            this.Status = SharePointHelper.ToStringNullSafe(item["Status"]);
            this.ErrorMessage = SharePointHelper.ToStringNullSafe(item["ErrorMessage"]);
            this.Application = SharePointHelper.ToStringNullSafe(item["Application"]);
            this.IsAnnouncment = Convert.ToBoolean(item["IsAnnouncement"]);
            //this.AttachmentListName = SharePointHelper.ToStringNullSafe(item["AttachmentListName"]);
            //this.AttachmentKey = SharePointHelper.ToStringNullSafe(item["AttachmentKey"]);
            //this.AttachmentKeyValue = Convert.ToInt32(item["AttachmentKeyValue"]);
        }

        public static void UpdateInfo()
        {
            var notifications = Notifications.GetAll();
            var systemNotifications = NotificationTemplates.GetAllBy("Frequency", "Real Time", true);

            foreach(Notifications n in notifications)
            {
                if (systemNotifications.Where(x => x.Title == n.Title).Count() > 0)
                    n.IsAnnouncment = true;
                else
                    n.IsAnnouncment = false;

                n.Application = Constants.ApplicationName.OGE_FORM_450;

                n.Save();
            }
        }

        public override void MapToList(ListItem dest)
        {
            base.MapToList(dest);

            dest["Subject"] = this.Subject;
            dest["Recipient"] = this.Recipient;
            dest["Cc"] = this.Cc;
            dest["Body"] = this.Body;
            dest["SentDateTime"] = SharePointHelper.ToDateTimeNullIfMin(this.SentDateTime);
            dest["Status"] = this.Status;
            dest["ErrorMessage"] = this.ErrorMessage;
            dest["Application"] = Application;
            dest["IsAnnouncement"] = IsAnnouncment;
            //dest["AttachmentListName"] = AttachmentListName;
            //dest["AttachmentKey"] = AttachmentKey;
            //dest["AttachmentKeyValue"] = AttachmentKeyValue;
        }
    }
}
