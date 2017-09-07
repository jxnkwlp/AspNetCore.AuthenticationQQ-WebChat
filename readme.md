# AspNetCore.Authentication.QQ-WebChat 

- QQ   
~~~ csharp
 // startup.cs 
public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
// config 
 app.UseQQAuthentication(new Microsoft.AspNetCore.Authentication.QQ.QQAuthenticationOptions()
            {
                ClientId = "[you client id]",
                ClientSecret ="[you client Secret]",
            });

 // .... others ...
}
~~~   

Then get current external qq login information. eg: AccountController
~~~  csharp
// GET: /Account/ExternalLoginCallback
[HttpGet]
[AllowAnonymous]
public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
{ 
    // others code ...
    // .....
  
    // get information from AuthenticationManager
    var loginQQUserInfo = await HttpContext.Authentication.GetExternalQQLoginInfoAsync();
    
    // todo ...
    // others code ...
}

var loginQQUserInfo = await HttpContext.Authentication.GetExternalQQLoginInfoAsync(); 

~~~

- Webchat  [ Coding ... ]
  