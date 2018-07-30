using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.MultiOAuth;
using Microsoft.AspNetCore.Authentication.MultiOAuth.Stores;
using Microsoft.AspNetCore.Authentication.Weixin;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary> 
    /// </summary>
    public static class QQAuthenticationExtensions
    {
        /// <summary> 
        /// </summary>
        public static AuthenticationBuilder AddWeixinAuthenticationStore<TClientStore>(this AuthenticationBuilder builder) where TClientStore : class, IClientStore
        {
            return builder.AddWeixinAuthenticationStore<TClientStore>(WeixinAuthenticationDefaults.AuthenticationScheme, WeixinAuthenticationDefaults.DisplayName, options => { });
        }

        ///// <summary> 
        ///// </summary>
        //public static AuthenticationBuilder AddWeixinAuthenticationStore<TClientStore>(this AuthenticationBuilder builder, Action<WeixinAuthenticationOptions> configureOptions)
        //{
        //    return builder.AddWeixinAuthenticationStore(WeixinAuthenticationDefaults.AuthenticationScheme, WeixinAuthenticationDefaults.DisplayName, configureOptions);
        //}

        ///// <summary> 
        ///// </summary>
        //public static AuthenticationBuilder AddWeixinAuthenticationStore<TClientStore>(this AuthenticationBuilder builder, string authenticationScheme, Action<WeixinAuthenticationOptions> configureOptions)
        //{
        //    return builder.AddWeixinAuthenticationStore<TClientStore>(authenticationScheme, WeixinAuthenticationDefaults.DisplayName, configureOptions);
        //}

        public static AuthenticationBuilder AddWeixinAuthenticationStore<TClientStore>(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<WeixinAuthenticationOptions> configureOptions) where TClientStore : class, IClientStore
        {
            builder.Services.AddScoped<IClientStore, TClientStore>();
            return builder.AddMultiOAuth<WeixinAuthenticationOptions, WeixinAuthenticationHandler>(authenticationScheme, displayName, configureOptions);
        }


        public static AuthenticationBuilder AddMultiOAuth<TOptions, THandler>(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<TOptions> configureOptions)
           where TOptions : MultiOAuthOptions, new()
           where THandler : MultiOAuthHandler<TOptions>
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<TOptions>, MultiOAuthPostConfigureOptions<TOptions, THandler>>());
            return builder.AddRemoteScheme<TOptions, THandler>(authenticationScheme, displayName, configureOptions);
        }


        //private AuthenticationBuilder AddSchemeHelper<TOptions, THandler>(string authenticationScheme, string displayName, Action<TOptions> configureOptions)
        //where TOptions : class, new()
        //where THandler : class, IAuthenticationHandler
        //    {
        //        Services.Configure<AuthenticationOptions>(o =>
        //        {
        //            o.AddScheme(authenticationScheme, scheme => {
        //                scheme.HandlerType = typeof(THandler);
        //                scheme.DisplayName = displayName;
        //            });
        //        });
        //        if (configureOptions != null)
        //        {
        //            Services.Configure(authenticationScheme, configureOptions);
        //        }
        //        Services.AddTransient<THandler>();
        //        return this;
        //    }
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