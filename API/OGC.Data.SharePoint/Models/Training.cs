using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.SharePoint.Client;
using OMB.SharePoint.Infrastructure;

namespace OGC.Data.SharePoint.Models
{
    public class Training : SPListBase<Training>, ISPList
    {
        #region properties
        public DateTime DateAndTime { get; set; }
        public string Location { get; set; }
        public string EthicsOfficial { get; set; }
        public string Employee { get; set; }
        public string EmployeesName { get; set; }
        public int Year { get; set; }
        public string TrainingType { get; set; }
        public string Division { get; set; }
        #endregion

        public Training()
        {
            this.ListName = this.GetType().Name;
        }

        public override void MapToList(ListItem dest)
        {
            var userProfile = UserProfileHelper.GetUserProfile(this.Employee);

            if (Id == 0)
                dest["Employee"] = SharePointHelper.GetFieldUser(SPContext, this.Employee);

            base.MapToList(dest);
            
            dest["EmployeesName"] = userProfile.DisplayName;
            dest["DateAndTime"] = SharePointHelper.ToDateTimeNullIfMin(DateAndTime);
            dest["Location"] = Location;
            dest["EthicsOfficial"] = EthicsOfficial;
            dest["Year"] = Year;
            dest["TrainingType"] = TrainingType;
            dest["Division"] = Division;
        }

        public override void MapFromList(ListItem item, bool includeChildren = false)
        {
            base.MapFromList(item);

            DateAndTime = Convert.ToDateTime(item["DateAndTime"]);
            Location = SharePointHelper.ToStringNullSafe(item["Location"]);
            EthicsOfficial = SharePointHelper.ToStringNullSafe(item["EthicsOfficial"]);
            Employee = ((FieldUserValue)item["Employee"]).LookupValue;
            EmployeesName = SharePointHelper.ToStringNullSafe(item["EmployeesName"]);
            TrainingType = SharePointHelper.ToStringNullSafe(item["TrainingType"]);
            Division = SharePointHelper.ToStringNullSafe(item["Division"]);
            Year = Convert.ToInt32(item["Year"]);
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
            return dict;
        }

        public string RunBusinessRules(UserInfo user)
        {
            // set Year
            if (Year == 0)
                Year = DateAndTime.Year;

            // This shouldn't happen unless someone hacks the json
            if (Year != DateAndTime.Year)
                return "Date and time year must match training year.";

            // If Id = 0, set the employee to user
            this.Employee = user.Upn;

            // Ensure filing
            // Only 1 annual per year
            // Only 1 Initial ever
            var trainings = GetAllByUser("Employee", user.Id);
            var checkList = trainings.Where(x => x.TrainingType == TrainingType && (TrainingType == "Initial" || x.Year == Year)).ToList();

            if (checkList.Count > 0)
            {
                if (TrainingType == "Annual")
                    return "Cannot create two annual trainings for the same year.";
                else
                    return "Cannot create more than one initial training.";
            }
            else
                return "";
        }
    }
}
