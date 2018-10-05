using System;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

using OGC.Data.SharePoint.Models;
using System.Collections.Generic;
using OGC.Data.SharePoint;

namespace OGC.Form450.API.Controllers
{
    [Form450Authorize("requireAuthorization")]
    public class EmployeesController : BaseController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            try
            {
                var identity = HttpContext.Current.User.Identity as ClaimsIdentity;
                OGE450User = UserInfo.GetUser(identity);

                if (OGE450User.IsAdmin)
                {
                    var employees = Employee.GetUsers();

                    return Json(employees.OrderBy(x => x.DisplayName), CamelCase);
                }
                else
                {
                    return Unauthorized();
                }
                
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        
        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            try
            {
                var identity = HttpContext.Current.User.Identity as ClaimsIdentity;
                OGE450User = UserInfo.GetUser(identity);

                if (OGE450User.IsAdmin)
                {
                    var employee = Employee.GetUser(id);

                    var dict = new Dictionary<string, string>();
                    dict = Settings.GetAppSettings(dict);

                    employee.NewEntrantEmailText = EmailHelper.GetEmailText(NotificationTemplates.NotificationTypes.OGE_FORM_450_NEW_ENTRANT, dict);
                    employee.AnnualEmailText = EmailHelper.GetEmailText(NotificationTemplates.NotificationTypes.OGE_FORM_450_NEW_ANNUAL, dict);

                    return Json(employee, CamelCase);
                }
                else
                {
                    return Unauthorized();
                }

            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut]
        public IHttpActionResult Update(Employee item)
        {
            try
            {
                var identity = HttpContext.Current.User.Identity as ClaimsIdentity;
                OGE450User = UserInfo.GetUser(identity);

                if (OGE450User.IsAdmin)
                {
                    if (item.Inactive || item.EmployeeStatus == Constants.EmployeeStatus.INACTIVE)
                        item.Deactivate();

                    var oldItem = Employee.Get(item.Id);

                    oldItem.FilerType = item.FilerType;
                    oldItem.AppointmentDate = item.AppointmentDate;
                    oldItem.ReportingStatus = item.ReportingStatus;
                    oldItem.GenerateForm = item.GenerateForm;
                    oldItem.EmployeeStatus = item.EmployeeStatus;
                    oldItem.Inactive = item.Inactive || item.EmployeeStatus == Constants.EmployeeStatus.INACTIVE;
                    oldItem.InactiveDate = item.InactiveDate;

                    var emp = oldItem.Save();

                    if (oldItem.GenerateForm && !oldItem.Inactive)
                    {
                        var form = OGEForm450.Create(item);

                        form.ProcessEmails();
                    }

                    return Json(emp, CamelCase);
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
