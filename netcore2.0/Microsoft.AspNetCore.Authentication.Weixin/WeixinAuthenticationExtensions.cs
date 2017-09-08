using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Weixin;
using System;


namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary> 
    /// </summary>
    public static class QQAuthenticationExtensions
    {
        /// <summary> 
        /// </summary>
        public static AuthenticationBuilder AddWeixinAuthentication(this AuthenticationBuilder builder)
        {
            return builder.AddWeixinAuthentication(WeixinAuthenticationDefaults.AuthenticationScheme, WeixinAuthenticationDefaults.DisplayName, options => { });
        }

        /// <summary> 
        /// </summary>
        public static AuthenticationBuilder AddWeixinAuthentication(this AuthenticationBuilder builder, Action<WeixinAuthenticationOptions> configureOptions)
        {
            return builder.AddWeixinAuthentication(WeixinAuthenticationDefaults.AuthenticationScheme, WeixinAuthenticationDefaults.DisplayName, configureOptions);
        }

        /// <summary> 
        /// </summary>
        public static AuthenticationBuilder AddWeixinAuthentication(this AuthenticationBuilder builder, string authenticationScheme, Action<WeixinAuthenticationOptions> configureOptions)
        {
            return builder.AddWeixinAuthentication(authenticationScheme, WeixinAuthenticationDefaults.DisplayName, configureOptions);
        }

        /// <summary> 
        /// </summary>
        public static AuthenticationBuilder AddWeixinAuthentication(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<WeixinAuthenticationOptions> configureOptions)
        {
            return builder.AddOAuth<WeixinAuthenticationOptions, WeixinAuthenticationHandler>(authenticationScheme, displayName, configureOptions);
        }
    }
}

#region Old Code 

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Extension methods to add Weixin authentication capabilities to an HTTP application pipeline.
    /// </summary>
    public static class WeixinAuthenticationExtensions
    {
        /// <summary>
        /// Adds the <see cref="WeixinAuthenticationMiddleware"/> middleware to the specified
        /// <see cref="IApplicationBuilder"/>, which enables Weixin authentication capabilities.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> to add the middleware to.</param>
        /// <param name="options">A <see cref="WeixinAuthenticationOptions"/> that specifies options for the middleware.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IApplicationBuilder UseWeixinAuthentication(this IApplicationBuilder app, WeixinAuthenticationOptions options)
        {
            throw new NotSupportedException("This method is no longer supported, see https://go.microsoft.com/fwlink/?linkid=845470");
        }

        /// <summary>
        /// Adds the <see cref="WeixinAuthenticationMiddleware"/> middleware to the specified
        /// <see cref="IApplicationBuilder"/>, which enables Weixin authentication capabilities.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> to add the middleware to.</param>
        /// <param name="configuration">An action delegate to configure the provided <see cref="WeixinAuthenticationOptions"/>.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IApplicationBuilder UseWeixinAuthentication(this IApplicationBuilder app, Action<WeixinAuthenticationOptions> configuration)
        {
            throw new NotSupportedException("This method is no longer supported, see https://go.microsoft.com/fwlink/?linkid=845470");
        }
    }


}

#endregion