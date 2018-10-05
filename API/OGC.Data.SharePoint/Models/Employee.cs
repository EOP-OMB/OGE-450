using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.SharePoint.Client;
using OMB.SharePoint.Infrastructure;
using System.Security.Principal;
using System.Threading;

namespace OGC.Data.SharePoint.Models
{
    public class Employee : SPListBase<Employee>, ISPList
    {
        public Employee()
        {
            this.ListName = "Employees";
        }

        // List Columns
        public string AccountName { get; set; } // FK to UPS
        public string EmployeeStatus { get; set; }
        public bool Inactive { get; set; }
        public DateTime? InactiveDate { get; set; }
        public string FilerType { get; set; }
        public string ReportingStatus { get; set; }
        public DateTime? Last450Date { get; set; }
        public DateTime? AppointmentDate { get; set; } // HireDate?
        public string Division { get; set; }

        // Other Columns
        public int CurrentFormId { get; set; }
        public string CurrentFormStatus { get; set; }
        public DateTime? DueDate { get; set; }
        public bool GenerateForm { get; set; }
        public string Notes { get; set; }

        // From User Profile Service
        public string Position { get; set; }    // Title
        public string EmailAddress { get; set; }
        public string DisplayName { get; set; }
        public string WorkPhone { get; set; }
        public string Agency { get; set; }  // Department
        public string Branch { get; set; } // Office
        public string ProfileUrl { get; set; }
        public string PictureUrl { get; set; }

        public string NewEntrantEmailText { get; set; }
        public string AnnualEmailText { get; set; }

        public override void MapFromList(ListItem item, bool includeChildren = false)
        {
            base.MapFromList(item);

            AccountName = SharePointHelper.ToStringNullSafe(item["AccountName"]);
            DisplayName = SharePointHelper.ToStringNullSafe(item["Title"]);
            Inactive = Convert.ToBoolean(item["Inactive"]);
            FilerType = SharePointHelper.ToStringNullSafe(item["FilerType"]);

            ReportingStatus = SharePointHelper.ToStringNullSafe(item["ReportingStatus"]);
            Last450Date = SharePointHelper.ToDateTimeNullIfMin(Convert.ToDateTime(item["Last450Date"]));
            AppointmentDate = SharePointHelper.ToDateTimeNullIfMin(Convert.ToDateTime(item["AppointmentDate"]));

            Division = SharePointHelper.ToStringNullSafe(item["Division"]);
            EmployeeStatus = SharePointHelper.ToStringNullSafe(item["EmployeeStatus"]);

            if (Division == "")
                Division = "N/A";
        }

        public override void MapToList(ListItem dest)
        {
            base.MapToList(dest);

            dest["AccountName"] = this.AccountName;
            dest["Inactive"] = this.Inactive;
            dest["FilerType"] = this.FilerType;

            dest["ReportingStatus"] = this.ReportingStatus;
            dest["Last450Date"] = SharePointHelper.ToDateTimeNullIfMin(this.Last450Date);
            dest["InactiveDate"] = SharePointHelper.ToDateTimeNullIfMin(this.InactiveDate);
            dest["AppointmentDate"] = SharePointHelper.ToDateTimeNullIfMin(this.AppointmentDate);
            dest["Division"] = Division;

            dest["EmployeeStatus"] = EmployeeStatus;
        }

        public static void UpdateNotStartedFormsAddProfileData()
        {
            var list = OGEForm450.GetAll().Where(x => x.FormStatus == Constants.FormStatus.NOT_STARTED).ToList();


            foreach (OGEForm450 form in list)
            {
                var emp = Get(form.Filer);
                GetEmployeeUserProfileInfo(emp);

                form.Agency = emp.Agency;
                form.BranchUnitAndAddress = emp.Branch;
                form.EmailAddress = emp.EmailAddress;
                form.PositionTitle = emp.Position;
                form.WorkPhone = emp.WorkPhone;

                form.Save();
            }
        }

        public static void SyncEmployeeStatus()
        {
            var list = Employee.GetAll();

            foreach (Employee emp in list)
            {
                emp.EmployeeStatus = emp.Inactive ? Constants.EmployeeStatus.INACTIVE : Constants.EmployeeStatus.ACTIVE;

                emp.Save();
            }
        }

