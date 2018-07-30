using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.Authentication.MultiOAuth.Stores
{
    public class StoreModel
    {
        //public string Provider { get; set; }

        public string SubjectId { get; set; }

        /// <summary>
        /// Gets or sets the provider-assigned client id.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the provider-assigned client secret.
        /// </summary>
        public string ClientSecret { get; set; }

    }
}
