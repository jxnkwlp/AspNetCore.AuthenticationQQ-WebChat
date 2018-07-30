# MultiOAuth

config multi oauth client (example only weixin implement) , use 'SubjectId' to distinguish between different clients  .
动态 oauth 客户端配置（当前实现了微信登录，可以配置多个appid），使用 'SubjectId' 来区分不同的客户端

# Useage

1. config clients.  eg: define `WeixinClientStore` class implement `IClientStore`  .   配置 clients, 实现`IClientStore` 接口 

~~~ csharp  
public class WeixinClientStore : IClientStore
{
    List<StoreModel> _stores = new List<StoreModel>() {
        new StoreModel()
        {
            ClientId = "CLIENT ID",
            ClientSecret = "CLIENT SECTET",
            SubjectId = "client1" // eg client1
        },
        new StoreModel()
        {
            ClientId = "CLIENT ID",
            ClientSecret = "CLIENT SECTET",
            SubjectId = "client2" // eg client1
        },
    };

    // implement this method .  用于查找配置
    public StoreModel FindBySubjectId(string subjectId)
    {
        return _stores.FirstOrDefault(t => t.SubjectId == subjectId);
    }
}
~~~  


2. add the service .  添加到服务    
~~~ csharp 
 // startup.cs 
public void ConfigureServices(IServiceCollection services)
{
    // .... others code ...
    // config 
    services.AddAuthentication() 
        .AddWeixinAuthenticationStore<WeixinClientStore>(); //now only weixin implemented.  目前只实现了微信

    // .... others code ...
}
~~~   

3. login action . 登录操作    
~~~ csharp
// AccountController.cs
// add subjectId parameter . 添加 subjectId 参数
[HttpPost]
[AllowAnonymous]
[ValidateAntiForgeryToken]
public IActionResult ExternalLogin(string provider, string subjectId, string returnUrl = null)
{
    // Request a redirect to the external login provider.
    var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { subjectId, returnUrl });
    var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

    // add subjectId
    properties.Items["subjectId"] = subjectId;

    return Challenge(properties, provider);
}
~~~   

4. web page . web 页面    
~~~ html
<!-- login.cshtml -->
<form asp-action="ExternalLogin" asp-route-returnurl="@ViewData["ReturnUrl"]" method="post" class="form-horizontal">
    <div>
        <p> 

        <!-- config subjectId when submit  -->
            <button type="submit" class="btn btn-default" name="provider" value="Weixin" asp-action="ExternalLogin" asp-route-returnurl="@ViewData["ReturnUrl"]" asp-route-subjectId="client1">weixin(client1)</button>

            <button type="submit" class="btn btn-default" name="provider" value="Weixin" asp-action="ExternalLogin" asp-route-returnurl="@ViewData["ReturnUrl"]" asp-route-subjectId="client2">weixin(client2)</button>

        </p>
    </div>
</form>

~~~





# Microsoft.AspNetCore.Authentication Extensions
QQ and Webchat extensions for Microsoft.AspNetCore.Authentication

# Get Started

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