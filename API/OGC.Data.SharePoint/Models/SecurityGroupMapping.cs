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

        public static void ResetDiscovery()
        {
            var mapping = SecurityGroupMapping.GetAll();

            foreach (SecurityGroupMapping map in mapping)
            {
                map.DueDate = null;
                map.UserCount = 0;
                map.Status = "";

                map.Save();
            }

            var employees = Employee.GetAllBy("Division", "", true);

            foreach (Employee emp in employees)
            {
                emp.Division = "";

                emp.Save();
            }
        }

        public static void Discovery()
        {
            var mapping = SecurityGroupMapping.GetAll();

            foreach (SecurityGroupMapping map in mapping)
            {
                map.DueDate = map.InitialFiling.AddDays(30);

                DiscoverADGroups(map);

                map.Status = "Discovered";

                map.Save();
            }
        }

        private static void DiscoverADGroups(SecurityGroupMapping map)
        {
            var groups = map.Groups.Split(',');
            
            for (int i = 0; i < groups.Length; i++)
            {
                var group = groups[i].Trim();

                 map.UserCount += DiscoverADGroups(group, map);
            }
        }

        private static int DiscoverADGroups(string group, SecurityGroupMapping map)
        {
            var users = LDAPHelper.SearchUsersByGroup(group);
            var count = 0;

            foreach (LDAPUser user in users)
            {
                if (CheckOffice(user.Office, map.Office))
                {
                    var prefix = "";

                    prefix = "i:0e.t|adfs|";

                    try
                    {
                        var employee = Employee.GetBy("AccountName", prefix + user.UPN);

                        if (!employee.Division.Contains(map.Abbreviation))
                        {
                            if (employee.Division == "N/A")
                                employee.Division = "";

                            if (employee.Division != "")
                                employee.Division += ", ";

                            employee.Division += map.Abbreviation;
                            
                            employee.Save();
                            count++;
                        }
                    }
                    catch (Exception ex)
                    {
                        LDAPHelper.WriteException(ex, "SecurityGroupMapping.DiscoverADGroups : " + prefix + user.UPN);
                    }
                }
            }

            return count;
        }

        private static bool CheckOffice(string userOffice, string mapOffice)
        {
            if (string.IsNullOrEmpty(mapOffice))
                return true;

            if (string.IsNullOrEmpty(userOffice))
                return false;

            var offices = mapOffice.Split(';');

            foreach (string off in offices)
            {
                var checkOff = off.Trim();

                if (checkOff.ToLower() == userOffice.ToLower())
                    return true;
            }

            return false;
        }
    }
}

