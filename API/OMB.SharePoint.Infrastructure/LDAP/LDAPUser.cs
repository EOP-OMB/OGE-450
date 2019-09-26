using System;
using System.Collections.Generic;
using System.DirectoryServices;

namespace OMB.SharePoint.Infrastructure.LDAP
{
    public class LDAPUser
    {
        public string UPN { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string EmployeeType { get; set; }
        public string Department { get; set; }
        public string Office { get; set; }
        public string Title { get; set; }
        public List<string> Groups { get; set; }
        public bool Inactive { get { return Convert.ToBoolean(UserAccountControl & 0x0002)|| string.IsNullOrEmpty(SamAccountName); } }

        /// <summary>
        /// SAM = Security Accounts Manager
        /// </summary>
        public string SamAccountName { get; set; }

        /// <summary>
        /// Bit field flags that control the behavior of the AD user account.
        ///
        /// A few relevant ones:
        /// 2   = ACCOUNTDISABLE
        /// 512 = NORMAL_ACCOUNT
        ///
        /// 514 = NORMAL_ACCOUNT && ACCOUNTDISABLE
        /// </summary>
        public int UserAccountControl { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MemberOf { get; set; }
        public string WhenCreated { get; set; }

        public LDAPUser()
        {
            Groups = new List<string>();
        }

        public LDAPUser(SearchResult result) : this()
        {
            LDAPHelper.CatchException(delegate { this.Email = result.Properties["extensionAttribute3"][0].ToString(); });
            LDAPHelper.CatchException(delegate { this.Office = result.Properties["physicalDeliveryOfficeName"][0].ToString(); });
            LDAPHelper.CatchException(delegate { Title = result.Properties["title"][0].ToString(); });
            LDAPHelper.CatchException(delegate { EmployeeType = result.Properties["employeeType"][0].ToString(); });
            LDAPHelper.CatchException(delegate { Department = result.Properties["department"][0].ToString(); });
            LDAPHelper.CatchException(delegate { SamAccountName = result.Properties["samaccountname"][0].ToString(); });
            LDAPHelper.CatchException(delegate { FirstName = result.Properties["givenname"][0].ToString(); });
            LDAPHelper.CatchException(delegate { LastName = result.Properties["sn"][0].ToString(); });
            LDAPHelper.CatchException(delegate { WhenCreated = result.Properties["whenCreated"][0].ToString(); });

            UPN = result.Properties["UserPrincipalName"][0].ToString();
            DisplayName = result.Properties["displayName"][0].ToString();
            UserAccountControl = (result.Properties["useraccountcontrol"][0] is int) ? (int)result.Properties["useraccountcontrol"][0] : 0;
        }
    }
}
