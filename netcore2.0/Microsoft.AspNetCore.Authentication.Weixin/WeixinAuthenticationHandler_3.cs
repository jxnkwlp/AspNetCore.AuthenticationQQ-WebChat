#if NETCOREAPP3_0 || NETCOREAPP3_1

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Authentication.Weixin
{
	internal class WeixinAuthenticationHandler : OAuthHandler<WeixinAuthenticationOptions>
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
				["openid"] = tokens.Response.RootElement.GetProperty("openid").GetString()
			});

			var response = await Backchannel.GetAsync(address);
			if (!response.IsSuccessStatusCode)
			{
				Logger.LogError("An error occurred while retrieving the user profile: the remote server returned a {Status} response with the following payload: {Headers} {Body}.",
								/* Status: */ response.StatusCode,
								/* Headers: */ response.Headers.ToString(),
								/* Body: */ await response.Content.ReadAsStringAsync());

				throw new HttpRequestException("An error occurred while retrieving user information.");
			}

			string responseContent = await response.Content.ReadAsStringAsync();
			using (var payload = JsonDocument.Parse(responseContent))
			{
				string errorCode = payload.RootElement.GetString("errcode");
				if (!string.IsNullOrEmpty(errorCode))
				{
					Logger.LogError("An error occurred while retrieving the user profile: the remote server returned a {Status} response with the following payload: {Headers} {Body}.",
									/* Status: */ response.StatusCode,
									/* Headers: */ response.Headers.ToString(),
									/* Body: */ await response.Content.ReadAsStringAsync());

					throw new HttpRequestException("An error occurred while retrieving user information.");
				}

				if (payload.RootElement.TryGetProperty("unionid", out var unionidEle))
				{
					identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, unionidEle.GetString(), Options.ClaimsIssuer));
				}
				else
				{
					identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, payload.RootElement.GetString("openid"), Options.ClaimsIssuer));
				}

				if (payload.RootElement.TryGetProperty("privilege", out var privilegeEle))
				{
					string privilege = string.Join(",", privilegeEle.EnumerateArray().ToArray().Select(t => t.GetString()));

					identity.AddClaim(new Claim("urn:weixin:privilege", privilege, Options.ClaimsIssuer));
				}


				identity.AddClaim(new Claim("urn:weixin:user_info", payload.ToString(), Options.ClaimsIssuer));

				var context = new OAuthCreatingTicketContext(new ClaimsPrincipal(identity), properties, Context, Scheme, Options, Backchannel, tokens, payload.RootElement);

				context.RunClaimActions();

				await Events.CreatingTicket(context);

				return new AuthenticationTicket(context.Principal, context.Properties, Scheme.Name);
			}


		}

		/// <summary>
		/// Step 2：通过code获取access_token
		/// </summary> 
		protected override async Task<OAuthTokenResponse> ExchangeCodeAsync(OAuthCodeExchangeContext context)
		{
			var address = QueryHelpers.AddQueryString(Options.TokenEndpoint, new Dictionary<string, string>()
			{
				["appid"] = Options.ClientId,
				["secret"] = Options.ClientSecret,
				["code"] = context.Code,
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

			var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
			if (!string.IsNullOrEmpty(payload.RootElement.GetString("errcode")))
			{
				Logger.LogError("An error occurred while retrieving an access token: the remote server returned a {Status} response with the following payload: {Headers} {Body}.",
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
				["redirect_uri"] = redirectUri,
				["response_type"] = "code",
				["scope"] = FormatScope(),
				["state"] = Options.StateDataFormat.Protect(properties)
			});
		}

		protected override string FormatScope()
		{
			return string.Join(",", Options.Scope);
		}

	}
}

#endif