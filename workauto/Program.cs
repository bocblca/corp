using Hangfire;
using Hangfire.Dashboard;
using Hangfire.LiteDB;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Mysqldb;
using rcbblc.netauto;
using Senparc.CO2NET;
using Senparc.CO2NET.AspNet;
using Senparc.CO2NET.RegisterServices;
using Senparc.Weixin;
using Senparc.Weixin.Entities;
using Senparc.Weixin.RegisterServices;
using Senparc.Weixin.Work;
using Senparc.Weixin.Work.MessageHandlers.Middleware;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Email;
using Serilog.Sinks.SystemConsole.Themes;
using System.Net;
using System.Reflection;
using System.Text;
using workapi.JWT;
using workapi.Models;
using workauto.corp;
using workauto.filter;
using Zack.EventBus;

var builder = WebApplication.CreateBuilder(args);
//动态注入DBcontext等类
var asms = Getassembly.Getasms("rcbautoservice");

if (asms != null)
{
    //RunModuleInitializers方法需要提供接口
    //builder.Services.RunModuleInitializers(asms);
    //runmethod属性不需要提供接口，需要提供类名
    builder.Services.Runmethod(asms, "AutoInitcs");
}
builder.Services.Configure<IntegrationEventRabbitMQOptions>(o =>
{
    o.HostName = "192.168.100.223"; //rcbmq-rabbitmq.net6
    o.ExchangeName = "demo1";
    o.UserName = "blc";
    o.Password = "blc741004";

});

builder.Services.AddEventBus("queue1", Assembly.GetExecutingAssembly());
builder.Services.AddEndpointsApiExplorer();




//日志提供程序,warning以上级别log会发送邮件并记录在MYSQL数据库
builder.Services.AddLogging(logBuilder =>
{
    Log.Logger = new LoggerConfiguration()
      .MinimumLevel.Warning()
      .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
      .Enrich.FromLogContext()
      .WriteTo.Console(theme: SystemConsoleTheme.Colored)
      .WriteTo.PostgreSQL(builder.Configuration.GetConnectionString("postpresql"), "corplogs")
      //.WriteTo.MySQL(builder.Configuration.GetConnectionString("mysql"))
      .WriteTo.Email(new EmailConnectionInfo()
      {
          EmailSubject = "K8S workapi系统警告级别!",//邮件标题
          FromEmail = "blcrcb@qq.com",//发件人邮箱
          MailServer = "smtp.qq.com",//smtp服务器地址
          NetworkCredentials = new NetworkCredential("blcrcb@qq.com", "xkgvdblzrgnybjgh"),//两个参数分别是发件人邮箱与客户端授权码
          Port = 587,//端口号
          ToEmail = "18518116581@163.com"//收件人
      })
      .CreateLogger();

    logBuilder.AddSerilog();
});

builder.Services.AddSwaggerGen(c =>
{
    var scheme = new OpenApiSecurityScheme()
    {
        Description = "Authorization header. \r\nExample: 'Bearer 12345abcdef'",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Authorization"
        },
        Scheme = "oauth2",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
    };
    c.AddSecurityDefinition("Authorization", scheme);
    var requirement = new OpenApiSecurityRequirement();
    requirement[scheme] = new List<string>();
    c.AddSecurityRequirement(requirement);
     
});
builder.Services.AddMemoryCache();
//builder.Host.UseServiceProviderFactory(new SenparcServiceProviderFactory());
builder.Services.AddControllersWithViews()
                    .AddNewtonsoftJson(options =>
                    {
                        // 设置时间格式
                        options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                        //忽略null值

                    });// 支持 NewtonsoftJson

builder.Services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();
//如果部署在linux系统上，需要加上下面的配置：
builder.Services.Configure<KestrelServerOptions>(options => { options.AllowSynchronousIO = true; });
//builder.Services.Configure<IISServerOptions>(options => options.AllowSynchronousIO = true);
//builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.Configure<FormOptions>(options =>
{
    options.KeyLengthLimit = int.MaxValue;
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
});
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = int.MaxValue;
    options.Limits.MaxRequestBufferSize = int.MaxValue;
});

