using System.Web.Http;
using System.Web.Http.Cors;
using System.Configuration;
using Newtonsoft.Json.Serialization;

namespace OGC.Event.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            string origin = ConfigurationManager.AppSettings["corsOrigin"];

            EnableCorsAttribute cors = new EnableCorsAttribute(origin, "*", "*");

            config.EnableCors(cors);

            // Web API configuration and services
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
