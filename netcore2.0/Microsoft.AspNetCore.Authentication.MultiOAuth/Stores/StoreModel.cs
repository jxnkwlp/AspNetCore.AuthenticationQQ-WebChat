using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.Authentication.MultiOAuth.Stores
{
    /// <summary>
    ///  表示一个 Client 信息 
    /// </summary>
    public class ClientStoreModel
    {
        public string Provider { get; set; }

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
