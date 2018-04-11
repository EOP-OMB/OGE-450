using System;
using System.Configuration;
using Microsoft.SharePoint.Client;
using OMB.SharePoint.Infrastructure;

namespace OGC.Data.SharePoint.Models
{
    public class DebugLog : SPListBase<DebugLog>, ISPList
    {
        #region properties
        public string Application { get; set; }
        public string Namespace { get; set; }
        public string Function { get; set; }
        public string Info { get; set; }
        public string InfoType { get; set; }
        public DateTime? Date { get; set; }

        #endregion

        public DebugLog()
        {
            this.ListName = this.GetType().Name;
        }

        public override void MapToList(ListItem dest)
        {
            base.MapToList(dest);

            dest["Application"] = Application;
            dest["Namespace"] = Namespace;
            dest["Function"] = Function;
            dest["Info"] = Info;
            dest["InfoType"] = InfoType;
            dest["Date"] = SharePointHelper.ToDateTimeNullIfMin(Date);
        }

        public override void MapFromList(ListItem item, bool includeChildren = false)
        {
            base.MapFromList(item);

            this.Application = SharePointHelper.ToStringNullSafe(item["Application"]);
            this.Namespace = SharePointHelper.ToStringNullSafe(item["Namespace"]);
            this.Function = SharePointHelper.ToStringNullSafe(item["Function"]);
            this.Info = SharePointHelper.ToStringNullSafe(item["Info"]);
            this.InfoType = SharePointHelper.ToStringNullSafe(item["InfoType"]);
            this.Date = SharePointHelper.ToDateTimeNullIfMin(Convert.ToDateTime(item["Date"]));
        }

        public static void Log(string user, string application, string ns, string function, string info, string infoType, DateTime date)
        {
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["debugLog"]))
            {
                var log = new DebugLog() { Title=user, Application = application, Namespace = ns, Function = function, Info = info, InfoType = infoType, Date = date };

                log.Save();
            }
        }
    }
}
