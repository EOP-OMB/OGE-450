using System;
using System.Web;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Configuration;
using System.Linq;
using OMB.SharePoint.Infrastructure.Models;
using System.Collections;

namespace OMB.SharePoint.Infrastructure.LDAP
{
    public class LDAPHelper
    {
        public delegate void WrappedOpDelegate();

        public static void CatchException(WrappedOpDelegate whatToDo)
        {
            try
            {
                whatToDo();
            }
            catch (Exception)
            {
            }
        }


        public static List<LDAPUser> SearchUsers(string pattern)
        {
            var filter = ConfigurationManager.AppSettings["ldapEmployeeFilter"];

            var provider = new LdapProvider("LDAP://DC=TODO");

            return provider.GetUsers(filter);
        }

        private static void SetupUserLoadProperties(DirectorySearcher ds)
        {
            ds.ReferralChasing = ReferralChasingOption.All;
            ds.PropertiesToLoad.Add("userPrincipalName");
            ds.PropertiesToLoad.Add("extensionAttribute3"); 
            ds.PropertiesToLoad.Add("displayName");
            ds.PropertiesToLoad.Add("employeeType");
            ds.PropertiesToLoad.Add("department");
            ds.PropertiesToLoad.Add("physicalDeliveryOfficeName");
            ds.PropertiesToLoad.Add("title");
            ds.PropertiesToLoad.Add("memberOf");
            ds.PropertiesToLoad.Add("userAccountControl");
            ds.PropertiesToLoad.Add("samaccountname");
            ds.PropertiesToLoad.Add("givenname");
            ds.PropertiesToLoad.Add("sn");
        }

        public static void WriteException(Exception ex, string title)
        {
            var spEx = new Exceptions();

            spEx.Title = title;
            spEx.User = "LdapProvider";
            spEx.Message = ex.Message;
            spEx.InnerException = ex.InnerException == null ? "" : ex.InnerException.ToString();
            spEx.Data = ExtractData(ex.Data);
            spEx.HelpLink = ex.HelpLink;
            spEx.HResult = ex.HResult;
            spEx.Source = ex.Source;
            spEx.StackTrace = ex.StackTrace;

            spEx.Save();
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
