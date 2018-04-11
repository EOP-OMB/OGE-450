using System;
using System.Configuration;
using Microsoft.SharePoint.Client;
using OMB.SharePoint.Infrastructure;

namespace OGC.Data.SharePoint.Models
{
    public class StaffingReport : SPListBase<StaffingReport>, ISPList
    {
        #region Properties
        public string Division { get; set; }
        public string DivisionCode { get; set; }
        public string FullName { get; set; }
        public string PayPlan { get; set; }
        public string PositionTitle { get; set; }
        public string AppointmentType { get; set; }
        public string Upn { get; set; }
        #endregion  

        public StaffingReport()
        {
            this.ListName = this.GetType().Name;
        }

        public override void MapFromList(ListItem item, bool includeChildren = false)
        {
            base.MapFromList(item);

            this.Division = Title;
            this.DivisionCode = SharePointHelper.ToStringNullSafe(item["DivisionCode"]);
            this.FullName = SharePointHelper.ToStringNullSafe(item["FullName"]);
            this.PayPlan = SharePointHelper.ToStringNullSafe(item["PayPlan"]);
            this.PositionTitle = SharePointHelper.ToStringNullSafe(item["PositionTitle"]);
            this.AppointmentType = SharePointHelper.ToStringNullSafe(item["AppointmentType"]);
            this.Upn = SharePointHelper.ToStringNullSafe(item["Upn"]);
        }

        public override void MapToList(ListItem dest)
        {
            base.MapToList(dest);
            
            dest["DivisionCode"] = this.DivisionCode;
            dest["FullName"] = this.FullName;
            dest["PayPlan"] = this.PayPlan;
            dest["PositionTitle"] = this.PositionTitle;
            dest["AppointmentType"] = this.AppointmentType;
            dest["Upn"] = this.Upn;
        }
    }
}
