using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using OGC.Data.SharePoint.Models;
using OGC.Form450.API.Models;
using System.Configuration;

namespace OGC.Form450.API.Controllers
{
    public class HealthCheckController : BaseController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            var checks = new List<HealthCheck>();

            checks.Add(CheckAPI());
            checks.Add(CheckSharePoint());
            checks.Add(CheckADFS());

            return Json(checks, CamelCase);
        }

        private HealthCheck CheckADFS()
        {
            var health = new HealthCheck();

            try
            {
                health.description = "Check ADFS availability";

                var url = ConfigurationManager.AppSettings["ida:AdfsMetadataEndpoint"];
                var request = HttpWebRequest.Create(url);
                var response = (System.Net.HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                    health.status = "Success";
                else
                    health.status = "Error";
            }
            catch(Exception ex)
            {
                health.status = "Error";
            }
            

            return health;
        }

        private HealthCheck CheckSharePoint()
        {
            var health = new HealthCheck();

            health.description = "SharePoint List Access";

            try
            {
                var links = HelpfulLinks.GetAll();

                health.status = "Success";
            }
            catch (Exception ex)
            {
                health.status = "Error";
            }

            return health;
        }

        private HealthCheck CheckAPI()
        {
            var health = new HealthCheck();

            health.description = "OGE Form 450 API";
            health.status = "Success";

            return health;
        }
    }
}
