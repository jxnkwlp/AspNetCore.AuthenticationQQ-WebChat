using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Authentication.QQ
{
	/// <summary> 
	/// </summary>
	public class QQAuthenticationOptions : OAuthOptions
	{
		public string OpenIdEndpoint { get; set; }

		public QQAuthenticationOptions()
		{
			ClaimsIssuer = QQAuthenticationDefaults.Issuer;
			CallbackPath = new PathString(QQAuthenticationDefaults.CallbackPath);

			AuthorizationEndpoint = QQAuthenticationDefaults.AuthorizationEndpoint;
			TokenEndpoint = QQAuthenticationDefaults.TokenEndpoint;
			UserInformationEndpoint = QQAuthenticationDefaults.UserInformationEndpoint;
			OpenIdEndpoint = QQAuthenticationDefaults.UserOpenIdEndpoint;

#if NETSTANDARD2_0

			ClaimActionCollectionMapExtensions.MapJsonKey(ClaimActions, ClaimTypes.NameIdentifier, "id");
			ClaimActionCollectionMapExtensions.MapJsonKey(ClaimActions, ClaimTypes.Name, "displayName");
#endif

#if NETCOREAPP3_0 || NETCOREAPP3_1

			ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
			ClaimActions.MapJsonKey(ClaimTypes.Name, "nickname");
			ClaimActions.MapJsonKey(ClaimTypes.Gender, "gender");

			ClaimActions.MapJsonKey("urn:qq:figureurl", "figureurl");
			ClaimActions.MapJsonKey("urn:qq:figureurl_1", "figureurl_1");
			ClaimActions.MapJsonKey("urn:qq:figureurl_2", "figureurl_2");
			ClaimActions.MapJsonKey("urn:qq:figureurl_qq_1", "figureurl_qq_1");
			ClaimActions.MapJsonKey("urn:qq:figureurl_qq_2", "figureurl_qq_2");
			ClaimActions.MapJsonKey("urn:qq:is_yellow_vip", "is_yellow_vip");
			ClaimActions.MapJsonKey("urn:qq:vip", "vip");
			ClaimActions.MapJsonKey("urn:qq:yellow_vip_level", "yellow_vip_level");
			ClaimActions.MapJsonKey("urn:qq:level", "level");
			ClaimActions.MapJsonKey("urn:qq:is_yellow_year_vip", "is_yellow_year_vip");
#endif
		}
	}
}