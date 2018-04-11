using System;
using System.Linq;
using Microsoft.SharePoint.Client;
using OMB.SharePoint.Infrastructure;
using OMB.SharePoint.Infrastructure.LDAP;

namespace OGC.Data.SharePoint.Models
{
    public class SecurityGroupMapping : SPListBase<SecurityGroupMapping>, ISPList
    {
        #region properties
        public string Abbreviation { get; set; }
        public string Groups { get; set; }
        public string Office { get; set; }
        public string ExcludeOffice { get; set; }
        public DateTime InitialFiling { get; set; }
        public DateTime? DueDate { get; set; }
        public int UserCount { get; set; }

        public string Status { get; set; }
        public string Notes { get; set; }
        #endregion

        public SecurityGroupMapping()
        {
            this.ListName = this.GetType().Name;
        }

        public override void MapToList(ListItem dest)
        {
            base.MapToList(dest);

            dest["Abbreviation"] = Abbreviation;
            dest["Groups"] = Groups;
            dest["Office"] = Office;
            dest["ExcludeOffice"] = ExcludeOffice;
            dest["InitialFiling"] = SharePointHelper.ToDateTimeNullIfMin(InitialFiling);
            dest["DueDate"] = SharePointHelper.ToDateTimeNullIfMin(DueDate);
            dest["UserCount"] = UserCount;
            dest["Status"] = Status;
            dest["Notes"] = Notes;
        }

        public override void MapFromList(ListItem item, bool includeChildren = false)
        {
            base.MapFromList(item);

            Abbreviation = SharePointHelper.ToStringNullSafe(item["Abbreviation"]);
            Groups = SharePointHelper.ToStringNullSafe(item["Groups"]);
            Office = SharePointHelper.ToStringNullSafe(item["Office"]);
            ExcludeOffice = SharePointHelper.ToStringNullSafe(item["ExcludeOffice"]);
            InitialFiling = Convert.ToDateTime(item["InitialFiling"]);
            DueDate = Convert.ToDateTime(item["DueDate"]);
            UserCount = Convert.ToInt32(item["UserCount"]);
            Status = SharePointHelper.ToStringNullSafe(item["Status"]);
            Notes = SharePointHelper.ToStringNullSafe(item["Notes"]);
        }

        
    }
}

