using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Authentication.Weixin
{
    public static class AuthenticationManagerExtensions
    {
        /// <summary> 
        ///  Get the external login information from weixin provider.
        /// </summary> 
        public static async Task<Dictionary<string, string>> GetExternalWeixinLoginInfoAsync(this AuthenticationManager authenticationManager, string expectedXsrf = null)
        {
            AuthenticateContext authenticateContext = new AuthenticateContext(WeixinAuthenticationDefaults.AuthenticationScheme);
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

            var userInfo = authenticateContext.Principal.FindFirst("urn:weixin:user_info");
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
}
