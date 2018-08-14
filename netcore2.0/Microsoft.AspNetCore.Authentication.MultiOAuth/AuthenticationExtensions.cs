using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.MultiOAuth;
using Microsoft.AspNetCore.Authentication.MultiOAuth.Stores;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AuthenticationExtensions
    {
        public static AuthenticationBuilder AddMultiOAuthStore<TClientStore>(this AuthenticationBuilder builder)
             where TClientStore : class, IClientStore
        {
            builder.Services.TryAddScoped<IClientStore, TClientStore>();
            return builder;
        }

        /// <summary>
        /// 
        /// </summary> 
        public static AuthenticationBuilder AddMultiOAuth<TOptions, THandler>(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<TOptions> configureOptions)
                   where TOptions : MultiOAuthOptions, new()
                   where THandler : MultiOAuthHandler<TOptions>
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<TOptions>, MultiOAuthPostConfigureOptions<TOptions, THandler>>());
            return builder.AddRemoteScheme<TOptions, THandler>(authenticationScheme, displayName, configureOptions);
        }
    }
}
