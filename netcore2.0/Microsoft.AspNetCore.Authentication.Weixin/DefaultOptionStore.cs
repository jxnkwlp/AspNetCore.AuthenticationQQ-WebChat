//using Microsoft.AspNetCore.Authentication.Weixin.Stores;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Microsoft.AspNetCore.Authentication.Weixin
//{
//    public class DefaultOptionStore : IOptionStore
//    {
//        private readonly IClientStore _clientStore;

//        public DefaultOptionStore(IClientStore clientStore)
//        {
//            _clientStore = clientStore;

//        }

//        public WeixinAuthenticationOptions GetOptionsByClientId(string clientId)
//        {
//            throw new NotImplementedException();
//        }

//        public WeixinAuthenticationOptions GetOptionsBySubject(string subjectId)
//        {
//            var client = _clientStore.FindBySubjectId(subjectId);

//            return new WeixinAuthenticationOptions()
//            {
//                ClientId = client.ClientId,
//                ClientSecret = client.ClientSecret,
//            };
//        }
//    }
//}
