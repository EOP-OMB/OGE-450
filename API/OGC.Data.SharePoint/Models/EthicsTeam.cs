using System;
using System.Configuration;
using Microsoft.SharePoint.Client;
using OMB.SharePoint.Infrastructure;
using System.Collections.Generic;
using System.IO;

namespace OGC.Data.SharePoint.Models
{
    public class EthicsTeam : SPListBase<EthicsTeam>, ISPList
    {
        #region Properties
        public string Position { get; set; }
        public string Org { get; set; }
        public string Branch { get; set; }
        public string Email { get; set; }
        public string WorkPhone { get; set; }
        public string CellPhone { get; set; }
        public bool IsUser { get; set; }
        public int SortOrder { get; set; }
        #endregion

        public EthicsTeam()
        {
            this.ListName = "Ethics Team";
        }

        #region Mapping
        public override void MapFromList(ListItem item, bool includeChildren = false)
        {
            base.MapFromList(item);

            Position = SharePointHelper.ToStringNullSafe(item["Position"]);
            Org = SharePointHelper.ToStringNullSafe(item["Org"]);
            Branch = SharePointHelper.ToStringNullSafe(item["Branch"]);
            Email = SharePointHelper.ToStringNullSafe(item["Email"]);
            SortOrder = Convert.ToInt32(item["SortOrder"]);
            WorkPhone = SharePointHelper.ToStringNullSafe(item["WorkPhone"]);
            CellPhone = SharePointHelper.ToStringNullSafe(item["CellPhone"]);
            IsUser = SharePointHelper.ToStringNullSafe(item["IsUser"]) == "True";
        }
        #endregion
    }
}
