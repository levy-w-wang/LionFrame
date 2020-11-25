using AspectCore.Extensions.Autofac;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using LionFrame.CoreCommon;
using LionFrame.CoreCommon.AutoMapperCfg;
using LionFrame.CoreCommon.Cache;
using LionFrame.CoreCommon.Cache.Redis;
using LionFrame.CoreCommon.CustomFilter;
using LionFrame.Data.BasicData;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Quartz;
using Z.EntityFramework.Extensions;
using LionFrame.Quartz;

namespace LionFrame.MainWeb
{
    public partial class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder().SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            var config = builder.Build();
            LionWeb.Configuration = config;
            Configuration = config;
        }

        public IConfiguration Configuration { get; set; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            ConfigureServices_Db(services);

            services.AddMemoryCache();//使用MemoryCache

            // 添加 AutoMapper 映射关系
            services.AddAutoMapper(c => c.AddProfile<MappingProfile>());

            services.AddHttpClient<IHttpClientBuilder>();
            // If using Kestrel:
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;

            });
            // If using IIS:
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services.AddControllers(options =>
            {
                //验证错误最大个数，避免返回一长串错误
                options.MaxModelValidationErrors = 3;
                // 3.异常过滤器--处理mvc中未捕捉的异常
                options.Filters.Add<ExceptionFilter>();
            })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .ConfigureApiBehaviorOptions(options =>
                {
                    //抑制系统自带模型验证,不然只会走 ApiController 自带的模型验证
                    options.SuppressModelStateInvalidFilter = true;
                })
                .AddNewtonsoftJson()
                .AddControllersAsServices();
            //资源路径小写
            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
            });

#if !DEBUG
              ConfigureServices_HealthChecks(services); //测试情况下不开启健康检查
#endif
            ConfigureServices_Swagger(services);
        }

        /// <summary>
        /// 新版autofac注入
        /// </summary>
        /// <param name="builder"></param>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule<AutofacModule>();
            // 注册redis实例
            builder.RegisterInstance(new RedisClient(Configuration)).SingleInstance().PropertiesAutowired();
            builder.RegisterInstance(new LionMemoryCache("Cache")).SingleInstance().PropertiesAutowired();
            var scheduler = new SchedulerFactory(Configuration).GetScheduler().Result;
            builder.RegisterInstance(scheduler).As<IScheduler>().PropertiesAutowired().SingleInstance();

            #region 注册dbcontext上下文 使用属性注入 -- 但是使用上面的方式直接add好像也可以

            //builder.Register(context =>
            //{
            //    //var config = context.Resolve<IConfiguration>();
            //    var opt = new DbContextOptionsBuilder<LionDbContext>();
            //    opt.UseSqlServer(Configuration.GetConnectionString("SqlServerConnection"));
            //    opt.ReplaceService<IMigrationsModelDiffer, MigrationsModelDifferWithoutForeignKey>();
            //    opt.EnableSensitiveDataLogging();
            //    opt.UseLoggerFactory(DbConsoleLoggerFactory);

            //    return new LionDbContext(opt.Options);
            //}).InstancePerLifetimeScope().PropertiesAutowired();

            #endregion

            // 配置AOP代理
            // 属性注入得把方法加上 virtual  不然没效果
            builder.RegisterDynamicProxy(config =>
            {
                ////全局使用AOP  这里由于不是使用的接口的方式，需要在要使用AOP的方法上加 virtual 关键字
                //config.Interceptors.AddTyped<LogInterceptorAttribute>();
                //config.Interceptors.AddServiced<LogInterceptorAttribute>();
                //// 带有Service后缀当前方法会被拦截
                //config.Interceptors.AddTyped<LogInterceptorAttribute>(method => method.Name.EndsWith("Service"));
                //// 使用 通配符 的特定全局拦截器
                //config.Interceptors.AddTyped<LogInterceptorAttribute>(Predicates.ForService("*Service"));

                ////Demo.Data命名空间下的Service不会被代理
                //config.NonAspectPredicates.AddNamespace("Demo.Data");

                ////最后一级为Data的命名空间下的Service不会被代理
                //config.NonAspectPredicates.AddNamespace("*.Data");

                ////ICustomService接口不会被代理
                //config.NonAspectPredicates.AddService("ICustomService");

                ////后缀为Service的接口和类不会被代理
                //config.NonAspectPredicates.AddService("*Service");

                ////命名为FindUser的方法不会被代理
                //config.NonAspectPredicates.AddMethod("FindUser");

                ////后缀为User的方法不会被代理
                //config.NonAspectPredicates.AddMethod("*User");
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMemoryCache memoryCache)
        {
            // 全局异常处理的三种方式
            // 1.自定义的异常拦截管道 - - 放在第一位处理全局未捕捉的异常
            app.UseExceptionHandler(build => build.Use(CustomExceptionHandler));
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
                app.UseHttpsRedirection();// 从 http 跳转到 https
                Config_HealthChecks(app); //测试情况下不开启健康检查
            }

            // 2.使用自定义异常处理中间件  处理该中间件以后未捕捉的异常
            //app.UseMiddleware<CustomExceptionMiddleware>();

            //autofac 新增 
            LionWeb.AutofacContainer = app.ApplicationServices.CreateScope().ServiceProvider.GetAutofacRoot();

            // Z.EntityFramework.Extensions 扩展包需要  --无法显示日志
            EntityFrameworkManager.ContextFactory = context => LionWeb.AutofacContainer.Resolve<LionDbContext>();

            LionWeb.Environment = env;
            LionWeb.Configure(LionWeb.AutofacContainer.Resolve<IHttpContextAccessor>());

            LionWeb.MemoryCache = memoryCache;

            app.UseStaticFiles();
            //app.UseCookiePolicy();

            Config_Swagger(app);

            app.UseRouting();
            //app.UseRequestLocalization();

            // 跨域
            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().WithMethods(new string[]
            {
                HttpMethods.Get,
                HttpMethods.Post,
                HttpMethods.Delete,
                HttpMethods.Put
            }));

            // app.UseSession();
            // app.UseResponseCaching();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapGet("/", async context =>
                {
                    context.Response.ContentType = "text/html; charset=utf-8";
                    await context.Response.WriteAsync("<div style='text-align: center;margin-top: 15%;'><h3>项目<b style='color: green;'>启动成功</b>,测试请使用接口测试工具，或与前端联调！</h3> <h4>项目<a href='/apidoc' style='color: cornflowerblue;'>接口文档</a>,点击查看</h4></div>");
                });
            });
        }
    }
}
