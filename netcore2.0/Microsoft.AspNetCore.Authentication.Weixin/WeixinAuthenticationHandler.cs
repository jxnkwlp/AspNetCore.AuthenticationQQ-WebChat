using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

namespace Microsoft.AspNetCore.Authentication.Weixin
{
    class WeixinAuthenticationHandler : OAuthHandler<WeixinAuthenticationOptions>
    {
        public WeixinAuthenticationHandler(IOptionsMonitor<WeixinAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        /// <summary>
        ///  Last step:
        ///  create ticket from remote server
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="properties"></param>
        /// <param name="tokens"></param>
        /// <returns></returns>
        protected override async Task<AuthenticationTicket> CreateTicketAsync(ClaimsIdentity identity, AuthenticationProperties properties, OAuthTokenResponse tokens)
        {
            var address = QueryHelpers.AddQueryString(Options.UserInformationEndpoint, new Dictionary<string, string>
            {
                ["access_token"] = tokens.AccessToken,
                ["openid"] = tokens.Response.Value<string>("openid")
            });

            var response = await Backchannel.GetAsync(address);
            if (!response.IsSuccessStatusCode)
            {
                Logger.LogError("An error occurred while retrieving the user profile: the remote server " +
                                "returned a {Status} response with the following payload: {Headers} {Body}.",
                                /* Status: */ response.StatusCode,
                                /* Headers: */ response.Headers.ToString(),
                                /* Body: */ await response.Content.ReadAsStringAsync());

                throw new HttpRequestException("An error occurred while retrieving user information.");
            }

            var payload = JObject.Parse(await response.Content.ReadAsStringAsync());
            if (!string.IsNullOrEmpty(payload.Value<string>("errcode")))
            {
                Logger.LogError("An error occurred while retrieving the user profile: the remote server " +
                                "returned a {Status} response with the following payload: {Headers} {Body}.",
                                /* Status: */ response.StatusCode,
                                /* Headers: */ response.Headers.ToString(),
                                /* Body: */ await response.Content.ReadAsStringAsync());

                throw new HttpRequestException("An error occurred while retrieving user information.");
            }

            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, WeixinAuthenticationHelper.GetUnionid(payload), Options.ClaimsIssuer));
            identity.AddClaim(new Claim(ClaimTypes.Name, WeixinAuthenticationHelper.GetNickname(payload), Options.ClaimsIssuer));
            identity.AddClaim(new Claim(ClaimTypes.Gender, WeixinAuthenticationHelper.GetSex(payload), Options.ClaimsIssuer));
            identity.AddClaim(new Claim(ClaimTypes.Country, WeixinAuthenticationHelper.GetCountry(payload), Options.ClaimsIssuer));
            identity.AddClaim(new Claim("urn:weixin:openid", WeixinAuthenticationHelper.GetOpenId(payload), Options.ClaimsIssuer));
            identity.AddClaim(new Claim("urn:weixin:province", WeixinAuthenticationHelper.GetProvince(payload), Options.ClaimsIssuer));
            identity.AddClaim(new Claim("urn:weixin:city", WeixinAuthenticationHelper.GetCity(payload), Options.ClaimsIssuer));
            identity.AddClaim(new Claim("urn:weixin:headimgurl", WeixinAuthenticationHelper.GetHeadimgUrl(payload), Options.ClaimsIssuer));
            identity.AddClaim(new Claim("urn:weixin:privilege", WeixinAuthenticationHelper.GetPrivilege(payload), Options.ClaimsIssuer));

            identity.AddClaim(new Claim("urn:weixin:user_info", payload.ToString(), Options.ClaimsIssuer));

            var context = new OAuthCreatingTicketContext(new ClaimsPrincipal(identity), properties, Context, Scheme, Options, Backchannel, tokens, payload);
            context.RunClaimActions();

            await Events.CreatingTicket(context);

            return new AuthenticationTicket(context.Principal, context.Properties, Scheme.Name);
        }

        /// <summary>
        /// Step 2：通过code获取access_token
        /// </summary> 
        protected override async Task<OAuthTokenResponse> ExchangeCodeAsync(string code, string redirectUri)
        {
            var address = QueryHelpers.AddQueryString(Options.TokenEndpoint, new Dictionary<string, string>()
            {
                ["appid"] = Options.ClientId,
                ["secret"] = Options.ClientSecret,
                ["code"] = code,
                ["grant_type"] = "authorization_code"
            });

            var response = await Backchannel.GetAsync(address);
            if (!response.IsSuccessStatusCode)
            {
                Logger.LogError("An error occurred while retrieving an access token: the remote server " +
                                "returned a {Status} response with the following payload: {Headers} {Body}.",
                                /* Status: */ response.StatusCode,
                                /* Headers: */ response.Headers.ToString(),
                                /* Body: */ await response.Content.ReadAsStringAsync());

                return OAuthTokenResponse.Failed(new Exception("An error occurred while retrieving an access token."));
            }

            var payload = JObject.Parse(await response.Content.ReadAsStringAsync());
            if (!string.IsNullOrEmpty(payload.Value<string>("errcode")))
            {
                Logger.LogError("An error occurred while retrieving an access token: the remote server " +
                                "returned a {Status} response with the following payload: {Headers} {Body}.",
                                /* Status: */ response.StatusCode,
                                /* Headers: */ response.Headers.ToString(),
                                /* Body: */ await response.Content.ReadAsStringAsync());

                return OAuthTokenResponse.Failed(new Exception("An error occurred while retrieving an access token."));
            }
            return OAuthTokenResponse.Success(payload);
        }

        /// <summary>
        ///  Step 1：请求CODE 
        ///  构建用户授权地址
        /// </summary> 
        protected override string BuildChallengeUrl(AuthenticationProperties properties, string redirectUri)
        {
            return QueryHelpers.AddQueryString(Options.AuthorizationEndpoint, new Dictionary<string, string>
            {
                ["appid"] = Options.ClientId,
                ["scope"] = FormatScope(),
                ["response_type"] = "code",
                ["redirect_uri"] = redirectUri,
                ["state"] = Options.StateDataFormat.Protect(properties)
            });
        }

        protected override string FormatScope()
        {
            return string.Join(",", Options.Scope);
        }
    }
}