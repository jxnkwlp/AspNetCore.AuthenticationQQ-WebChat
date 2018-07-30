using System.Net.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Authentication.MultiOAuth
{
    /// <summary>
    /// Used to setup defaults for the MultiOAuthOptions.
    /// </summary>
    public class MultiOAuthPostConfigureOptions<TOptions, THandler> : IPostConfigureOptions<TOptions>
         where TOptions : MultiOAuthOptions, new()
         where THandler : MultiOAuthHandler<TOptions>
    {
        private readonly IDataProtectionProvider _dp;

        public MultiOAuthPostConfigureOptions(IDataProtectionProvider dataProtection)
        {
            _dp = dataProtection;
        }

        public void PostConfigure(string name, TOptions options)
        {
            options.DataProtectionProvider = options.DataProtectionProvider ?? _dp;
            if (options.Backchannel == null)
            {
                options.Backchannel = new HttpClient(options.BackchannelHttpHandler ?? new HttpClientHandler());
                options.Backchannel.DefaultRequestHeaders.UserAgent.ParseAdd("Microsoft ASP.NET Core OAuth handler");
                options.Backchannel.Timeout = options.BackchannelTimeout;
                options.Backchannel.MaxResponseContentBufferSize = 1024 * 1024 * 10; // 10 MB
            }

            if (options.StateDataFormat == null)
            {
                var dataProtector = options.DataProtectionProvider.CreateProtector(
                    typeof(THandler).FullName, name, "v1");
                options.StateDataFormat = new PropertiesDataFormat(dataProtector);
            }
        }
    }
}
