using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

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
            AuthenticationScheme = QQAuthenticationDefaults.AuthenticationScheme;
            DisplayName = QQAuthenticationDefaults.DisplayName;
            ClaimsIssuer = QQAuthenticationDefaults.Issuer;
            CallbackPath = new PathString(QQAuthenticationDefaults.CallbackPath);

            AuthorizationEndpoint = QQAuthenticationDefaults.AuthorizationEndpoint;
            TokenEndpoint = QQAuthenticationDefaults.TokenEndpoint;
            UserInformationEndpoint = QQAuthenticationDefaults.UserInformationEndpoint;
            OpenIdEndpoint = QQAuthenticationDefaults.UserOpenIdEndpoint;

        }
    }
}
