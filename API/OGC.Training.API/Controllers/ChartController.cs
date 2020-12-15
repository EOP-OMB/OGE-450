using System;
using System.Linq;
using System.Web.Http;

using OGC.Data.SharePoint.Models;
using System.Collections.Generic;
using System.Web;
using System.Security.Claims;
using OGC.Training.API.Models;
using System.Globalization;

namespace OGC.Training.API.Controllers
{
    [Authorize]
    public class ChartController : BaseController
    {
        [HttpGet]
        public IHttpActionResult Get(string a)
        {
            try
            {
                var identity = HttpContext.Current.User.Identity as ClaimsIdentity;
                AppUser = UserInfo.GetUser(identity);

                var values = a.Split('-');
                var year = "";
                if (values.Length > 1)
                {
                    a = values[0];
                    year = values[1];
                }

                

                if (AppUser.IsAdmin || AppUser.IsReviewer)
                {
                    switch (a)
                    {
                        case "training":
                            return GetTrainingChartData(year);
                        case "oge450":
                            return GetOGE450ChartData();
                        case "events":
                            return GetEventsChartData();
                        default:
                            return BadRequest();
                    }
                }
                else
                    return Unauthorized();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        private IHttpActionResult GetTrainingChartData(string year)
        {
            var data = new TrainingChartData();
            int tmp = 0;
            var runForYear = DateTime.Now.Year;

            if (int.TryParse(year, out tmp))
                runForYear = Convert.ToInt32(year);

            var trainings = Data.SharePoint.Models.Training.GetAll().ToList();
            Data.SharePoint.Models.Training.SetInactiveFlag(trainings);

            var employees = Data.SharePoint.Models.Employee.GetAll();

            data.CompletedTraining = trainings.Where(x => x.TrainingType == Data.SharePoint.Models.Constants.TrainingType.ANNUAL && x.Year == runForYear && x.Inactive == false).Count();
            data.TotalEmployees = employees.Where(x => x.Inactive == false && (!x.AccountCreatedDate.HasValue || x.AccountCreatedDate.Value.Year <= runForYear)).Count();

            return Json(data, CamelCase);
        }

        private IHttpActionResult GetOGE450ChartData()
        {
            var data = new OGE450ChartData();

            data.Labels = new List<string>(new string[] { "Not-Started", "Draft", "Missing Information", "Overdue", "Submitted", "Certified" });

            var settings = Settings.GetAll().FirstOrDefault();

            var forms = OGEForm450.GetAllBy("Year", settings.CurrentFilingYear);

            var notStarted = forms.Where(x => x.FormStatus == Constants.FormStatus.NOT_STARTED).Count();
            var draft = forms.Where(x => x.FormStatus == Constants.FormStatus.DRAFT).Count();
            var missingInfo = forms.Where(x => x.FormStatus == Constants.FormStatus.MISSING_INFORMATION).Count();
            var overdue = forms.Where(x => x.IsOverdue == true).Count();
            var submitted = forms.Where(x => x.FormStatus == Constants.FormStatus.SUBMITTED || x.FormStatus == Constants.FormStatus.RE_SUBMITTED).Count();
            var certified = forms.Where(x => x.FormStatus == Constants.FormStatus.CERTIFIED).Count();

            data.Data = new List<int>(new int[] { notStarted, draft, missingInfo, overdue, submitted, certified });

            return Json(data, CamelCase);
        }

        private IHttpActionResult GetEventsChartData()
        {
            var data = new EventsChartData();
            data.Datasets = new List<DataSet>();

            var backgroundColors = new List<string>();
            var borderColors = new List<string>();

            //backgroundColors.Add("#f8f9fa");

            backgroundColors.Add("#337ab7");    // Primary
            backgroundColors.Add("#6c757d");    // Secondary
            backgroundColors.Add("#5cb85c");    // Success
            backgroundColors.Add("#d9534f");    // Error
            backgroundColors.Add("#f0ad4e");    // Warning
            backgroundColors.Add("#5bc0de");    // Info

            //borderColors.Add("#e6e7de");
            borderColors.Add("#236ba7");    // Primary
            borderColors.Add("#5a6268");    // Secondary
            borderColors.Add("#4cae4c");    // Success
            borderColors.Add("#d43f3a");    // Error
            borderColors.Add("#ec971f");    // Warning
            borderColors.Add("#46b8da");    // Info

            var events = EventRequest.GetAll();
            events = events.Where(x => x.Status == Constants.EventRequestStatus.APPROVED).ToList();
            var thisYear = DateTime.Now.Year;
            var tmpEvent = events.OrderBy(x => x.EventStartDate.HasValue ? x.EventStartDate.Value.Year : 9999).FirstOrDefault();
            var historyYear = tmpEvent == null ? thisYear : tmpEvent.EventStartDate.Value.Year;

            data.Labels = new List<string>() { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };


            for (int i = 0; historyYear <= thisYear; i++, historyYear++)
            {
                var set = new DataSet();

                var yearEvents = events.Where(x => (x.EventStartDate.HasValue ? x.EventStartDate.Value.Year : 0) == historyYear).ToList();

                set.Label = historyYear.ToString() + " (" + yearEvents.Count + ")";
                set.Data = new List<int>();
                set.BackgroundColor = backgroundColors[i % 6];
                set.BorderColor = borderColors[i % 6];
                set.Fill = false;

                for (int m = 1; m <= 12; m++)
                {
                    var monthEvents = yearEvents.Where(x => x.EventStartDate.Value.Month == m).ToList();
                    set.Data.Add(monthEvents.Count);
                }

                data.Datasets.Add(set);
            }

            return Json(data, CamelCase);
        }

        //private IHttpActionResult GetEventsChartData()
        //{
        //    var data = new EventsChartData();
        //    data.Datasets = new List<DataSet>();

        //    var today = DateTime.Now;
        //    var date = today.AddMonths(-12);
        //    date = date.AddDays(date.Day * -1 + 1);

        //    data.Labels = new List<string>();

        //    var clearedSet = new DataSet();
        //    clearedSet.Label = "Cleared";
        //    clearedSet.Data = new List<int>();
        //    clearedSet.BackgroundColor = "#5cb85c";
        //    clearedSet.BorderColor = "#4cae4c";
        //    clearedSet.Fill = false;

        //    var totalSet = new DataSet();
        //    totalSet.Label = "Events";
        //    totalSet.Data = new List<int>();
        //    totalSet.BackgroundColor = "#999";
        //    totalSet.BorderColor = "#aaa";
        //    totalSet.Fill = false;

        //    var events = EventRequest.GetAll();
        //    events = events.Where(x => x.EventStartDate.HasValue && x.EventStartDate.Value >= date && x.EventStartDate.Value <= today).ToList();

        //    while (date < today)
        //    {
        //        data.Labels.Add(date.ToString("MMM", CultureInfo.InvariantCulture));

        //        var monthEvents = events.Where(x => x.EventStartDate.Value.Month == date.Month && x.EventStartDate.Value.Year == date.Year).ToList();
        //        var totalInMonth = monthEvents.Count();
        //        var clearedInMonth = monthEvents.Where(x => x.Status == Constants.EventRequestStatus.APPROVED).Count();

        //        totalSet.Data.Add(totalInMonth);
        //        clearedSet.Data.Add(clearedInMonth);

        //        date = date.AddMonths(1);
        //    }

        //    data.Datasets.Add(clearedSet);
        //    data.Datasets.Add(totalSet);

        //    return Json(data, CamelCase);
        //}
    }
}
