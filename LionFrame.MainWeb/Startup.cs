using AspectCore.Extensions.Autofac;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using LionFrame.Basic;
using LionFrame.Basic.Extensions;
using LionFrame.CoreCommon;
using LionFrame.CoreCommon.AutoMapperCfg;
using LionFrame.CoreCommon.Cache.Redis;
using LionFrame.CoreCommon.CustomException;
using LionFrame.CoreCommon.CustomFilter;
using LionFrame.Data.BasicData;
using LionFrame.Model;
using LionFrame.Model.ResponseDto.ResultModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;

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

            //services.AddDbContext<LionDbContext>(options =>
            //{
            //    options.UseSqlServer(connectionString: Configuration.GetConnectionString("SqlServerConnection"));
            //    options.EnableSensitiveDataLogging();
            //    options.UseLoggerFactory(loggerFactory: DbConsoleLoggerFactory);
            //});

            services.AddMemoryCache();//使用MemoryCache

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

                #region 添加头部swagger全局头部参数

                options.AddSecurityDefinition("token", new OpenApiSecurityScheme()
                {
                    Description = "JWT Authorization header using the Bearer scheme.",//参数描述
                    Name = "token",//名字
                    In = ParameterLocation.Header,//对应位置
                    Type = SecuritySchemeType.ApiKey,//类型描述
                    Scheme = "token"
                });
                //添加Jwt验证设置，不然在代码中取不到
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "token" }
                        }, new List<string>()
                    }
                });

                #endregion

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
            // 注册redis实例
            builder.RegisterInstance(new RedisClient(Configuration)).SingleInstance().PropertiesAutowired();
            // 注册dbcontext上下文
            builder.Register(context =>
            {
                //var config = context.Resolve<IConfiguration>();
                var opt = new DbContextOptionsBuilder<LionDbContext>();
                opt.UseSqlServer(Configuration.GetConnectionString("SqlServerConnection"));

                opt.EnableSensitiveDataLogging();
                opt.UseLoggerFactory(DbConsoleLoggerFactory);

                return new LionDbContext(opt.Options);
            }).InstancePerLifetimeScope().PropertiesAutowired();

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
            // 全局异常处理的三种方式  个人使用第三种  //搜索异常查找
            // 1.自定义的异常拦截管道 - - 放在第一位处理全局未捕捉的异常
            // app.UseExceptionHandler(build => build.Use(CustomExceptionHandler));
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            // 2.使用自定义异常处理中间件  处理该中间件以后未捕捉的异常
            //app.UseMiddleware<CustomExceptionMiddleware>();

            //autofac 新增 
            LionWeb.AutofacContainer = app.ApplicationServices.CreateScope().ServiceProvider.GetAutofacRoot();

            // autofac测试
            //if (LionWeb.AutofacContainer.IsRegistered<TestController>())
            //{
            //    var testBll = LionWeb.AutofacContainer.Resolve<TestController>();
            //    var guid = testBll.GetGuid();
            //}

            LionWeb.Environment = env;
            LionWeb.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());

            LionWeb.MemoryCache = memoryCache;

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

        #region 自定义的错误拦截管道来作为处理程序

        /// <summary>
        /// 自定义的错误拦截管道来作为处理程序
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        private async Task CustomExceptionHandler(HttpContext httpContext, Func<Task> next)
        {
            //该信息由ExceptionHandlerMiddleware中间件提供，里面包含了ExceptionHandlerMiddleware中间件捕获到的异常信息。
            var exceptionDetails = httpContext.Features.Get<IExceptionHandlerFeature>();
            var ex = exceptionDetails?.Error;

            if (ex != null)
            {
                LogHelper.Logger.Fatal(ex,
                    $"【异常信息】：{ex.Message} 【请求路径】：{httpContext.Request.Method}:{httpContext.Request.Path}\n " +
                    $"【UserHostAddress】:{ LionWeb.GetClientIp()} " +
                    $"【UserAgent】:{ httpContext.Request.Headers["User-Agent"]}");

                if (ex is CustomSystemException se)
                {
                    await ExceptionResult(httpContext, new ResponseModel().Fail(se.Code, se.Message, "").ToJson(true, isLowCase: true));
                }
                else if (ex is DataValidException de)
                {
                    await ExceptionResult(httpContext, new ResponseModel().Fail(de.Code, de.Message, "").ToJson(true, isLowCase: true));
                }
                else
                {
#if DEBUG
                    Console.WriteLine(ex);
                    var content = ex.ToJson();
#else
                var content = "系统错误，请稍后再试或联系管理人员。";
#endif
                    await ExceptionResult(httpContext, new ResponseModel().Fail(ResponseCode.UnknownEx, content, "").ToJson(true, isLowCase: true));
                }
            }
        }

        public async Task ExceptionResult(HttpContext httpContext, string data)
        {
            httpContext.Response.StatusCode = 200;
            if (string.IsNullOrEmpty(data))
                return;
            httpContext.Response.ContentType = "application/json;charset=utf-8";
            var bytes = Encoding.UTF8.GetBytes(data);

            await httpContext.Response.Body.WriteAsync(bytes, 0, bytes.Length);
        }

        #endregion

    }
}
