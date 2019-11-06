#if NETCOREAPP3_0

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Authentication.QQ
{
	/// <summary>
	/// </summary>
	internal class QQAuthenticationHandler : OAuthHandler<QQAuthenticationOptions>
	{
		public QQAuthenticationHandler(IOptionsMonitor<QQAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
		{
		}

		/// <summary>
		///  Last Step
		/// </summary>
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

			var responseContent = await response.Content.ReadAsStringAsync();
			using (var payload = JsonDocument.Parse(responseContent))
			{
				if (payload.RootElement.TryGetProperty("ret", out var result))
				{
					if (result.GetInt32() != 0)
					{
						Logger.LogError("An error occurred while retrieving the user profile: the remote server " +
										"returned a {Status} response with the following payload: {Headers} {Body}.",
										/* Status: */ response.StatusCode,
										/* Headers: */ response.Headers.ToString(),
										/* Body: */ responseContent);

						throw new HttpRequestException("An error occurred while retrieving user information.");
					}
				}

				identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userOpenId, Options.ClaimsIssuer));
				identity.AddClaim(new Claim("urn:qq:openid", userOpenId, Options.ClaimsIssuer));
				identity.AddClaim(new Claim("urn:qq:user_info", responseContent, Options.ClaimsIssuer));

				var context = new OAuthCreatingTicketContext(new ClaimsPrincipal(identity), properties, Context, Scheme, Options, Backchannel, tokens, payload.RootElement);

				context.RunClaimActions();

				await Events.CreatingTicket(context);

				return new AuthenticationTicket(context.Principal, context.Properties, Scheme.Name);
			}
		}

		/// <summary>
		///  Step2：通过Authorization Code获取Access Token
		///  http://wiki.connect.qq.com/%E4%BD%BF%E7%94%A8authorization_code%E8%8E%B7%E5%8F%96access_token
		/// </summary>
		protected override async Task<OAuthTokenResponse> ExchangeCodeAsync(OAuthCodeExchangeContext context)
		{
			var address = QueryHelpers.AddQueryString(Options.TokenEndpoint, new Dictionary<string, string>()
			{
				["client_id"] = Options.ClientId,
				["client_secret"] = Options.ClientSecret,
				["code"] = context.Code,
				["grant_type"] = "authorization_code",
				["redirect_uri"] = context.RedirectUri,
			});

			var response = await Backchannel.GetAsync(address);

			if (!response.IsSuccessStatusCode)
			{
				Logger.LogError("An error occurred while retrieving an access token: the remote server returned a {Status} response with the following payload: {Headers} {Body}.",
								/* Status: */ response.StatusCode,
								/* Headers: */ response.Headers.ToString(),
								/* Body: */ await response.Content.ReadAsStringAsync());

				return OAuthTokenResponse.Failed(new Exception("An error occurred while retrieving an access token."));
			}

			// 成功：  access_token=FE04************************CCE2&expires_in=7776000&refresh_token=88E4************************BE14
			// 失败：  callback( {"error":123456 ,"error_description":"**************"} );

			var responseString = await response.Content.ReadAsStringAsync();

			if (responseString.StartsWith("callback"))
			{
				Logger.LogError("An error occurred while retrieving an access token: the remote server " +
								"returned a {Status} response with the following payload: {Headers} {Body}.",
								/* Status: */ response.StatusCode,
								/* Headers: */ response.Headers.ToString(),
								/* Body: */ await response.Content.ReadAsStringAsync());

				return OAuthTokenResponse.Failed(new Exception("An error occurred while retrieving an access token."));
			}

			JsonDocument payload = JsonDocument.Parse("{}");

			var responseParams = responseString.Split('&');

			var buffer = new ArrayBufferWriter<byte>();
			var write = new Utf8JsonWriter(buffer);

			foreach (var parm in responseParams)
			{
				var kv = parm.Split('=');

				write.WritePropertyName(kv[0]);
				write.WriteStringValue(kv[1]);
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
				Logger.LogError("An error occurred while retrieving the user open id: the remote server " +
								"returned a {Status} response with the following payload: {Headers} {Body}.",
								/* Status: */ response.StatusCode,
								/* Headers: */ response.Headers.ToString(),
								/* Body: */ await response.Content.ReadAsStringAsync());

				throw new HttpRequestException("An error occurred while retrieving user information.");
			}

			string responseString = await response.Content.ReadAsStringAsync();

			// callback( {"client_id":"YOUR_APPID","openid":"YOUR_OPENID"} );\n

			responseString = responseString.Remove(0, 9);
			responseString = responseString.Remove(responseString.Length - 3);

			JsonDocument document = JsonDocument.Parse(responseString);

			return document.RootElement.GetProperty("openid").GetString();
		}

		protected override string FormatScope()
		{
			return string.Join(",", Options.Scope);
		}

		/// <summary>
		///  Step1：获取Authorization Code
		///  构建请求地址
		/// </summary>
		/// <param name="properties"></param>
		/// <param name="redirectUri"></param>
		/// <returns></returns>
		protected override string BuildChallengeUrl(AuthenticationProperties properties, string redirectUri)
		{
			var url = base.BuildChallengeUrl(properties, redirectUri);
			return url;
		}
	}
}

#endif