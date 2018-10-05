using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.UserProfiles;

using System.Net;

namespace OMB.SharePoint.Infrastructure
{
    public class UserProfileHelper
    {

        public static PersonProperties GetUserProfile(string loginName)
        {
            if (string.IsNullOrEmpty(loginName))
                return null;

            using (var context = new ClientContext(SharePointHelper.UserProfileUrl))
            {
                var web = context.Web;
                var peopleManager = new PeopleManager(context);
                PersonProperties userProfile = null;

                if (peopleManager != null)
                {
                    userProfile = peopleManager.GetPropertiesFor(loginName);
                    context.Load(userProfile);

                    context.ExecuteQuery();
                }

                return userProfile;
            }
        }

        public static List<PersonProperties> GetUserProfiles(string filter = "")
        {
            var userProfilesResult = new List<PersonProperties>();
            var returnList = new List<PersonProperties>();

            using (var context = new ClientContext(SharePointHelper.UserProfileUrl))
            {
                var web = context.Web;
                var peopleManager = new PeopleManager(context);
               
                var siteUsers = from user in web.SiteUsers
                                where user.PrincipalType == Microsoft.SharePoint.Client.Utilities.PrincipalType.User
                                select user;

                var usersResult = context.LoadQuery(siteUsers);
                context.ExecuteQuery();

                foreach (var user in usersResult)
                {
                    if (user.Title.ToLower().Contains(filter.ToLower()))
                    {
                        
                        var userProfile = peopleManager.GetPropertiesFor(user.LoginName);
                        context.Load(userProfile);

                        userProfilesResult.Add(userProfile);
                    }
                }
                context.ExecuteQuery();

                userProfilesResult = userProfilesResult.Where(x => x.ServerObjectIsNull != null && x.ServerObjectIsNull.Value != true).ToList();

                foreach (PersonProperties prop in userProfilesResult)
                {
                    if (!returnList.Any(x => x.AccountName == prop.AccountName))
                        returnList.Add(prop);
                }
            }

            return returnList;
        }

        public static List<PersonProperties> GetUserProfiles()
        {
            var userProfilesResult = new List<PersonProperties>();
            var returnList = new List<PersonProperties>();

            using (var context = new ClientContext(SharePointHelper.UserProfileUrl))
            {
                var web = context.Web;
                var peopleManager = new PeopleManager(context);

                var siteUsers = from user in web.SiteUsers
                                where user.PrincipalType == Microsoft.SharePoint.Client.Utilities.PrincipalType.User
                                select user;

                var usersResult = context.LoadQuery(siteUsers);
                context.ExecuteQuery();

                foreach (var user in usersResult)
                {
                    var userProfile = peopleManager.GetPropertiesFor(user.LoginName);
                    context.Load(userProfile);

                    userProfilesResult.Add(userProfile);  
                }
                context.ExecuteQuery();

                userProfilesResult = userProfilesResult.Where(x => x.ServerObjectIsNull != null && x.ServerObjectIsNull.Value != true).ToList();

                foreach (PersonProperties prop in userProfilesResult)
                {
                    if (!returnList.Any(x => x.AccountName == prop.AccountName))
                        returnList.Add(prop);
                }
            }

            return returnList;
        }

        public static List<LDAP.LDAPUser> Query()
        {
            var users = LDAP.LDAPHelper.SearchUsers("");

            return users;
        }
    }
}
