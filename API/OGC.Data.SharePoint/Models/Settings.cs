using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.SharePoint.Client;
using OMB.SharePoint.Infrastructure;

namespace OGC.Data.SharePoint.Models
{
    public class Settings : SPListBase<Settings>, ISPList
    {
        #region properties
        public int CurrentFilingYear { get; set; }
        public string SiteUrl { get; set; }
        public string OGCEmail { get; set; }
        public string CcEmail { get; set; }
        public string FormVersion { get; set; }
        public string ReplacesVersion { get; set; }
        public int MinimumGiftValue { get; set; }
        public int TotalGiftValue { get; set; }
        public DateTime AnnualDueDate { get; set; }
        #endregion

        public static bool IN_MAINTENANCE_MODE = false;

        public Settings()
        {
            this.ListName = this.GetType().Name;
        }

        public override void MapToList(ListItem dest)
        {
            base.MapToList(dest);

            dest["CurrentFilingYear"] = CurrentFilingYear;
            dest["SiteUrl"] = SiteUrl;
            dest["OGCEmail"] = OGCEmail;
            dest["CcEmail"] = CcEmail;
            dest["FormVersion"] = FormVersion;
            dest["ReplacesVersion"] = ReplacesVersion;
            dest["MinimumGiftValue"] = MinimumGiftValue;
            dest["TotalGiftValue"] = TotalGiftValue;
            dest["AnnualDueDate"] = AnnualDueDate;
        }

        public override void MapFromList(ListItem item, bool includeChildren = false)
        {
            base.MapFromList(item);

            CurrentFilingYear = Convert.ToInt32(item["CurrentFilingYear"]);

            SiteUrl = SharePointHelper.ToStringNullSafe(item["SiteUrl"]);
            OGCEmail = SharePointHelper.ToStringNullSafe(item["OGCEmail"]);
            CcEmail = SharePointHelper.ToStringNullSafe(item["CcEmail"]);
            FormVersion = SharePointHelper.ToStringNullSafe(item["FormVersion"]);
            ReplacesVersion = SharePointHelper.ToStringNullSafe(item["ReplacesVersion"]);
            MinimumGiftValue = Convert.ToInt32(item["MinimumGiftValue"]);
            TotalGiftValue = Convert.ToInt32(item["TotalGiftValue"]);
            AnnualDueDate = Convert.ToDateTime(item["AnnualDueDate"]);
        }

        public static Dictionary<string, string> GetAppSettings(Dictionary<string, string> dict)
        {
            var settings = Settings.GetAll().FirstOrDefault();

            dict.Add("SiteUrl", settings.SiteUrl);
            dict.Add("SiteEmail", settings.OGCEmail);
            dict.Add("Cc", settings.CcEmail);

            return dict;
        }

        public static Dictionary<string, string> GetAppEmailFieldsDef(Dictionary<string, string> dict)
        {
            dict.Add("[SiteUrl]", "The Site URL from Settings.");
            dict.Add("[SiteEmail]", "The Site Email from Settings.");
            dict.Add("[Cc]", "The CC Email from Settings.");

            return dict;
        }
    }
}
