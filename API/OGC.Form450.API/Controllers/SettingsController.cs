using System;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

using OGC.Data.SharePoint.Models;
using System.Threading;

namespace OGC.Form450.API.Controllers
{
    [Form450Authorize("requireAuthorization")]
    public class SettingsController : BaseController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            try
            {
                var settings = Settings.GetAll().FirstOrDefault();

                return Json(settings, CamelCase);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet]
        public IHttpActionResult Get(string a)
        {
            try
            {
                if (a.ToLower() == "annual")
                {
                    var t = new Thread(StartAnnualFiling);

                    t.Start();

                    Settings.IN_MAINTENANCE_MODE = true;

                    var settings = Settings.GetAll().FirstOrDefault();

                    return Json(settings, CamelCase);
                }
                else
                {
                    return BadRequest("No such action.");
                }
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        private void StartAnnualFiling()
        {
            var settings = Settings.GetAll().FirstOrDefault();
            var expiredForms = OGEForm450.GetAllBy("Year", settings.CurrentFilingYear);
            expiredForms = expiredForms.Where(x => x.FormStatus != Constants.FormStatus.CERTIFIED && x.FormStatus != Constants.FormStatus.CANCELED).ToList();

            // Any incomplete filing for settings.currentFilingYear filing year will be marked as 'expired'
            foreach (OGEForm450 form in expiredForms)
            {
                form.FormStatus = form.FormStatus + " - " + Constants.FormStatus.EXPIRED;
                form.Save();
            }

            // The filing year will move to settings.currentFilingYear + 1 
            settings.CurrentFilingYear += 1;
            settings.AnnualDueDate = settings.AnnualDueDate.AddYears(1);
            settings.Save();

            // All employees with a filer type of '450 Filer' who are not 'On Leave' or 'On Detail' will be assigned a new 'Annual' OGE Form 450
            // New forms will be a copy of the previous filing year's form if applicable
            var emps = Employee.GetAllBy("FilerType", "450 Filer");
            emps = emps.Where(x => x.EmployeeStatus != Constants.EmployeeStatus.ON_DETAIL && x.EmployeeStatus != Constants.EmployeeStatus.ON_LEAVE && x.Inactive == false).ToList();

            foreach (Employee emp in emps)
            {
                if (emp.ReportingStatus != Constants.ReportingStatus.ANNUAL)
                {
                    emp.ReportingStatus = Constants.ReportingStatus.ANNUAL;
                    emp.Save();
                }

                CreateFormForEmployee(emp, settings);
            }

            var email = new Notifications();
            email.Title = "Annual Filing Complete";
            email.Subject = "OGE Form 450: Annual Filing Initialization - COMPLETE";
            email.Body = "The OGE Form 450 Annual Filing Initiation process has completed.  Access to the application has been restored.";
            email.Recipient = settings.CcEmail;
            email.Status = "Pending";
            email.Application = "OGE450";
            email.Save();
            Settings.IN_MAINTENANCE_MODE = false;
        }

        private void CreateFormForEmployee(Employee emp, Settings settings)
        {
            try
            {
                Employee.GetEmployeeUserProfileInfo(emp);

                if (emp != null && !string.IsNullOrEmpty(emp.AccountName))
                {
                    var previousForm = OGEForm450.GetPreviousFormByUser(emp.AccountName, settings);
                    OGEForm450 form;

                    if (previousForm == null)
                        form = OGEForm450.Create(emp);
                    else
                        form = OGEForm450.Create(emp, previousForm, settings);

                    form.ProcessEmails();
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        [HttpPut]
        public IHttpActionResult Update(Settings item)
        {
            try
            {
                var identity = HttpContext.Current.User.Identity as ClaimsIdentity;
                OGE450User = UserInfo.GetUser(identity);

                if (OGE450User.IsAdmin)
                {
                    return Json(item.Save(), CamelCase);
                }
                else
                    return Unauthorized();
                
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
