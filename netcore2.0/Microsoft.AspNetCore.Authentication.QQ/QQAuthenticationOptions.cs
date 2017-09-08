using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Microsoft.AspNetCore.Authentication.QQ
{
    /// <summary>
    ///  
    /// </summary>
    public class QQAuthenticationOptions : OAuthOptions
    {
        public string OpenIdEndpoint { get; set; }

        public QQAuthenticationOptions()
        {

            ClaimsIssuer = QQAuthenticationDefaults.Issuer;
            CallbackPath = new PathString(QQAuthenticationDefaults.CallbackPath);

            AuthorizationEndpoint = QQAuthenticationDefaults.AuthorizationEndpoint;
            TokenEndpoint = QQAuthenticationDefaults.TokenEndpoint;
            UserInformationEndpoint = QQAuthenticationDefaults.UserInformationEndpoint;
            OpenIdEndpoint = QQAuthenticationDefaults.UserOpenIdEndpoint;

            ClaimActionCollectionMapExtensions.MapJsonKey(ClaimActions, ClaimTypes.NameIdentifier, "id");
            ClaimActionCollectionMapExtensions.MapJsonKey(ClaimActions, ClaimTypes.Name, "displayName");

        }
    }
}
