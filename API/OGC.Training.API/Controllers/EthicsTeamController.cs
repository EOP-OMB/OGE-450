using System;
using System.Linq;
using System.Web.Http;

using OGC.Data.SharePoint.Models;

namespace OGC.Training.API.Controllers
{
    [Authorize]
    public class EthicsTeamController : BaseController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            var files = EthicsTeam.GetAll();

            return Json(files.OrderBy(x => x.SortOrder), CamelCase);
        }
    }
}