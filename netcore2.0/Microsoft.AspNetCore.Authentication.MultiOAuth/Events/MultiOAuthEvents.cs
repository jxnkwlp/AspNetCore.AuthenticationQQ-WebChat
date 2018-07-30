using System;
using System.Threading.Tasks;

// https://github.com/aspnet/Security/blob/2.1.0/src/Microsoft.AspNetCore.Authentication.OAuth/Events/OAuthEvents.cs
namespace Microsoft.AspNetCore.Authentication.MultiOAuth.Events
{
    /// <summary>
    /// Default implementation.
    /// </summary>
    public class MultiOAuthEvents : RemoteAuthenticationEvents
    {
        /// <summary>
        /// Gets or sets the function that is invoked when the CreatingTicket method is invoked.
        /// </summary>
        public Func<MultiOAuthCreatingTicketContext, Task> OnCreatingTicket { get; set; } = context => Task.CompletedTask;

        /// <summary>
        /// Gets or sets the delegate that is invoked when the RedirectToAuthorizationEndpoint method is invoked.
        /// </summary>
        public Func<RedirectContext<MultiOAuthOptions>, Task> OnRedirectToAuthorizationEndpoint { get; set; } = context =>
        {
            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        };

        /// <summary>
        /// Invoked after the provider successfully authenticates a user.
        /// </summary>
        /// <param name="context">Contains information about the login session as well as the user <see cref="System.Security.Claims.ClaimsIdentity"/>.</param>
        /// <returns>A <see cref="Task"/> representing the completed operation.</returns>
        public virtual Task CreatingTicket(MultiOAuthCreatingTicketContext context) => OnCreatingTicket(context);

        /// <summary>
        /// Called when a Challenge causes a redirect to authorize endpoint in the OAuth handler.
        /// </summary>
        /// <param name="context">Contains redirect URI and <see cref="AuthenticationProperties"/> of the challenge.</param>
        public virtual Task RedirectToAuthorizationEndpoint(RedirectContext<MultiOAuthOptions> context) => OnRedirectToAuthorizationEndpoint(context);
    }
}
