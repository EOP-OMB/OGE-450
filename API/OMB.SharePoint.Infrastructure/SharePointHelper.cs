using System;
using Microsoft.SharePoint.Client;
using System.Configuration;
using Microsoft.SharePoint.Client.Search.Query;
using System.Collections.Generic;
using OMB.SharePoint.Infrastructure.Models;
using System.Collections;

namespace OMB.SharePoint.Infrastructure
{
    public class SharePointHelper
    {
        public static string UserProfileUrl
        {
            get
            {
                return Url;
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

        public static string RestrictedAdmin
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("restrictedAdmin") ?? "";
            }
        }

        public static string MigrateUrl { get { return ConfigurationManager.AppSettings.Get("migrationUrl"); } }

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

        internal static CamlQuery GetAllJoinCaml(string joinList, string joinField, string[] projectedFields)
        {
            var caml = new CamlQuery();

            var query = "<View><Query/><ProjectedFields>";


            foreach (string field in projectedFields)
            {
                query += String.Format("<Field Name='{0}_{1}' Type='Lookup' List='{0}' ShowField='{1}' />", joinList, field);
            }

            query += "</ProjectedFields><Joins><Join Type='INNER' ListAlias='{0}'><Eq><FieldRef Name='{1}' RefType='Id'/><FieldRef List='{0}' Name='ID'/></Eq></Join></Joins></View>";

            query = String.Format(query, joinList, joinField);

            caml.ViewXml = query;

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

        public static CamlQuery GetByCaml(string key, int id, bool notEqual = false)
        {
            var caml = new CamlQuery();
            var eq = notEqual ? "Neq" : "Eq";

            caml.ViewXml = "<View Scope='RecursiveAll'><Query><Where><" + eq + "><FieldRef Name='" + key + "' /><Value Type='Number'>" + id + "</Value></" + eq + "></Where></Query></View>";

            return caml;
        }

        public static CamlQuery GetByCaml(string key, string value, bool notEqual = false)
        {
            var caml = new CamlQuery();
            var eq = notEqual ? "Neq" : "Eq";

            caml.ViewXml = "<View Scope='RecursiveAll'><Query><Where><" + eq + "><FieldRef Name='" + key + "' /><Value Type='Text'>" + value + "</Value></" + eq + "></Where></Query></View>";

            return caml;
        }

        public static CamlQuery GetByUserCaml(string key, int userId)
        {
            var caml = new CamlQuery();

            caml.ViewXml = "<View><Query><Where><Eq><FieldRef Name='" + key + "' LookupId='True' /><Value Type='User'>" + userId.ToString() + "</Value></Eq></Where></Query></View>";

            return caml;
        }

        public static FieldUserValue GetFieldUser(string upn)
        {
            var userValue = new FieldUserValue();

            try
            {
                var user = EnsureUser(upn);

                userValue.LookupId = user.Id;
            }
            catch (Exception ex)
            {
                userValue = null;
            }

            return userValue;
        }

        public static User EnsureUser(string upn)
        {
            using (ClientContext ctx = new ClientContext(Url))
            {
                var user = ctx.Web.EnsureUser(upn);

                ctx.Load(user);

                ctx.ExecuteQuery();

                return user;
            }
        }

        public static void HandleException(Exception ex, string user, string title)
        {
            try
            {
                var spEx = new Exceptions();

                spEx.Title = title;
                spEx.User = user;
                spEx.Message = ex.Message;
                spEx.InnerException = ex.InnerException == null ? "" : ex.InnerException.ToString();
                spEx.Data = ExtractData(ex.Data);
                spEx.HelpLink = ex.HelpLink;
                spEx.HResult = ex.HResult;
                spEx.Source = ex.Source;
                spEx.StackTrace = ex.StackTrace;

                spEx.Save();
            }
            catch (Exception ex2)
            {
                // TODO: Log exception handling exception to file

            }
        }

        private static string ExtractData(IDictionary data)
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
