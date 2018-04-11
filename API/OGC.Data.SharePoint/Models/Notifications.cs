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
        }
    }
}
