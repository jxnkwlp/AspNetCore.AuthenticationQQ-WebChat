using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features.Authentication;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
#if NETSTANDARD2_0
using Microsoft.AspNetCore.Http.Authentication;
using Newtonsoft.Json.Linq;
#endif
#if NETCOREAPP3_0
using System.Text.Json;
#endif

namespace Microsoft.AspNetCore.Authentication.QQ
{
	public static class HttpContextExtensions
	{
		/// <summary>
		///  Get the external login information from qq provider.
		/// </summary>
		public static async Task<Dictionary<string, string>> GetExternalQQLoginInfoAsync(this HttpContext httpContext, string expectedXsrf = null)
		{
			var auth = await httpContext.AuthenticateAsync(QQAuthenticationDefaults.AuthenticationScheme);

			var items = auth?.Properties?.Items;
			if (auth?.Principal == null || items == null || !items.ContainsKey("LoginProvider"))
			{
				return null;
			}

			if (expectedXsrf != null)
			{
				if (!items.ContainsKey("XsrfId"))
				{
					return null;
				}
				var userId = items["XsrfId"] as string;
				if (userId != expectedXsrf)
				{
					return null;
				}
			}

			var userInfo = auth.Principal.FindFirst("urn:qq:user_info");
			if (userInfo == null)
			{
				return null;
			}

			if (!string.IsNullOrEmpty(userInfo.Value))
			{
				return GetUserInfo(userInfo.Value);
			}

			return null;
		}

		private static Dictionary<string, string> GetUserInfo(string json)
		{
			Dictionary<string, string> dict = new Dictionary<string, string>();

#if NETSTANDARD2_0
			var jObject = JObject.Parse(json);

			foreach (var item in jObject)
			{
				dict[item.Key] = item.Value?.ToString();
			}
#endif

#if NETCOREAPP3_0
			var document = JsonDocument.Parse(json);

			foreach (var item in document.RootElement.EnumerateObject())
			{
				dict[item.Name] = item.Value.GetString();
			}
#endif
			return dict;
		}
	}

#if NETSTANDARD2_0

	#region Old

	public static class AuthenticationManagerExtensions
	{
		/// <summary>
		///  Get the external login information from qq provider.
		/// </summary>
		[Obsolete("Use HttpContext.GetExternalQQLoginInfoAsync()")]
		public static async Task<Dictionary<string, string>> GetExternalQQLoginInfoAsync(this AuthenticationManager authenticationManager, string expectedXsrf = null)
		{
			AuthenticateContext authenticateContext = new AuthenticateContext(QQAuthenticationDefaults.AuthenticationScheme);
			await authenticationManager.AuthenticateAsync(authenticateContext);

			if (authenticateContext.Principal == null || authenticateContext.Properties == null || !authenticateContext.Properties.ContainsKey("LoginProvider"))
			{
				return null;
			}

			if (expectedXsrf != null)
			{
				if (!authenticateContext.Properties.ContainsKey("XsrfId"))
				{
					return null;
				}
				if (authenticateContext.Properties["XsrfId"] != expectedXsrf)
				{
					return null;
				}
			}

			var userInfo = authenticateContext.Principal.FindFirst("urn:qq:user_info");
			if (userInfo == null)
			{
				return null;
			}

			if (!string.IsNullOrEmpty(userInfo.Value))
			{
				var jObject = JObject.Parse(userInfo.Value);

				Dictionary<string, string> dict = new Dictionary<string, string>();

				foreach (var item in jObject)
				{
					dict[item.Key] = item.Value?.ToString();
				}

				return dict;
			}

			return null;
		}
	}

	#endregion Old

#endif
}