        public static void ResetFilerType()
        {
            var list = Employee.GetAll();

            foreach (Employee emp in list)
            {
                if (emp.FilerType == "")
                {
                    emp.FilerType = Constants.FilerType.NOT_ASSIGNED;
                    emp.Save();
                }
            }
        }

        public static List<Employee> GetUsers(string filter = "")
        {
            var list = Employee.GetAll();
            var settings = Settings.GetAll().FirstOrDefault();
            var forms = OGEForm450.GetAllBy("Year", settings.CurrentFilingYear);

            foreach (Employee emp in list)
            {
                var f = forms.Where(x => x.Filer.ToLower() == emp.AccountName.ToLower() && x.FormStatus != Constants.FormStatus.CANCELED).FirstOrDefault();

                emp.GenerateForm = false;

                emp.CurrentFormId = f != null ? f.Id : 0;
                emp.CurrentFormStatus = f != null ? f.FormStatus : "Not Available";
            }

            return list;
        }

        public static Employee GetUser(int id)
        {
            var list = new List<Employee>();

            var employee = Employee.Get(id);
            var form = UserInfo.GetUserForm(employee.AccountName);

            employee.GenerateForm = false;
            employee.CurrentFormId = form != null ? form.Id : 0;
            employee.CurrentFormStatus = form != null ? form.FormStatus : "Not Available";

            GetEmployeeUserProfileInfo(employee);

            return employee;
        }

        public static void GetEmployeeUserProfileInfo(Employee emp)
        {
            var userProfile = UserProfileHelper.GetUserProfile(emp.AccountName);

            if (userProfile != null)
            {
                emp.Title = userProfile.DisplayName;
                emp.DisplayName = userProfile.DisplayName;
                emp.EmailAddress = userProfile.Email;
                emp.Position = userProfile.IsPropertyAvailable("Title") ? userProfile.Title : string.Empty;
                emp.WorkPhone = userProfile.IsPropertyAvailable("UserProfileProperties") && userProfile.UserProfileProperties.ContainsKey("WorkPhone") ? userProfile.UserProfileProperties["WorkPhone"] : string.Empty;
                emp.Agency = userProfile.IsPropertyAvailable("UserProfileProperties") && userProfile.UserProfileProperties.ContainsKey("Department") ? userProfile.UserProfileProperties["Department"] : string.Empty;
                emp.Branch = userProfile.IsPropertyAvailable("UserProfileProperties") && userProfile.UserProfileProperties.ContainsKey("Office") ? userProfile.UserProfileProperties["Office"] : string.Empty;
                emp.ProfileUrl = userProfile.UserUrl;
                emp.PictureUrl = userProfile.PictureUrl;
            }
        }

        public static void FormCertified(OGEForm450 form)
        {
            var emp = Employee.Get(form.Filer);

            emp.Last450Date = form.DateOfReviewerSignature;
            emp.ReportingStatus = Constants.ReportingStatus.ANNUAL;

            emp.Save();
        }

        public static Employee Get(string filer)
        {
            return GetBy("AccountName", filer);
        }

