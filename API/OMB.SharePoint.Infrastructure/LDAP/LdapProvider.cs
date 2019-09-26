using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMB.SharePoint.Infrastructure.Models;

namespace OMB.SharePoint.Infrastructure.LDAP
{
    public class LdapProvider
    {
        // Called LDAP Path but can handle GC paths too
        private readonly string _ldapPath;
        private string _un;
        private string _pw;

        public LdapProvider(string path)
        {
            _ldapPath = path;
        }

        public LdapProvider(string path, string un, string pw) : this(path)
        {
            _un = un;
            _pw = pw;
        }

        /// <summary>
        /// Provide the friendly name of the group to get all members of the group.
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public List<LDAPUser> GetMembersOfGroup(string groupName)
        {
            var members = new List<LDAPUser>();

            using (var directoryEntry = GetEntry())
            {
                try
                {
                    var groupDistinguishedName = GetGroupDistinguishedName(directoryEntry, groupName);

                    if (string.IsNullOrEmpty(groupDistinguishedName))
                         throw new Exception("Group distinguished name not found. Group: [" + groupName + "]");

                    members = GetMembersOfGroup(directoryEntry, groupDistinguishedName);
                }
                catch (Exception ex)
                {
                    WriteException(ex);
                }
            }

            return members;
        }

        private DirectoryEntry GetEntry()
        {
            if (_un == "" || _pw == "")
                return new DirectoryEntry(_ldapPath);
            else
                return new DirectoryEntry(_ldapPath, _un, _pw);
        }

        private void WriteException(Exception ex)
        {
            LDAPHelper.WriteException(ex, this.GetType().Name);
        }
        
        private void SetupDefaultPropertiesOnDirectorySearcher(DirectorySearcher searcher)
        {
            // allow us to use references to other active dir domains.
            searcher.ReferralChasing = ReferralChasingOption.All;
            searcher.ServerTimeLimit = new TimeSpan(0, 1, 0);
        }

        private string GetGroupDistinguishedName(DirectoryEntry directoryEntry, string groupName)
        {
            var distinguishedName = ""; 

            var filter = string.Format("(&(objectClass=group)(cn={0}))", groupName);
            var propertiesToLoad = new string[] { "distinguishedName" };

            using (var ds = new DirectorySearcher(directoryEntry, filter, propertiesToLoad))
            {
                SetupDefaultPropertiesOnDirectorySearcher(ds);

                var result = ds.FindOne();
                if (result != null)
                {
                    distinguishedName = result.Properties["distinguishedName"][0].ToString();
                }
            }

            return distinguishedName;
        }

        private List<LDAPUser> GetMembersOfGroup(DirectoryEntry directoryEntry, string groupDistinguishedName)
        {
            if (string.IsNullOrEmpty(groupDistinguishedName))
            {
                throw new Exception("Group name not provided. Cannot look for group members.");
            }

            var filter = string.Format("(&(objectClass=user)(memberof={0}))", groupDistinguishedName);

            return GetUsers(directoryEntry, filter);
        }

        public List<LDAPUser> GetUsers(string filter)
        {
            try
            {
                var users = new List<LDAPUser>();

                using (System.Web.Hosting.HostingEnvironment.Impersonate())
                {
                    using (DirectoryEntry entry = new DirectoryEntry(_ldapPath))
                    {
                        users = GetUsers(entry, filter);
                    }
                }

                return users;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<SearchResult> SafeFindAll(DirectorySearcher searcher)
        {
            using (SearchResultCollection results = searcher.FindAll())
            {
                foreach (SearchResult result in results)
                {
                    yield return result;
                }
            } // SearchResultCollection will be disposed here
        }

        private List<LDAPUser> GetUsers(DirectoryEntry entry, string filter)
        {
            var users = new List<LDAPUser>();

            if (entry != null)
            {
                var propertiesToLoad = new string[] { "givenname", "samaccountname", "sn", "userPrincipalName", "extensionAttribute3", "displayName", "employeeType", "department", "physicalDeliveryOfficeName", "title", "userAccountControl", "whenCreated" };

                using (DirectorySearcher ds = new DirectorySearcher(entry, filter, propertiesToLoad))
                {
                    SetupDefaultPropertiesOnDirectorySearcher(ds);
                    ds.PageSize = 1000;

                    var results = SafeFindAll(ds);

                    foreach (SearchResult result in results)
                    {
                        try
                        {
                            var item = new LDAPUser(result);

                            users.Add(item);
                        }
                        catch (Exception ex)
                        {
                            var ex2 = new Exception("Failed to load user " + result.Path, ex);
                            
                            WriteException(ex2);
                        }
                    }
                }
            }

            return users;
        }
    }
}
