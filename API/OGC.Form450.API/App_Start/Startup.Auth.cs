using System.Configuration;
using System.IdentityModel.Tokens;
using Microsoft.Owin.Security.ActiveDirectory;
using Owin;

namespace OGC.Form450.API
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseActiveDirectoryFederationServicesBearerAuthentication(
               new ActiveDirectoryFederationServicesBearerAuthenticationOptions
               {
                   MetadataEndpoint = ConfigurationManager.AppSettings["ida:AdfsMetadataEndpoint"],
                   TokenValidationParameters = new TokenValidationParameters()
                   {
                       ValidAudience = ConfigurationManager.AppSettings["ida:Audience"],
                       ValidIssuer = ConfigurationManager.AppSettings["ida:Issuer"]
                   }
               }
           );
        }
    }
}