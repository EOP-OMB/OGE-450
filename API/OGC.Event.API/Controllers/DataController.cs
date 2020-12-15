using System;
using System.Web.Http;
using OGC.Event.API.Models;

namespace OGC.Event.API.Controllers
{
    [Authorize]
    public class DataController : BaseController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            try
            {
                var data = OGC.Event.API.Models.Data.GetAll();

                return Json(data, CamelCase);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

    }
}
