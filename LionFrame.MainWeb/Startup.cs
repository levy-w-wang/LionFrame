using AspectCore.Extensions.Autofac;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using LionFrame.Controller;
using LionFrame.CoreCommon;
using LionFrame.CoreCommon.AutoMapperCfg;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace LionFrame.MainWeb
{
    public partial class Startup
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

            // 添加 automapper 映射关系
            services.AddAutoMapper(c => c.AddProfile<MappingProfile>());

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

            #region Swagger
            // 若要注释等信息记得修改输出路径  参考本项目csproj中的更改,取消显示警告加上1591
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Version = "v1",
                    Title = "API Doc",
                    Description = "作者:Levy_w_Wang",
                    //服务条款
                    TermsOfService = new Uri("http://book.levy.net.cn/"),
                    //作者信息
                    Contact = new OpenApiContact
                    {
                        Name = "levy",
                        Email = "levywang123@gmail.com",
                        Url = new Uri("http://book.levy.net.cn/")
                    },
                    //许可证
                    License = new OpenApiLicense
                    {
                        Name = "MIT",
                        Url = new Uri("https://github.com/levy-w-wang/LionFrame/blob/master/LICENSE")
                    }
                });

                #region XmlComments

                var basePath1 = Path.GetDirectoryName(typeof(Program).Assembly.Location); //获取应用程序所在目录（绝对，不受工作目录(平台)影响，建议采用此方法获取路径）
                //获取目录下的XML文件 显示注释等信息
                var xmlComments = Directory.GetFiles(basePath1, "*.xml", SearchOption.AllDirectories).ToList();

                foreach (var xmlComment in xmlComments)
                {
                    options.IncludeXmlComments(xmlComment);
                }

                #endregion

                options.DocInclusionPredicate((docName, description) => true);

                options.IgnoreObsoleteProperties(); //忽略 有Obsolete 属性的方法
                options.IgnoreObsoleteActions();
                options.DescribeAllEnumsAsStrings();
            });

            #endregion
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
            LionWeb.Environment = env;
            LionWeb.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());

            // 从 http 跳转到 https
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            //app.UseCookiePolicy();

            #region Swagger

            app.UseSwagger(c =>
            {
                c.RouteTemplate = "apidoc/{documentName}/swagger.json";
            });
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "apidoc";
                c.SwaggerEndpoint("v1/swagger.json", "ContentCenter API V1");
                c.DocExpansion(DocExpansion.None);
            });

            #endregion

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
