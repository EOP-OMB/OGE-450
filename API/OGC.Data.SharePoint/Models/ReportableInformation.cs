using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using OMB.SharePoint.Infrastructure;

namespace OGC.Data.SharePoint.Models
{
    public class ReportableInformation : SPListBaseYear<ReportableInformation>, ISPList
    {
        #region Properties
        public int OGEForm450Id { get; set; }
        public string InfoType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string AdditionalInfo { get; set; }
        public bool NoLongerHeld { get; set; }
        public string AppUser { get; set; }
        public string CorrelationId { get; set; }
        #endregion

        public ReportableInformation()
        {
            this.ListName = this.GetType().Name;
        }

        public override void MapToList(ListItem dest)
        {
            base.MapToList(dest);

            var flv = new FieldLookupValue();
            flv.LookupId = OGEForm450Id;

            dest["Title"] = "Title"; // Title isn't used, but is required, so overwrite it
            dest["OGEForm450Id"] = flv;
            dest["InfoType"] = InfoType;
            dest["Name"] = Name;
            dest["Description"] = Description;
            dest["AdditionalInfo"] = AdditionalInfo;
            dest["NoLongerHeld"] = NoLongerHeld;
            dest["AppUser"] = AppUser;
            dest["CorrelationId"] = CorrelationId;
        }

        public override void MapFromList(ListItem item, bool includeChildren = false)
        {
            base.MapFromList(item);

            OGEForm450Id = ((FieldLookupValue)item["OGEForm450Id"]).LookupId;
            InfoType = SharePointHelper.ToStringNullSafe(item["InfoType"]);
            Name = SharePointHelper.ToStringNullSafe(item["Name"]);
            Description = SharePointHelper.ToStringNullSafe(item["Description"]);
            AdditionalInfo = SharePointHelper.ToStringNullSafe(item["AdditionalInfo"]);
            NoLongerHeld = Convert.ToBoolean(item["NoLongerHeld"]);
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(AdditionalInfo) && string.IsNullOrEmpty(Name) && string.IsNullOrEmpty(Description);
        }
    }
}