        public static void SyncUserProfilesToEmployees()
        {
            var ldapUsers = UserProfileHelper.Query();

            var info = "ldapUsers.Count: " + ldapUsers.Count.ToString();

            DebugLog.Log("SyncUserProfilesToEmployees", "OGE450", "OOGC.Data.SharePoint.Employee", "SyncUserProfilesToEmployees", info, "Info", DateTime.Now);

            var knownEmployees = Employee.GetAll();
            var prefix = "";

            info = "knownEmployees.Count: " + knownEmployees.Count.ToString();
            DebugLog.Log("SyncUserProfilesToEmployees", "OGE450", "OOGC.Data.SharePoint.Employee", "SyncUserProfilesToEmployees", info, "Info", DateTime.Now);

            prefix = "i:0e.t|adfs|";

            // Find all new employees or employees who are now inactive
            var result =
                from ldap in ldapUsers
                join emp in knownEmployees on prefix + ldap.UPN.ToLower() equals emp.AccountName.ToLower() into emps
                from emp in emps.DefaultIfEmpty()
                where emp == null || ldap.Inactive != emp.Inactive
                select new Employee()
                {
                    Id = emp == null ? 0 : emp.Id,
                    AccountName = prefix + ldap.UPN,
                    Title = ldap.DisplayName,
                    Inactive = ldap.Inactive,
                    EmailAddress = ldap.Email,
                    EmployeeStatus = ldap.Inactive ? Constants.EmployeeStatus.INACTIVE : Constants.EmployeeStatus.ACTIVE
                };

            if (result != null)
            {
                var employees = result.ToList();

                info = "employees.Count: " + employees.Count.ToString();
                DebugLog.Log("SyncUserProfilesToEmployees", "OGE450", "OOGC.Data.SharePoint.Employee", "SyncUserProfilesToEmployees", info, "Info", DateTime.Now);

                foreach (Employee emp in employees)
                {
                    if (emp.Inactive)
                    {
                        emp.Deactivate();
                    }

                    if (emp.Id == 0)
                        emp.FilerType = Constants.FilerType.NOT_ASSIGNED;

                    emp.Save();
                }
            }

            knownEmployees = knownEmployees.Where(x => x.Inactive == false).ToList();

            // Find all employees who are no longer found within the ldap query 
            result = from emp in knownEmployees
                     join ldap in ldapUsers on emp.AccountName.ToLower() equals prefix + ldap.UPN.ToLower() into ldaps
                     from ldap in ldaps.DefaultIfEmpty()
                     where ldap == null
                     select emp;

            if (result != null)
            {
                var employees = result.ToList();

                foreach (Employee emp in employees)
                {
                    // known employee no longer found.
                    emp.Deactivate();

                    emp.Save();
                }
            }
        }


        public static IEnumerable<Employee> GetEmployeesFromLDAP(List<Employee> employees)
        {
            var ldapUsers = UserProfileHelper.Query();

            var prefix = "i:0e.t|adfs|";

            var result =
                from ldap in ldapUsers
                join emp in employees on prefix + ldap.UPN.ToLower() equals emp.AccountName.ToLower() into emps
                from emp in emps.DefaultIfEmpty()
                select new Employee()
                {
                    Id = emp == null ? 0 : emp.Id,
                    AccountName = prefix + ldap.UPN,
                    Title = ldap.DisplayName,
                    Inactive = ldap.Inactive,
                    EmailAddress = ldap.Email
                };

            return result;
        }

        public static void UpdateFilerTypes()
        {
            var employees = Employee.GetAll();
            var stafflist = StaffingReport.GetAll();

            foreach (Employee emp in employees)
            {
                var ses = stafflist.Any(x => emp.AccountName.ToLower().EndsWith(x.Upn.ToLower()));

                if (string.IsNullOrEmpty(emp.FilerType) || emp.FilerType == Constants.FilerType.NOT_ASSIGNED)
                {
                    if (ses)
                        emp.FilerType = Constants.FilerType._278_FILER;
                    else if (emp.Inactive)
                        emp.FilerType = Constants.FilerType.NON_FILER;
                    else
                        emp.FilerType = Constants.FilerType._450_FILER;

                    emp.Save();
                }
            }
        }

        public void Deactivate()
        {
            try
            {
                if (this.CurrentFormId == 0)
                {
                    var f = OGEForm450.GetCurrentFormByUser(AccountName);

                    CurrentFormId = f != null ? f.Id : 0;
                }

                //  Set Current Form to Canceled
                if (this.CurrentFormId > 0)
                {
                    var form = OGEForm450.Get(this.CurrentFormId);

                    form.FormStatus = Constants.FormStatus.CANCELED;

                    form.Save();

                    form.RemoveExtensions();
                }

                this.Inactive = true;
                this.InactiveDate = DateTime.Now;
                this.EmployeeStatus = Constants.EmployeeStatus.INACTIVE;
            }
            catch (Exception ex)
            {
                // Couldn't find form, ignore exception
                SharePointHelper.HandleException(ex, "", "Employee.Deactivate");
            }
        }
    }
}