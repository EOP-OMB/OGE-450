using System;
using Microsoft.SharePoint.Client;

namespace OMB.SharePoint.Infrastructure.Models
{
    public class Exceptions : SPListBase<Exceptions>, ISPList
    {
        #region Properties
        public string User { get; set; }
        public string Message { get; set; }
        public string InnerException { get; set; }
        public string Source { get; set; }
        public string StackTrace { get; set; }
        public string HelpLink { get; set; }
        public int HResult { get; set; }
        public string Data { get; set; }
        #endregion

        public Exceptions()
        {
            this.ListName = this.GetType().Name;
        }

        public override void MapFromList(ListItem item, bool includeChildren = false)
        {
            base.MapFromList(item);

            User = SharePointHelper.ToStringNullSafe(item["User"]);
            Message = SharePointHelper.ToStringNullSafe(item["Message"]);
            InnerException = SharePointHelper.ToStringNullSafe(item["InnerException"]);
            Source = SharePointHelper.ToStringNullSafe(item["Source"]);
            StackTrace = SharePointHelper.ToStringNullSafe(item["StackTrace"]);
            HelpLink = SharePointHelper.ToStringNullSafe(item["HelpLink"]);
            HResult = Convert.ToInt32(item["HResult"]);
            Data = SharePointHelper.ToStringNullSafe(item["Data"]);
        }

        public override void MapToList(ListItem dest)
        {
            base.MapToList(dest);

            dest["User"] = this.User;
            dest["Message"] = this.Message;
            dest["InnerException"] = this.InnerException;
            dest["Source"] = this.Source;
            dest["StackTrace"] = this.StackTrace;
            dest["HelpLink"] = this.HelpLink;
            dest["HResult"] = this.HResult;
            dest["Data"] = this.Data;
        }
    }
}
