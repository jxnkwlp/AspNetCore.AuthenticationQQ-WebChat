using Microsoft.AspNetCore.Authentication.OAuth;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using Microsoft.AspNetCore.Http.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Microsoft.AspNetCore.Authentication.QQ
{
    public class QQAuthenticationHandler : OAuthHandler<QQAuthenticationOptions>
    {
        public QQAuthenticationHandler(HttpClient client) : base(client)
        {

        }

        protected override async Task<AuthenticationTicket> CreateTicketAsync(ClaimsIdentity identity, AuthenticationProperties properties, OAuthTokenResponse tokens)
        {
            // 获取用户OpenID
            var userOpenId = await ObtainUserOpenIdAsync(tokens);
            if (string.IsNullOrWhiteSpace(userOpenId))
            {
                throw new HttpRequestException("User openId was not found.");
            }

            // 获取用户基本信息
            var address = QueryHelpers.AddQueryString(Options.UserInformationEndpoint, new Dictionary<string, string>
            {
                ["access_token"] = tokens.AccessToken,
                ["oauth_consumer_key"] = Options.ClientId,
                ["openid"] = userOpenId,
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
            if (payload.Value<int>("ret") != 0)
            {
                Logger.LogError("An error occurred while retrieving the user profile: the remote server " +
                                "returned a {Status} response with the following payload: {Headers} {Body}.",
                                /* Status: */ response.StatusCode,
                                /* Headers: */ response.Headers.ToString(),
                                /* Body: */ await response.Content.ReadAsStringAsync());

                throw new HttpRequestException("An error occurred while retrieving user information.");
            }

            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, tokens.Response.Value<string>("openid"), Options.ClaimsIssuer));
            identity.AddClaim(new Claim(ClaimTypes.Name, QQAuthenticationHelper.GetNickname(payload), Options.ClaimsIssuer));
            identity.AddClaim(new Claim(ClaimTypes.Gender, QQAuthenticationHelper.GetGender(payload), Options.ClaimsIssuer));
            identity.AddClaim(new Claim("urn:qq:openid", QQAuthenticationHelper.GetOpenId(payload), Options.ClaimsIssuer));

            identity.AddClaim(new Claim("urn:qq:figureurl", QQAuthenticationHelper.GetFigureUrl(payload), Options.ClaimsIssuer));
            identity.AddClaim(new Claim("urn:qq:figureurl_1", QQAuthenticationHelper.GetFigureUrl_1(payload), Options.ClaimsIssuer));
            identity.AddClaim(new Claim("urn:qq:figureurl_2", QQAuthenticationHelper.GetFigureUrl_2(payload), Options.ClaimsIssuer));
            identity.AddClaim(new Claim("urn:qq:figureurl_qq_1", QQAuthenticationHelper.GetFigureUrl_QQ_1(payload), Options.ClaimsIssuer));
            identity.AddClaim(new Claim("urn:qq:figureurl_qq_2", QQAuthenticationHelper.GetFigureUrl_QQ_2(payload), Options.ClaimsIssuer));
            identity.AddClaim(new Claim("urn:qq:gender", QQAuthenticationHelper.GetGender(payload), Options.ClaimsIssuer));
            identity.AddClaim(new Claim("urn:qq:is_yellow_vip", QQAuthenticationHelper.GetIsYellowVip(payload), Options.ClaimsIssuer));
            identity.AddClaim(new Claim("urn:qq:vip", QQAuthenticationHelper.GetIsVip(payload), Options.ClaimsIssuer));
            identity.AddClaim(new Claim("urn:qq:yellow_vip_level", QQAuthenticationHelper.GetYellowVipLevel(payload), Options.ClaimsIssuer));
            identity.AddClaim(new Claim("urn:qq:level", QQAuthenticationHelper.GetLevel(payload), Options.ClaimsIssuer));
            identity.AddClaim(new Claim("urn:qq:is_yellow_year_vip", QQAuthenticationHelper.GetIsYellowYearVip(payload), Options.ClaimsIssuer));

            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, properties, Options.AuthenticationScheme);

            var context = new OAuthCreatingTicketContext(ticket, Context, Options, Backchannel, tokens);

            await Options.Events.CreatingTicket(context);
            return context.Ticket;
        }



        /// <summary>
        ///  Step2：通过Authorization Code获取Access Token
        ///  http://wiki.connect.qq.com/%E4%BD%BF%E7%94%A8authorization_code%E8%8E%B7%E5%8F%96access_token
        /// </summary> 
        protected override async Task<OAuthTokenResponse> ExchangeCodeAsync(string code, string redirectUri)
        {
            var address = QueryHelpers.AddQueryString(Options.TokenEndpoint, new Dictionary<string, string>()
            {
                ["client_id"] = Options.ClientId,
                ["client_secret"] = Options.ClientSecret,
                ["code"] = code,
                ["grant_type"] = "authorization_code",
                ["redirect_uri"] = redirectUri,
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
            if (!string.IsNullOrEmpty(payload.Value<string>("msg")))
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
        ///  Step3：通过Access Token获取OpenId
        /// </summary>
        /// <param name="tokens"></param>
        protected async Task<string> ObtainUserOpenIdAsync(OAuthTokenResponse tokens)
        {
            var address = QueryHelpers.AddQueryString(Options.OpenIdEndpoint, new Dictionary<string, string>
            {
                ["access_token"] = tokens.AccessToken,
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

            string oauthTokenResponse = await response.Content.ReadAsStringAsync();

            // callback( {"client_id":"YOUR_APPID","openid":"YOUR_OPENID"} );\n

            oauthTokenResponse = oauthTokenResponse.Remove(0, 9);
            oauthTokenResponse = oauthTokenResponse.Remove(oauthTokenResponse.Length - 3);

            JObject oauth2Token = JObject.Parse(oauthTokenResponse);

            return oauth2Token.Value<string>("client_id");
        }

        protected override string FormatScope()
        {
            return string.Join(",", Options.Scope);
        }
    }
}
