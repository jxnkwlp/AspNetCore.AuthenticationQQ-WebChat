using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.Authentication.MultiOAuth.Stores
{
    public interface IClientStore
    {
        /// <summary>
        ///  由 <paramref name="provider"/> 和 <paramref name="subjectId"/> 查找 <seealso cref="ClientStoreModel"/>
        /// </summary> 
        ClientStoreModel FindBySubjectId(string provider, string subjectId);
    }
}
