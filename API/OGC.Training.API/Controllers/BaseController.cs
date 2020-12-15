using System;
using System.Collections;
using System.Diagnostics;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using OGC.Data.SharePoint.Models;
using OMB.SharePoint.Infrastructure.Models;

namespace OGC.Training.API.Controllers
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
            try
            {
                var spEx = new Exceptions();

                spEx.Title = this.GetType().Name;
                spEx.User = User == null ? "unknown" : AppUser.DisplayName;
                spEx.Message = ex.Message;
                spEx.InnerException = ex.InnerException == null ? "" : ex.InnerException.ToString();
                spEx.Data = ExtractData(ex.Data);
                spEx.HelpLink = ex.HelpLink;
                spEx.HResult = ex.HResult;
                spEx.Source = ex.Source;
                spEx.StackTrace = ex.StackTrace;

                spEx.Save();
            }
            catch(Exception ex2)
            {
                // TODO: Log exception handling exception to file
                EventLog.WriteEntry("OGC.Training.API", ex.Message + "Trace" + ex.StackTrace, EventLogEntryType.Error, 121, short.MaxValue);
            }
            
            return InternalServerError(ex);
        }

        private string ExtractData(IDictionary data)
        {
            var ret = "";

            foreach (DictionaryEntry entry in data)
            {
                ret += string.Format("Key: {0,-20}\tValue: {1}", "'" + entry.Key.ToString() + "'", entry.Value) + "\n";
            }

            return ret;
        }
    }
}
