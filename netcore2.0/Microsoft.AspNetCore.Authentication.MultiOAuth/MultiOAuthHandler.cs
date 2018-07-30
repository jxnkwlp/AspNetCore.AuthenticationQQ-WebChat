using Microsoft.AspNetCore.Authentication.MultiOAuth.Events;
using Microsoft.AspNetCore.Authentication.MultiOAuth.Stores;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Authentication.MultiOAuth
{
    /// <summary>
    ///  
    /// https://github.com/aspnet/Security/blob/master/src/Microsoft.AspNetCore.Authentication.OAuth/OAuthHandler.cs
    /// </summary>
    public class MultiOAuthHandler<TMultiOAuthOptions> : RemoteAuthenticationHandler<TMultiOAuthOptions> where TMultiOAuthOptions : MultiOAuthOptions, new()
    {
        protected HttpClient Backchannel => Options.Backchannel;

        protected IClientStore ClientStore { get; private set; }

        /// <summary>
        /// The handler calls methods on the events which give the application control at certain points where processing is occurring. 
        /// If it is not provided a default instance is supplied which does nothing when the methods are called.
        /// </summary>
        protected new MultiOAuthEvents Events
        {
            get { return (MultiOAuthEvents)base.Events; }
            set { base.Events = value; }
        }

        public MultiOAuthHandler(IOptionsMonitor<TMultiOAuthOptions> options, IClientStore clientStore, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
            this.ClientStore = clientStore;

        }

        public override Task<bool> ShouldHandleRequestAsync()
        {
            return Task.FromResult(Request.Path.Value.StartsWith(Options.CallbackPath.Add("/subject")));
        }

        /// <summary>
        /// Creates a new instance of the events instance.
        /// </summary>
        /// <returns>A new instance of the events instance.</returns>
        protected override Task<object> CreateEventsAsync() => Task.FromResult<object>(new OAuthEvents());


        protected override async Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
        {
            // /signin-xxx/subject/xxx

            var callbackPath = Options.CallbackPath.Add("/subject").Value;

            var query = Request.Query;
            var subjectId = Request.Path.Value.Remove(0, callbackPath.Length + 1);

            var state = query["state"];
            var properties = Options.StateDataFormat.Unprotect(state);

            if (properties == null)
            {
                return HandleRequestResult.Fail("The oauth state was missing or invalid.");
            }

            // OAuth2 10.12 CSRF
            if (!ValidateCorrelationId(properties))
            {
                return HandleRequestResult.Fail("Correlation failed.", properties);
            }

            var error = query["error"];
            if (!StringValues.IsNullOrEmpty(error))
            {
                var failureMessage = new StringBuilder();
                failureMessage.Append(error);
                var errorDescription = query["error_description"];
                if (!StringValues.IsNullOrEmpty(errorDescription))
                {
                    failureMessage.Append(";Description=").Append(errorDescription);
                }
                var errorUri = query["error_uri"];
                if (!StringValues.IsNullOrEmpty(errorUri))
                {
                    failureMessage.Append(";Uri=").Append(errorUri);
                }

                return HandleRequestResult.Fail(failureMessage.ToString(), properties);
            }

            var code = query["code"];

            if (StringValues.IsNullOrEmpty(code))
            {
                return HandleRequestResult.Fail("Code was not found.", properties);
            }

            var tokens = await ExchangeCodeAsync(subjectId, code, BuildRedirectUri(Options.CallbackPath, subjectId));

            if (tokens.Error != null)
            {
                return HandleRequestResult.Fail(tokens.Error, properties);
            }

            if (string.IsNullOrEmpty(tokens.AccessToken))
            {
                return HandleRequestResult.Fail("Failed to retrieve access token.", properties);
            }

            var identity = new ClaimsIdentity(ClaimsIssuer);

            if (Options.SaveTokens)
            {
                var authTokens = new List<AuthenticationToken>();

                authTokens.Add(new AuthenticationToken { Name = "access_token", Value = tokens.AccessToken });
                if (!string.IsNullOrEmpty(tokens.RefreshToken))
                {
                    authTokens.Add(new AuthenticationToken { Name = "refresh_token", Value = tokens.RefreshToken });
                }

                if (!string.IsNullOrEmpty(tokens.TokenType))
                {
                    authTokens.Add(new AuthenticationToken { Name = "token_type", Value = tokens.TokenType });
                }

                if (!string.IsNullOrEmpty(tokens.ExpiresIn))
                {
                    int value;
                    if (int.TryParse(tokens.ExpiresIn, NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
                    {
                        // https://www.w3.org/TR/xmlschema-2/#dateTime
                        // https://msdn.microsoft.com/en-us/library/az4se3k1(v=vs.110).aspx
                        var expiresAt = Clock.UtcNow + TimeSpan.FromSeconds(value);
                        authTokens.Add(new AuthenticationToken
                        {
                            Name = "expires_at",
                            Value = expiresAt.ToString("o", CultureInfo.InvariantCulture)
                        });
                    }
                }

                properties.StoreTokens(authTokens);
            }

            var ticket = await CreateTicketAsync(identity, properties, tokens);
            if (ticket != null)
            {
                return HandleRequestResult.Success(ticket);
            }
            else
            {
                return HandleRequestResult.Fail("Failed to retrieve user information from remote server.", properties);
            }
        }

        protected virtual async Task<OAuthTokenResponse> ExchangeCodeAsync(string subjectId, string code, string redirectUri)
        {
            var clientStore = GetClientStore(subjectId);

            var tokenRequestParameters = new Dictionary<string, string>()
            {
                { "client_id", clientStore.ClientId },
                { "client_secret", clientStore.ClientSecret },

                { "redirect_uri", redirectUri },
                { "code", code },
                { "grant_type", "authorization_code" },
            };

            var requestContent = new FormUrlEncodedContent(tokenRequestParameters);

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, Options.TokenEndpoint);
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requestMessage.Content = requestContent;
            var response = await Backchannel.SendAsync(requestMessage, Context.RequestAborted);
            if (response.IsSuccessStatusCode)
            {
                var payload = JObject.Parse(await response.Content.ReadAsStringAsync());
                return OAuthTokenResponse.Success(payload);
            }
            else
            {
                var error = "OAuth token endpoint failure: " + await Display(response);
                return OAuthTokenResponse.Failed(new Exception(error));
            }
        }

        private static async Task<string> Display(HttpResponseMessage response)
        {
            var output = new StringBuilder();
            output.Append("Status: " + response.StatusCode + ";");
            output.Append("Headers: " + response.Headers.ToString() + ";");
            output.Append("Body: " + await response.Content.ReadAsStringAsync() + ";");
            return output.ToString();
        }

        protected virtual async Task<AuthenticationTicket> CreateTicketAsync(ClaimsIdentity identity, AuthenticationProperties properties, OAuthTokenResponse tokens)
        {
            var context = new MultiOAuthCreatingTicketContext(new ClaimsPrincipal(identity), properties, Context, Scheme, Options, Backchannel, tokens);
            await Events.CreatingTicket(context);
            return new AuthenticationTicket(context.Principal, context.Properties, Scheme.Name);
        }

        // step 1
        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            string subjectId = properties.Items["subjectId"];
            SetSubjectIdToContext(subjectId);

            if (string.IsNullOrEmpty(properties.RedirectUri))
            {
                properties.RedirectUri = CurrentUri;
            }

            // OAuth2 10.12 CSRF
            GenerateCorrelationId(properties);

            var clientStore = GetClientStore();

            var authorizationEndpoint = BuildChallengeUrl(properties, BuildRedirectUri(Options.CallbackPath, subjectId), clientStore);
            var redirectContext = new RedirectContext<MultiOAuthOptions>(
                Context, Scheme, Options,
                properties, authorizationEndpoint);
            await Events.RedirectToAuthorizationEndpoint(redirectContext);
        }

        protected virtual string BuildChallengeUrl(AuthenticationProperties properties, string redirectUri, StoreModel storeModel)
        {
            var scopeParameter = properties.GetParameter<ICollection<string>>(OAuthChallengeProperties.ScopeKey);
            var scope = scopeParameter != null ? FormatScope(scopeParameter) : FormatScope();

            var state = Options.StateDataFormat.Protect(properties);
            var parameters = new Dictionary<string, string>
            {
                { "client_id", storeModel.ClientId },
                { "scope", scope },
                { "response_type", "code" },
                { "redirect_uri", redirectUri },
                { "state", state },
            };
            return QueryHelpers.AddQueryString(Options.AuthorizationEndpoint, parameters);
        }

        protected string BuildRedirectUri(string targetPath, string subjectId)
        {
            return this.Request.Scheme + "://" + this.Request.Host + this.OriginalPathBase + targetPath + "/subject/" + subjectId;
        }


        /// <summary>
        /// Format a list of OAuth scopes.
        /// </summary>
        /// <param name="scopes">List of scopes.</param>
        /// <returns>Formatted scopes.</returns>
        protected virtual string FormatScope(IEnumerable<string> scopes)
            => string.Join(" ", scopes); // OAuth2 3.3 space separated

        /// <summary>
        /// Format the <see cref="OAuthOptions.Scope"/> property.
        /// </summary>
        /// <returns>Formatted scopes.</returns>
        /// <remarks>Subclasses should rather override <see cref="FormatScope(IEnumerable{string})"/>.</remarks>
        protected virtual string FormatScope()
            => FormatScope(Options.Scope);


        //protected virtual string BuilderCallbackUrl(string subjectId) => Options.CallbackPath.Add(subjectId);


        protected virtual void SetSubjectIdToContext(string subjectId)
        {
            this.Context.Items["MultiOAuthHandler:subjectId"] = subjectId;
        }

        protected virtual string GetSubjectIdFromContext()
        {
            return this.Context.Items["MultiOAuthHandler:subjectId"] as string;
        }

        protected virtual StoreModel GetClientStore()
        {
            var subjectId = GetSubjectIdFromContext();

            if (string.IsNullOrWhiteSpace(subjectId))
            {
                throw new ArgumentNullException("subjectId");
            }

            var store = ClientStore.FindBySubjectId(subjectId);

            if (store == null)
            {
                throw new ArgumentNullException($"can not found '{subjectId}' store config.");
            }

            return store;
        }

        protected virtual StoreModel GetClientStore(string subjectId)
        {
            if (string.IsNullOrWhiteSpace(subjectId))
            {
                throw new ArgumentNullException("subjectId");
            }

            var store = ClientStore.FindBySubjectId(subjectId);

            if (store == null)
            {
                throw new ArgumentNullException($"can not found '{subjectId}' store config.");
            }

            return store;
        }

    }
}
