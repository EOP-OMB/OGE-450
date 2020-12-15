using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

using OGC.Data.SharePoint.Models;

namespace OGC.Training.API.Controllers
{
    [Authorize]
    public class OGEForm450Controller : BaseController
    {

        public const int MIN_ASSETS = 5;
        public const int MIN_LIABILITIES = 2;
        public const int MIN_POSITIONS = 6;
        public const int MIN_AGREEMENTS = 4;
        public const int MIN_GIFTS = 3;

        [HttpGet]
        public IHttpActionResult Get()
        {
            // Get my current filing year form
            var identity = HttpContext.Current.User.Identity as ClaimsIdentity;
            AppUser = UserInfo.GetUser(identity);
            

            var settings = Settings.GetAll().FirstOrDefault();
            var forms = GetMyForms(AppUser);

            var form = forms.Where(x => x.Year == settings.CurrentFilingYear).FirstOrDefault();

            return Json(form, CamelCase);
        }

        private List<OGEForm450> GetMyForms(UserInfo user)
        {
            var list = OGEForm450.GetAllByUser("Filer", user.Id).OrderByDescending(x => x.DueDate).ToList();

            return list;
        }
    }
}
