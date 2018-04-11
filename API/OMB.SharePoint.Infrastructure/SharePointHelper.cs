using System;
using Microsoft.SharePoint.Client;
using System.Configuration;
using Microsoft.SharePoint.Client.Search.Query;
using System.Collections.Generic;

namespace OMB.SharePoint.Infrastructure
{
    public class SharePointHelper
    {
        public static string UserProfileUrl {
            get
            {
#if DEBUG
                return ConfigurationManager.AppSettings.Get("userProfileUrl"); 
#else
                return Url;
#endif
            }
        }

        public static string Url { get { return ConfigurationManager.AppSettings.Get("siteCollectionUrl"); } }

        public static string SearchUrl { get { return ConfigurationManager.AppSettings.Get("searchUrl"); } }

        public static string SourceId { get { return ConfigurationManager.AppSettings.Get("sourceId"); } }

        public static bool EmailSharePointGroup
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings.Get("emailSharePointGroup"));
            }
        }


        public static string ReviewerGroup
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("reviewerGroup") ?? "";
            }
        }

        public static string AdminGroup
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("adminGroup") ?? ""; 
            }
        }

        public static Web GetWeb(ClientContext ctx)
        {
            Web web;

            //try
            //{
                web = ctx.Web;
                ctx.ExecuteQuery();
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception("Unable to access host web at " + ctx.Url + "." + ex.Message);
            //}

            return web;
        }

        public static Group GetGroup(string groupName)
        {
            using (ClientContext ctx = new ClientContext(Url))
            {
                var web = GetWeb(ctx);

                return GetGroup(ctx, web, groupName);
            }
        }

        public static Group GetGroup(ClientContext ctx, Web web, string groupName)
        {
            var group = web.SiteGroups.GetByName(groupName);

            ctx.Load(group);
            ctx.ExecuteQuery();

            return group;
        }

        public static List GetList(ClientContext ctx, Web web, string listName)
        {
            List list;

            try
            {
                list = web.Lists.GetByTitle(listName);
                ctx.Load(list);
                ctx.ExecuteQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("List " + listName + " not found." + ex.Message);
            }

            return list;
        }

        public static CamlQuery GetAllCaml()
        {
            var caml = new CamlQuery();

            caml.ViewXml = "<Query><OrderBy><FieldRef Name='ID' /></OrderBy></Query>";

            return caml;
        }

        public static CamlQuery GetByIdCaml(int id)
        {
            var caml = new CamlQuery();

            caml.ViewXml = "<View><Query><Where><Eq><FieldRef Name='ID' /><Value Type='Integer'>" + id + "</Value></Eq></Where></Query></View>";

            return caml;
        }

        public static string ToStringNullSafe(object value)
        {
            return (value ?? string.Empty).ToString();
        }

        public static DateTime? ToDateTimeNullIfMin(DateTime? date)
        {
            return date == DateTime.MinValue ? null : (DateTime?)date;
        }

        internal static CamlQuery GetByCaml(string key, int id, bool notEqual = false)
        {
            var caml = new CamlQuery();
            var eq = notEqual ? "Neq" : "Eq";

            caml.ViewXml = "<View><Query><Where><"+eq+"><FieldRef Name='" + key + "' /><Value Type='Number'>" + id + "</Value></" + eq + "></Where></Query></View>";

            return caml;
        }

        internal static CamlQuery GetByCaml(string key, string value, bool notEqual = false)
        {
            var caml = new CamlQuery();
            var eq = notEqual ? "Neq" : "Eq";

            caml.ViewXml = "<View><Query><Where><" + eq + "><FieldRef Name='" + key +"' /><Value Type='Text'>" + value + "</Value></" + eq + "></Where></Query></View>";

            return caml;
        }

        internal static CamlQuery GetByUserCaml(string key, int userId)
        {
            var caml = new CamlQuery();

            caml.ViewXml = "<View><Query><Where><Eq><FieldRef Name='" + key + "' LookupId='True' /><Value Type='User'>" + userId.ToString() + "</Value></Eq></Where></Query></View>";

            return caml;
        }

        public static FieldUserValue GetFieldUser(ClientContext ctx, string filer)
        {
            var userValue = new FieldUserValue();
            
            var user = ctx.Web.EnsureUser(filer);

            ctx.Load(user);

            ctx.ExecuteQuery();

            userValue.LookupId = user.Id;

            return userValue;
        }
    }
}
