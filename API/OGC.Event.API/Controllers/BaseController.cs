using System;
using System.Collections;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using OGC.Data.SharePoint.Models;
using OMB.SharePoint.Infrastructure;

namespace OGC.Event.API.Controllers
{
    public class BaseController : ApiController
    {
        public UserInfo AppUser { get; set; }

        public JsonSerializerSettings CamelCase
        {
            get
            {
                return new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };
            }
        }

        internal IHttpActionResult HandleException(Exception ex)
        {
            var user = AppUser == null ? "unknown" : AppUser.DisplayName;

            SharePointHelper.HandleException(ex, user, this.GetType().Name);
            
            return InternalServerError(ex);
        }
    }
}
