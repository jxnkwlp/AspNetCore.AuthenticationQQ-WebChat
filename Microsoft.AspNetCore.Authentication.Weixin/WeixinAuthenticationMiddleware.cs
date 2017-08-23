using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

namespace Microsoft.AspNetCore.Authentication.Weixin
{
    public class WeixinAuthenticationMiddleware : OAuthMiddleware<WeixinAuthenticationOptions>
    {
        public WeixinAuthenticationMiddleware(RequestDelegate next, IDataProtectionProvider dataProtectionProvider, ILoggerFactory loggerFactory, UrlEncoder encoder, IOptions<SharedAuthenticationOptions> sharedOptions, IOptions<WeixinAuthenticationOptions> options) : base(next, dataProtectionProvider, loggerFactory, encoder, sharedOptions, options)
        {
        }

        protected override AuthenticationHandler<WeixinAuthenticationOptions> CreateHandler()
        {
            return new WeixinAuthenticationHandler(Backchannel);
        }
    }
}