using System;
using System.Configuration;
using Microsoft.SharePoint.Client;
using OMB.SharePoint.Infrastructure;

namespace OGC.Data.SharePoint.Models
{
    public class SupportContacts : SPListBase<SupportContacts>, ISPList
    {
        #region properties
        public string Info { get; set; }
        public int SortOrder { get; set; }
        #endregion

        public SupportContacts()
        {
            this.ListName = this.GetType().Name;
        }

        public override void MapToList(ListItem dest)
        {
            base.MapToList(dest);

            dest["Info"] = Info;
            dest["SortOrder"] = SortOrder;
        }

        public override void MapFromList(ListItem item, bool includeChildren = false)
        {
            base.MapFromList(item);
            
            Info = SharePointHelper.ToStringNullSafe(item["Info"]);
            SortOrder = Convert.ToInt32(item["SortOrder"]);
        }
    }
}
