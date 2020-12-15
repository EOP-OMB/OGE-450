using System;
using System.Linq;
using System.Web.Http;

using System.Collections.Generic;
using System.Web;
using System.Security.Claims;
using System.IO;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;

using OGC.Training.API.Models;
using OGC.Data.SharePoint.Models;

namespace OGC.Training.API.Controllers
{
    [Authorize]
    public class TrainingController : BaseController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            try
            {
                var identity = HttpContext.Current.User.Identity as ClaimsIdentity;
                AppUser = UserInfo.GetUser(identity);   
                List<Data.SharePoint.Models.Training> list;

                if (AppUser.IsAdmin || AppUser.IsReviewer)
                    list = Data.SharePoint.Models.Training.GetAll().OrderByDescending(x => x.Year).ThenBy(x => x.TrainingType).ToList();
                else
                    list = GetMyTrainings(AppUser);

                Data.SharePoint.Models.Training.SetInactiveFlag(list);

                return Json(list, CamelCase);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet]
        public IHttpActionResult Get(string a)
        {
            var identity = HttpContext.Current.User.Identity as ClaimsIdentity;
            AppUser = UserInfo.GetUser(identity);
            List<Data.SharePoint.Models.Training> list;

            var values = a.Split('-');
            var year = "";
            if (values.Length > 1)
            {
                year = values[1];
                a = values[0];
            }

            try
            {
                switch (a)
                {
                    case "mytraining":
                        list = GetMyTrainings(AppUser);
                        Data.SharePoint.Models.Training.SetInactiveFlag(list);
                        break;
                    case "missingtrainingreport":
                        return GenerateMissingTrainingReport(year);
                    default:
                        return BadRequest("No such action.");
                }

                return Json(list, CamelCase);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }

        }

        private IHttpActionResult GenerateMissingTrainingReport(string year)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);

            int tmp = 0;
            var currentYear = DateTime.Now.Year;

            if (int.TryParse(year, out tmp))
                currentYear = Convert.ToInt32(year);
            

            var employees = Employee.GetAll().Where(x => x.Inactive == false && (!x.AccountCreatedDate.HasValue || x.AccountCreatedDate.Value.Year <= currentYear)).ToList();

            var trainings = Data.SharePoint.Models.Training.GetAllBy("Year", currentYear);
            var allTrainings = Data.SharePoint.Models.Training.GetAll().Where(x => x.Year <= currentYear);

            employees = employees.Where(x => trainings.Find(train => train.Employee.ToLower() == x.AccountName.ToLower()) == null).ToList();

            var ldapEmployees = Employee.GetEmployeesFromLDAP(employees).ToList();

            DebugLog.Log("test", "training", "ns", "GenerateMissingTrainingReport", "Employee Count: " + ldapEmployees.Count().ToString(), "Info", DateTime.Now);

            writer.Write("Employee, Email, Last Training Date" + Environment.NewLine);
            foreach (Employee emp in employees)
            {
                var lastTrainingDate = "";
                var lastTraining = allTrainings.Where(x => x.Employee == emp.AccountName).OrderByDescending(x => x.DateAndTime).FirstOrDefault();
                
                if (lastTraining != null)
                    lastTrainingDate = lastTraining.DateAndTime.ToString();
                else
                    lastTrainingDate = "N/A";

                var ldapEmp = ldapEmployees.Where(x => x.AccountName.ToLower() == emp.AccountName.ToLower()).FirstOrDefault();
                if (ldapEmp != null)
                    emp.EmailAddress = ldapEmp.EmailAddress;

                writer.Write(string.Format("\"{0}\", {1}, {2}" + Environment.NewLine, emp.DisplayName, emp.EmailAddress, lastTrainingDate));
            }

            writer.Flush();
            stream.Position = 0;
            var filename = currentYear.ToString() + " Missing Training Report.csv";

            return new FileResult(stream, Request, filename, "text/csv");

            //HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            //result.Content = new StreamContent(stream);
            //result.Content.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
            //result.Content.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
            //result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = currentYear.ToString() + " Missing Training Report.csv" };

            //return ResponseMessage(result);
        }

        private List<Data.SharePoint.Models.Training> GetMyTrainings(UserInfo user)
        {
            var list = Data.SharePoint.Models.Training.GetAllByUser("Employee", user.Id).OrderByDescending(x => x.DateAndTime).ToList();

            return list;
        }

        [HttpPut]
        public IHttpActionResult Update(Data.SharePoint.Models.Training item)
        {
            try
            {
                var identity = HttpContext.Current.User.Identity as ClaimsIdentity;
                AppUser = UserInfo.GetUser(identity);

                var result = item.RunBusinessRules(AppUser);
                if (string.IsNullOrEmpty(result))
                {
                    if (AppUser.IsAdmin)
                    {
                        return Json(item.Save(), CamelCase);
                    }
                    else
                        return Unauthorized();
                }
                else
                    return BadRequest(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost]
        public IHttpActionResult Create(Data.SharePoint.Models.Training item)
        {
            try
            {
                var identity = HttpContext.Current.User.Identity as ClaimsIdentity;
                AppUser = UserInfo.GetUser(identity);

                var result = item.RunBusinessRules(AppUser);
                if (string.IsNullOrEmpty(result))
                {
                    return Json(item.Save(), CamelCase);
                }
                else
                    return BadRequest(result);

            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
