using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Authentication.Weixin
{
	/// <summary>
	/// Defines a set of options used by <see cref="WeixinAuthenticationHandler"/>.
	/// </summary>
	public class WeixinAuthenticationOptions : OAuthOptions
	{
		public WeixinAuthenticationOptions()
		{
			ClaimsIssuer = WeixinAuthenticationDefaults.Issuer;
			CallbackPath = new PathString(WeixinAuthenticationDefaults.CallbackPath);

			AuthorizationEndpoint = WeixinAuthenticationDefaults.AuthorizationEndpoint;
			TokenEndpoint = WeixinAuthenticationDefaults.TokenEndpoint;
			UserInformationEndpoint = WeixinAuthenticationDefaults.UserInformationEndpoint;

			Scope.Add("snsapi_login");

#if NETCOREAPP3_0 || NETCOREAPP3_1

			ClaimActions.MapJsonKey(ClaimTypes.Name, "nickname");
			ClaimActions.MapJsonKey(ClaimTypes.Gender, "sex");
			ClaimActions.MapJsonKey(ClaimTypes.Country, "country");

			ClaimActions.MapJsonKey("urn:weixin:nickname", "nickname");
			ClaimActions.MapJsonKey("urn:weixin:city", "city");
			ClaimActions.MapJsonKey("urn:weixin:province", "province");
			ClaimActions.MapJsonKey("urn:weixin:headimgurl", "headimgurl");

#endif

		}
	}
}