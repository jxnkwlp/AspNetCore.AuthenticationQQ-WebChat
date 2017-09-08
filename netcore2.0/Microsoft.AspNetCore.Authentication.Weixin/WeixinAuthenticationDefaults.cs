namespace Microsoft.AspNetCore.Authentication.Weixin
{
    /// <summary>
    /// Default values for Weixin authentication.
    /// </summary>
    static class WeixinAuthenticationDefaults
    {
        /// <summary>
        /// Default value for <see cref="AuthenticationOptions.DefaultAuthenticateScheme"/>.
        /// </summary>
        public const string AuthenticationScheme = "Weixin";

        public const string DisplayName = "Weixin";

        /// <summary>
        /// Default value for <see cref="RemoteAuthenticationOptions.CallbackPath"/>.
        /// </summary>
        public const string CallbackPath = "/signin-weixin";

        /// <summary>
        /// Default value for <see cref="AuthenticationSchemeOptions.ClaimsIssuer"/>.
        /// </summary>
        public const string Issuer = "Weixin";

        /// <summary>
        /// Default value for <see cref="OAuth.OAuthOptions.AuthorizationEndpoint"/>.
        /// </summary>
        public const string AuthorizationEndpoint = "https://open.weixin.qq.com/connect/qrconnect";

        /// <summary>
        /// Default value for <see cref="OAuth.OAuthOptions.TokenEndpoint"/>.
        /// </summary>
        public const string TokenEndpoint = "https://api.weixin.qq.com/sns/oauth2/access_token";

        /// <summary>
        /// Default value for <see cref="OAuth.OAuthOptions.UserInformationEndpoint"/>.
        /// </summary>
        public const string UserInformationEndpoint = "https://api.weixin.qq.com/sns/userinfo";
    }
}