# Microsoft.AspNetCore.Authentication Extensions
QQ and Webchat extensions for Microsoft.AspNetCore.Authentication

# Get Started

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

    // .... others code ...
}
~~~   

Then get external login information when login success . eg:  AccountController
~~~  csharp
// GET: /Account/ExternalLoginCallback
[HttpGet]
[AllowAnonymous]
public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
{ 
    // .... others code ...
    // .....
  
    // get information from AuthenticationManager (using Microsoft.AspNetCore.Authentication.QQ;)
    var loginInfo = await HttpContext.Authentication.GetExternalQQLoginInfoAsync();
    
    // todo ...
    // .... others code ...
}
~~~

- Webchat
~~~ csharp
 // startup.cs 
public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
    // config 
    app.UseWeixinAuthentication(new Microsoft.AspNetCore.Authentication.Weixin.WeixinAuthenticationOptions()
            {
                ClientId = "[you client id]",
                ClientSecret ="[you client Secret]",
            });

    // .... others code ...
}
~~~   

Then get external login information when login success . eg:  AccountController
~~~  csharp
// GET: /Account/ExternalLoginCallback
[HttpGet]
[AllowAnonymous]
public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
{ 
    // .... others code ...
    // .....
  
    // get information from AuthenticationManager (using Microsoft.AspNetCore.Authentication.Weixin;)
    var loginInfo = await HttpContext.Authentication.GetExternalWeixinLoginInfoAsync();
    
    // todo ...
    // .... others code ...
}
~~~
 


     

# Microsoft.AspNetCore.Authentication 扩展
QQ 和 微信 Microsoft.AspNetCore.Authentication 扩展

# 使用方法

- QQ   
~~~ csharp
 // startup.cs 
public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
    // 配置 
    app.UseQQAuthentication(new Microsoft.AspNetCore.Authentication.QQ.QQAuthenticationOptions()
            {
                ClientId = "[you client id]",
                ClientSecret ="[you client Secret]",
            });

    // .... others code ...
}
~~~   

获取登陆成功后的信息 . eg:  AccountController
~~~  csharp
// GET: /Account/ExternalLoginCallback
[HttpGet]
[AllowAnonymous]
public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
{ 
    // .... others code ...
    // .....
  
    // 获取登录者信息 (using Microsoft.AspNetCore.Authentication.QQ;)
    var loginInfo = await HttpContext.Authentication.GetExternalQQLoginInfoAsync();
    
    // todo ...
    // .... others code ...
}
~~~

- 微信
~~~ csharp
 // startup.cs 
public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
    // 配置 
    app.UseWeixinAuthentication(new Microsoft.AspNetCore.Authentication.Weixin.WeixinAuthenticationOptions()
            {
                ClientId = "[you client id]",
                ClientSecret ="[you client Secret]",
            });

    // .... others code ...
}
~~~   

获取登陆成功后的信息 . eg:  AccountController
~~~  csharp
// GET: /Account/ExternalLoginCallback
[HttpGet]
[AllowAnonymous]
public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
{ 
    // .... others code ...
    // .....
  
    // 获取登录者信息 (using Microsoft.AspNetCore.Authentication.Weixin;)
    var loginInfo = await HttpContext.Authentication.GetExternalWeixinLoginInfoAsync();
    
    // todo ...
    // .... others code ...
}
~~~