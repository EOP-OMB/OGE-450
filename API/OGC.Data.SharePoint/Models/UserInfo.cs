using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.SharePoint.Client;
using OMB.SharePoint.Infrastructure;
using System.Security.Claims;
using Microsoft.SharePoint.Client.Search.Query;

namespace OGC.Data.SharePoint.Models
{
    public class UserInfo
    {
        public int Id { get; set; }
        public string Upn { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public bool IsReviewer { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsRestrictedAdmin { get; set; }
        public int CurrentFormId { get; set; }
        public string CurrentFormStatus { get; set; }
        public string UserProfileUrl { get; set; }

        public string Branch { get; set; }
        public string PhoneNumber { get; set; }

        public bool InMaintMode { get; set; }

        public List<string> Groups { get; set; }

        //public Dictionary<string, string> Profile { get; set; }

        public static UserInfo GetUser(ClaimsIdentity identity, bool clearCache = false)
        {
            var upn = identity.Claims.Where(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn").FirstOrDefault().Value;

            upn = "i:0e.t|adfs|" + upn;

            return GetUser(upn, clearCache);
        }

        public static List<UserInfo> Search(string query)
        {
            using (ClientContext clientContext = new ClientContext(SharePointHelper.SearchUrl))
            {
                KeywordQuery keywordQuery = new KeywordQuery(clientContext);
                keywordQuery.QueryText = query + "*";
                keywordQuery.TrimDuplicates = true;
                keywordQuery.SourceId = Guid.Parse(SharePointHelper.SourceId);

                SearchExecutor searchExecutor = new SearchExecutor(clientContext);
                ClientResult<ResultTableCollection> results = searchExecutor.ExecuteQuery(keywordQuery);

                clientContext.ExecuteQuery();

                var users = new List<UserInfo>();

                foreach (var result in results.Value[0].ResultRows)
                {
                    if (result != null)
                    {
                        if (result["PreferredName"] != null && result["AccountName"] != null)
                        {
                            var user = new UserInfo() { DisplayName = result["PreferredName"].ToString(), Upn = result["AccountName"].ToString() };

                            if (!users.Contains(user))
                                users.Add(user);
                        }
                    }
                }

                return users;
            }
        }

        public static UserInfo GetUser(string upn, bool clearCache = false)
        {
            if (clearCache)
                MemCache.Clear(upn);

            var result = (UserInfo)MemCache.Get(upn);

            if (result == null)
            {
                using (var ctx = new ClientContext(SharePointHelper.Url))
                {
                    var u = ctx.Web.EnsureUser(upn);
                    
                    ctx.Load(u);
                    ctx.Load(u.Groups);
                    ctx.ExecuteQuery();

                    result = MapToModel(u);
                    MemCache.Add(upn, result);
                }
            }

            return result;
        }

        public static List<UserInfo> GetUserByGroup(string groupName)
        {
            var result = (List<UserInfo>)MemCache.Get(groupName);

            if (result == null)
            {
                result = new List<UserInfo>();
                var users = EmailHelper.GetUsersInGroup(groupName);

                foreach (User usr in users)
                {
                    var ui = MapToModel(usr);

                    result.Add(ui);
                }

                MemCache.Add(groupName, result);
                
            }

            return result;
        }

        public static UserCollection GetAll()
        {
            using (var ctx = new ClientContext(SharePointHelper.UserProfileUrl))
            {
                var u = ctx.Web.SiteUsers;

                ctx.Load(u);
                ctx.ExecuteQuery();

                return u;
            }
        }

        private static UserInfo MapToModel(User u)
        {
            var info = new UserInfo();

            info.Id = u.Id;
            info.Upn = u.LoginName;

            var form = GetUserForm(u.LoginName);

            if (form != null)
            {
                info.CurrentFormId = form.Id;
                info.CurrentFormStatus = form.FormStatus;
            }
            else
            {
                info.CurrentFormId = 0;
                info.CurrentFormStatus = "Not Available";
            }

            var profile = UserProfileHelper.GetUserProfile(u.LoginName);

            if (profile != null)
            {
                info.DisplayName = profile.IsPropertyAvailable("DisplayName") ? profile.DisplayName : u.LoginName;
                info.UserProfileUrl = profile.IsPropertyAvailable("UserUrl") ? profile.UserUrl : "";
                info.Email = profile.IsPropertyAvailable("Email") ? profile.Email : "";

                info.PhoneNumber = (profile.IsPropertyAvailable("UserProfileProperties") && profile.UserProfileProperties.ContainsKey("WorkPhone")) ? profile.UserProfileProperties["WorkPhone"] : string.Empty;
                info.Branch = (profile.IsPropertyAvailable("UserProfileProperties") && profile.UserProfileProperties.ContainsKey("Office")) ? profile.UserProfileProperties["Office"] : string.Empty;
            }

            try
            {
                info.Groups = u.Groups.Select(x => x.Title).ToList();
                info.IsReviewer = info.IsInGroup(SharePointHelper.ReviewerGroup);
                info.IsAdmin = info.IsInGroup(SharePointHelper.AdminGroup);
                info.IsRestrictedAdmin = info.IsInGroup(SharePointHelper.RestrictedAdmin);
            }
            catch (Exception ex)
            {
                // ignore
            }

            info.InMaintMode = Settings.IN_MAINTENANCE_MODE;

            return info;
        }

        private bool IsInGroup(string groupName)
        {
            return this.Groups.Where(x => x == groupName).Count() > 0;
        }

        public static int GetUserFormId(string loginName)
        {
            var currentForm = GetUserForm(loginName);

            return currentForm == null ? 0 : currentForm.Id;
        }

        public static OGEForm450 GetUserForm(string loginName)
        {
            var form = OGEForm450.GetCurrentFormByUser(loginName);

            return form;
        }

        public static string GetUserByName(string username)
        {
            var results = Search(username);

            if (results != null && results.Count >= 1)
            {
                // return the first result (should be the adfs claim and not the windows log in)
                return results[0].Upn;
            }
            else
            {
                // user not found
                return "";
            }
        }

        internal static UserInfo GetUserInfoByName(string username)
        {
            var results = Search(username);

            if (results != null && results.Count == 1)
            {
                return results[0];
            }
            else
            {
                // user not found or more than one result matched the query
                return new UserInfo() { DisplayName = username };
            }
        }
    }
}