//IServiceCollection services = builder.Services;
//services.AddScoped<WorkApiController>();
builder.Services.AddHangfire(config =>
{
    config.UseLiteDbStorage("./hf20230521.db");

});
builder.Services.AddHangfireServer();
builder.Services.AddDataProtection();
builder.Services.AddIdentityCore<User>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
    options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
});
var idBuilder = new IdentityBuilder(typeof(User), typeof(myRole), builder.Services);
idBuilder.AddEntityFrameworkStores<Wxusers>()
    .AddDefaultTokenProviders()
    .AddRoleManager<RoleManager<myRole>>()
    .AddUserManager<UserManager<User>>();
builder.Services.Configure<JWTOptions>(builder.Configuration.GetSection("JWT"));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(x =>
{
    var jwtOpt = builder.Configuration.GetSection("JWT").Get<JWTOptions>();
    byte[] keyBytes = Encoding.UTF8.GetBytes(jwtOpt.SigningKey);
    var secKey = new SymmetricSecurityKey(keyBytes);
    x.TokenValidationParameters = new()
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = false,//true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = secKey
    };
});

builder.Services.AddSenparcWeixinServices(builder.Configuration);
builder.Services.AddSenparcGlobalServices(builder.Configuration);//Senparc.CO2NET 全局注册

builder.Services.Configure<MvcOptions>(options =>
{
    options.Filters.Add<TransactionScopeFilter>();
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("mcros", policy =>
    {
        policy.AllowAnyMethod()
              .SetIsOriginAllowed(_ => true)
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

//builder.Services.AddScoped<startHJob>();



var app = builder.Build();



//app.UseEventBus();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseFileServer();
var senparcWeixinSetting = app.Services.GetService<IOptions<SenparcWeixinSetting>>().Value; //builder.Configuration.Get<SenparcWeixinSetting>(); //app.Services.GetService<IOptions<SenparcWeixinSetting>>().Value;
ForwardedHeadersOptions options = new()
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};
options.KnownNetworks.Clear();
options.KnownProxies.Clear();
app.UseCors("mcros");
app.UseForwardedHeaders(options);

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseHangfireDashboard("/rcbfire", new DashboardOptions()
{
    Authorization = new[] { new CustomAuthorizeFilter() },
    IgnoreAntiforgeryToken = true,
    PrefixPath = "/corp", //这个前缀对于k8s部署的程序，尤其是一个域名下有多个app非常有用
    IsReadOnlyFunc = (DashboardContext context) => true  //这个设置可以禁止在Dashboard对hangfire任务进行操作
}) ;


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseSenparcGlobal(builder.Environment, Senparc.CO2NET.Config.SenparcSetting, reg =>
{
})
              .UseSenparcWeixin(senparcWeixinSetting, (weixinRegister, weixinSetting) =>
              {
                  weixinRegister
                  .RegisterWorkAccount(senparcWeixinSetting, "企业微信通讯录")
                  .RegisterWorkAccount(senparcWeixinSetting.Items["workprint"], "云打印小程序")
                  .RegisterWorkAccount(senparcWeixinSetting.Items["workscan"], "企业物品登记")
                  .RegisterWorkAccount(senparcWeixinSetting.Items["supernotice"], "督办通知单");
              });
app.UseMessageHandlerForWork("/Work", WorkCustomMessageHandler.GenerateMessageHandler, options =>
{
    options.AccountSettingFunc = context => Senparc.Weixin.Config.SenparcWeixinSetting;
});
app.UseMessageHandlerForWork("/Workprint", WorkprintMessageHandler.GenerateMessageHandler, options =>
{
    options.AccountSettingFunc = context => Senparc.Weixin.Config.SenparcWeixinSetting.Items["workprint"];
});
app.UseMessageHandlerForWork("/Workscan", WorkscanMessageHandler.GenerateMessageHandler, options =>
{
    options.AccountSettingFunc = context => Senparc.Weixin.Config.SenparcWeixinSetting.Items["workscan"];
});
app.UseMessageHandlerForWork("/supernotice", SuperNoticeMessageHandler.GenerateMessageHandler, options =>
{
    options.AccountSettingFunc = context => Senparc.Weixin.Config.SenparcWeixinSetting.Items["supernotice"];

});


app.MapControllers();
app.StartJob();

await app.RunAsync();
