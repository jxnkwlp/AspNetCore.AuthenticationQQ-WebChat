namespace Microsoft.AspNetCore.Authentication.QQ
{
    /// <summary>
    /// Default values for QQ authentication.
    /// </summary>
    class QQAuthenticationDefaults
    {
        /// <summary>
        /// Default value for <see cref="AuthenticationOptions.DefaultAuthenticateScheme"/>.
        /// </summary>
        public const string AuthenticationScheme = "QQ";

        /// <summary>
        /// Default value for <see cref="RemoteAuthenticationOptions.DisplayName"/>.
        /// </summary>
        public const string DisplayName = "QQ";

        /// <summary>
        /// Default value for <see cref="RemoteAuthenticationOptions.CallbackPath"/>.
        /// </summary>
        public const string CallbackPath = "/signin-qq";

        /// <summary>
        /// Default value for <see cref="AuthenticationOptions.ClaimsIssuer"/>.
        /// </summary>
        public const string Issuer = "QQ";

        public const string AuthorizationEndpoint = "https://graph.qq.com/oauth2.0/authorize";
        public const string TokenEndpoint = "https://graph.qq.com/oauth2.0/token";
        //public const string TokenRefreshEndpoint = "https://graph.qq.com/oauth2.0/token";
        public const string UserOpenIdEndpoint = "https://graph.qq.com/oauth2.0/me";
        public const string UserInformationEndpoint = "https://graph.qq.com/user/get_user_info";
    }
}
