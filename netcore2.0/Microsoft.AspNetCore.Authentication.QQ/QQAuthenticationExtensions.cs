using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.QQ;
using Microsoft.Extensions.Options;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 
    /// </summary>
    public static class QQAuthenticationExtensions
    {
        /// <summary> 
        /// </summary>
        public static AuthenticationBuilder AddQQAuthentication(this AuthenticationBuilder builder)
        {
            return builder.AddQQAuthentication(QQAuthenticationDefaults.AuthenticationScheme, QQAuthenticationDefaults.DisplayName, options => { });
        }

        /// <summary> 
        /// </summary>
        public static AuthenticationBuilder AddQQAuthentication(this AuthenticationBuilder builder, Action<QQAuthenticationOptions> configureOptions)
        {
            return builder.AddQQAuthentication(QQAuthenticationDefaults.AuthenticationScheme, QQAuthenticationDefaults.DisplayName, configureOptions);
        }

        /// <summary> 
        /// </summary>
        public static AuthenticationBuilder AddQQAuthentication(this AuthenticationBuilder builder, string authenticationScheme, Action<QQAuthenticationOptions> configureOptions)
        {
            return builder.AddQQAuthentication(authenticationScheme, QQAuthenticationDefaults.DisplayName, configureOptions);
        }

        /// <summary> 
        /// </summary>
        public static AuthenticationBuilder AddQQAuthentication(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<QQAuthenticationOptions> configureOptions)
        {
            return builder.AddOAuth<QQAuthenticationOptions, QQAuthenticationHandler>(authenticationScheme, displayName, configureOptions);
        }
    }
}

#region old code 



namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Extension methods to add QQ authentication capabilities to an HTTP application pipeline.
    /// </summary>
    public static class QQAuthenticationExtensions
    {
        /// <summary>
        /// Adds the <see cref="QQAuthenticationMiddleware"/> middleware to the specified
        /// <see cref="IApplicationBuilder"/>, which enables QQ authentication capabilities.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> to add the middleware to.</param>
        /// <param name="options">A <see cref="QQAuthenticationOptions"/> that specifies options for the middleware.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IApplicationBuilder UseQQAuthentication(this IApplicationBuilder app, QQAuthenticationOptions options)
        {
            throw new NotSupportedException("This method is no longer supported, see https://go.microsoft.com/fwlink/?linkid=845470");
        }

        /// <summary>
        /// Adds the <see cref="QQAuthenticationMiddleware"/> middleware to the specified
        /// <see cref="IApplicationBuilder"/>, which enables QQ authentication capabilities.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> to add the middleware to.</param>
        /// <param name="configuration">An action delegate to configure the provided <see cref="QQAuthenticationOptions"/>.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IApplicationBuilder UseQQAuthentication(this IApplicationBuilder app, Action<QQAuthenticationOptions> configuration)
        {
            throw new NotSupportedException("This method is no longer supported, see https://go.microsoft.com/fwlink/?linkid=845470");
        }
    }
}



#endregion