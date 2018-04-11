using System;
using System.Configuration;
using Microsoft.SharePoint.Client;
using OMB.SharePoint.Infrastructure;

namespace OGC.Data.SharePoint.Models
{
    public class HelpfulLinks : SPListBase<HelpfulLinks>, ISPList
    {
        #region properties
        public string Url { get; set; }
        #endregion

        public HelpfulLinks()
        {
            this.ListName = this.GetType().Name;
        }

        public override void MapToList(ListItem dest)
        {
            base.MapToList(dest);

            dest["URL"] = Url;
        }

        public override void MapFromList(ListItem item, bool includeChildren = false)
        {
            base.MapFromList(item);

            this.Url = SharePointHelper.ToStringNullSafe(item["URL"]); 
        }
    }
}
