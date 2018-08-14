using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Weixin;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary> 
    /// </summary>
    public static class WeixinAuthenticationExtensions
    {
        /// <summary> 
        /// </summary>
        public static AuthenticationBuilder AddMultiWeixinAuthentication(this AuthenticationBuilder builder)
        {
            return builder.AddMultiWeixinAuthentication(WeixinAuthenticationDefaults.AuthenticationScheme, WeixinAuthenticationDefaults.DisplayName, options => { });
        }

        /// <summary> 
        /// </summary>
        public static AuthenticationBuilder AddMultiWeixinAuthentication<TClientStore>(this AuthenticationBuilder builder, Action<WeixinAuthenticationOptions> configureOptions)
        {
            return builder.AddMultiWeixinAuthentication(WeixinAuthenticationDefaults.AuthenticationScheme, WeixinAuthenticationDefaults.DisplayName, configureOptions);
        }

        /// <summary> 
        /// </summary>
        public static AuthenticationBuilder AddMultiWeixinAuthentication<TClientStore>(this AuthenticationBuilder builder, string authenticationScheme, Action<WeixinAuthenticationOptions> configureOptions)
        {
            return builder.AddMultiWeixinAuthentication(authenticationScheme, WeixinAuthenticationDefaults.DisplayName, configureOptions);
        }

        public static AuthenticationBuilder AddMultiWeixinAuthentication(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<WeixinAuthenticationOptions> configureOptions)
        {
            return builder.AddMultiOAuth<WeixinAuthenticationOptions, WeixinAuthenticationHandler>(authenticationScheme, displayName, configureOptions);
        }


    }
}

