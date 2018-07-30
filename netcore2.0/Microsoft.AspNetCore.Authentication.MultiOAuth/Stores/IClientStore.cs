using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.Authentication.MultiOAuth.Stores
{
    public interface IClientStore
    {
        StoreModel FindBySubjectId(string subjectId);
    }
}
