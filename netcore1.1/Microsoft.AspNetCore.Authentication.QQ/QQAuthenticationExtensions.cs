using Microsoft.AspNetCore.Authentication.QQ;
using Microsoft.Extensions.Options;
using System;

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
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            return app.UseMiddleware<QQAuthenticationMiddleware>(Options.Create(options));
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
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            var options = new QQAuthenticationOptions();

            configuration(options);

            return app.UseMiddleware<QQAuthenticationMiddleware>(Options.Create(options));
        }
    }
}
