# AspNetCore.Authentication.QQ-WebChat 

# .net core 2.0

- QQ   
~~~ csharp
 // startup.cs 
public void ConfigureServices(IServiceCollection services)
{
    // .... others code ...
    // config 
    services.AddAuthentication() 
        .AddQQAuthentication(options =>
        {
            options.ClientId = Configuration.GetValue<string>("[you app id]");
            options.ClientSecret = Configuration.GetValue<string>("[you app secret]");
        });

    // .... others code ...
}
~~~   

Then get current external login information when login success . eg: AccountController
~~~  csharp
// GET: /Account/ExternalLoginCallback
[HttpGet]
[AllowAnonymous]
public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
{ 
    // .... others code ...
    // .....
  
    // get information from AuthenticationManager
    var loginQQUserInfo = await HttpContext.GetExternalQQLoginInfoAsync();
    
    // todo ...
    // .... others code ...
}
 
~~~

- Webchat  [ Coding ... ]
  