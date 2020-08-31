using AspectCore.Extensions.Autofac;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using LionFrame.Controller;
using LionFrame.CoreCommon;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace LionFrame.MainWeb
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder().SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            LionWeb.Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // If using Kestrel:
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;

            });
            // If using IIS:
            //services.Configure<IISServerOptions>(options =>
            //{
            //    options.AllowSynchronousIO = true;
            //});
            services.AddControllers(options =>
            {
                options.MaxModelValidationErrors = 3;
                //options.Filters.Add(xxx);
            })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddNewtonsoftJson()
                .AddControllersAsServices();
            ;
            services.AddRouting(options =>
            {
                options.LowercaseUrls = true; //资源路径小写
            });
        }

        /// <summary>
        /// 新版autofac注入
        /// </summary>
        /// <param name="builder"></param>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule<AutofacModule>();

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

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            //autofac 新增 
            LionWeb.AutofacContainer = app.ApplicationServices.CreateScope().ServiceProvider.GetAutofacRoot();

            // autofac测试
            if (LionWeb.AutofacContainer.IsRegistered<TestController>())
            {
                var testBll = LionWeb.AutofacContainer.Resolve<TestController>();
                var guid = testBll.GetGuid();
            }

            LionWeb.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());

            // 从 http 跳转到 https
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            //app.UseCookiePolicy();

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
                    await context.Response.WriteAsync("<div style='text-align: center;margin-top: 15%;'><h3>项目<b style='color: green;'>启动成功</b>,测试请使用接口测试工具，或与前端联调！</h3></div>");
                });
            });
        }
    }
}
