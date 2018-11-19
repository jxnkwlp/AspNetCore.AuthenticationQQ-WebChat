# Microsoft.AspNetCore.Authentication Extensions
QQ and Webchat extensions for Microsoft.AspNetCore.Authentication


# Get Started

- Install nuget package

  WebChat 

    ~~~
    PM> Install-Package Passingwind.AspNetCore.Authentication.Weixin
    ~~~ 
  QQ 

    ~~~
    PM> Install-Package Passingwind.AspNetCore.Authentication.QQ
    ~~~ 

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
            options.ClientId = "[you app id]";
            options.ClientSecret = "[you app secret]";
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
  
    // get information from HttpContext (using Microsoft.AspNetCore.Authentication.QQ;)
    var loginInfo = await HttpContext.GetExternalQQLoginInfoAsync();
    
    // todo ...
    // .... others code ...
}
 
~~~
- WebChat   
~~~ csharp
 // startup.cs 
public void ConfigureServices(IServiceCollection services)
{
    // .... others code ...
    // config 
    services.AddAuthentication() 
        .AddWeixinAuthentication(options =>
        {
            options.ClientId = "[you app id]";
            options.ClientSecret = "[you app secret]";
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
  
    // get information from HttpContext (using Microsoft.AspNetCore.Authentication.Weixin;)
    var loginInfo = await HttpContext.GetExternalWeixinLoginInfoAsync();
    
    // todo ...
    // .... others code ...
}
 
~~~

 

# Microsoft.AspNetCore.Authentication 扩展
QQ 和 微信 Microsoft.AspNetCore.Authentication 扩展

# 使用方法

- 安装 nuget 包

  微信 

    ~~~
    PM> Install-Package Passingwind.AspNetCore.Authentication.Weixin
    ~~~ 
  QQ 

    ~~~
    PM> Install-Package Passingwind.AspNetCore.Authentication.QQ
    ~~~ 

- QQ   
~~~ csharp
 // startup.cs 
public void ConfigureServices(IServiceCollection services)
{
    // .... others code ...
    // 配置 
    services.AddAuthentication() 
        .AddQQAuthentication(options =>
        {
            options.ClientId = "[you app id]";
            options.ClientSecret = "[you app secret]";
        });

    // .... others code ...
}
~~~   

获取登陆成功后的信息。  eg: AccountController
~~~  csharp
// GET: /Account/ExternalLoginCallback
[HttpGet]
[AllowAnonymous]
public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
{ 
    // .... others code ...
    // .....
  
    // 获取登录者信息 (using Microsoft.AspNetCore.Authentication.QQ;)
    var loginInfo = await HttpContext.GetExternalQQLoginInfoAsync();
    
    // todo ...
    // .... others code ...
}
 
~~~
- 微信   
~~~ csharp
 // startup.cs 
public void ConfigureServices(IServiceCollection services)
{
    // .... others code ...
    // 配置 
    services.AddAuthentication() 
        .AddWeixinAuthentication(options =>
        {
            options.ClientId = "[you app id]";
            options.ClientSecret = "[you app secret]";
        });

    // .... others code ...
}
~~~   

获取登陆成功后的信息。 eg: AccountController
~~~  csharp
// GET: /Account/ExternalLoginCallback
[HttpGet]
[AllowAnonymous]
public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
{ 
    // .... others code ...
    // .....
  
    // 获取登录者信息 (using Microsoft.AspNetCore.Authentication.Weixin;)
    var loginInfo = await HttpContext.GetExternalWeixinLoginInfoAsync();
    
    // todo ...
    // .... others code ...
}
 
~~~