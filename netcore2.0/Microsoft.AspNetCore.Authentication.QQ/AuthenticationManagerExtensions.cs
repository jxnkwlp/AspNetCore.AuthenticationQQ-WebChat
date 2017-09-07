using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

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

    #endregion
}
