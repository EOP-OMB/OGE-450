using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace OGC.Form450.API
{
    public class Form450Authorize : AuthorizeAttribute
    {
        private readonly string _configKey;

        public Form450Authorize(string configKey)
        {
            _configKey = configKey;
        }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            // Will be read from configuration
            var requireAuthorization = true;

            // Skip authorization if
            // (1) Found the specified key in app settings
            // (2) Could parse app setting value into a boolean
            // (3) App setting value is set to FALSE
            var skipAuthorization =
                ConfigurationManager.AppSettings[_configKey] != null
                && bool.TryParse(ConfigurationManager.AppSettings[_configKey],
                                 out requireAuthorization)
                && !requireAuthorization;

            return skipAuthorization ? true : base.IsAuthorized(actionContext);
        }
    }
}