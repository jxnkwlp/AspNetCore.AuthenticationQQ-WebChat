using Microsoft.AspNetCore.Authentication.Weixin;
using Microsoft.Extensions.Options;
using System;

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
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            return app.UseMiddleware<WeixinAuthenticationMiddleware>(Options.Create(options));
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
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            var options = new WeixinAuthenticationOptions();

            configuration(options);

            return app.UseMiddleware<WeixinAuthenticationMiddleware>(Options.Create(options));
        }
    }
}