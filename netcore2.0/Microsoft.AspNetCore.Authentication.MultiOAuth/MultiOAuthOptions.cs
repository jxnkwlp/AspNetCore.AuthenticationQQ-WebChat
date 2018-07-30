using Microsoft.AspNetCore.Authentication.MultiOAuth.Events;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using System;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.Authentication.MultiOAuth
{
    /// <summary>
    /// Configuration options OAuth.
    /// </summary>
    public class MultiOAuthOptions : RemoteAuthenticationOptions
    {
        public MultiOAuthOptions()
        {
            Events = new MultiOAuthEvents();
        }

        /// <summary>
        /// Check that the options are valid.  Should throw an exception if things are not ok.
        /// </summary>
        public override void Validate()
        {
            base.Validate();

            if (string.IsNullOrEmpty(AuthorizationEndpoint))
            {
                throw new ArgumentException($"The '{nameof(AuthorizationEndpoint)}' option must be provided");
            }

            if (string.IsNullOrEmpty(TokenEndpoint))
            {
                throw new ArgumentException($"The '{nameof(TokenEndpoint)}' option must be provided");
            }

            if (!CallbackPath.HasValue)
            {
                throw new ArgumentException($"The '{nameof(CallbackPath)}' option must be provided");
            }
        }



        /// <summary>
        /// Gets or sets the URI where the client will be redirected to authenticate.
        /// </summary>
        public string AuthorizationEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the URI the middleware will access to exchange the OAuth token.
        /// </summary>
        public string TokenEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the URI the middleware will access to obtain the user information.
        /// This value is not used in the default implementation, it is for use in custom implementations of
        /// IOAuthAuthenticationEvents.Authenticated or OAuthAuthenticationHandler.CreateTicketAsync.
        /// </summary>
        public string UserInformationEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="OAuthEvents"/> used to handle authentication events.
        /// </summary>
        public new MultiOAuthEvents Events
        {
            get { return (MultiOAuthEvents)base.Events; }
            set { base.Events = value; }
        }

        /// <summary>
        /// A collection of claim actions used to select values from the json user data and create Claims.
        /// </summary>
        public ClaimActionCollection ClaimActions { get; } = new ClaimActionCollection();

        /// <summary>
        /// Gets the list of permissions to request.
        /// </summary>
        public ICollection<string> Scope { get; } = new HashSet<string>();

        /// <summary>
        /// Gets or sets the type used to secure data handled by the middleware.
        /// </summary>
        public ISecureDataFormat<AuthenticationProperties> StateDataFormat { get; set; }
    }
}
