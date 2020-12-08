using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using HealthChecks.UI.Client;
using LionFrame.Basic;
using LionFrame.Basic.Extensions;
using LionFrame.CoreCommon;
using LionFrame.CoreCommon.CustomException;
using LionFrame.Data.BasicData;
using LionFrame.Model;
using LionFrame.Model.ResponseDto.ResultModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Quartz;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace LionFrame.MainWeb
{
    public partial class Startup
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public async void Init()
        {
            // 开启调度
            using var container = LionWeb.AutofacContainer.BeginLifetimeScope();
            await container.Resolve<IScheduler>().Start();
        }

        /// <summary>
        /// Db配置
        /// </summary>
        /// <param name="services"></param>
        private void ConfigureServices_Db(IServiceCollection services)
        {
            services.AddDbContext<LionDbContext>(options =>
            {
                var db = Configuration.GetSection("DB").Value;
                switch (db)
                {
                    case "MsSql":
                        options.UseSqlServer(connectionString: Configuration.GetConnectionString("MsSqlConnection"));
                        break;
                    case "MySql":
                        options.UseMySql(connectionString: Configuration.GetConnectionString("MySqlConnection"));
                        break;
                    default:
                        options.UseSqlServer(connectionString: Configuration.GetConnectionString("MsSqlConnection"));
                        break;
                }

                options.EnableSensitiveDataLogging();
                options.ReplaceService<IMigrationsModelDiffer, MigrationsModelDifferWithoutForeignKey>();
                options.UseLoggerFactory(loggerFactory: DbConsoleLoggerFactory);
            });
        }

        /// <summary>
        /// Swagger配置
        /// </summary>
        /// <param name="services"></param>
        private void ConfigureServices_Swagger(IServiceCollection services)
        {
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
                    Description = "JWT Authorization header using the Bearer scheme.", //参数描述
                    Name = "token", //名字
                    In = ParameterLocation.Header, //对应位置
                    Type = SecuritySchemeType.ApiKey, //类型描述
                    Scheme = "token"
                });
                //添加Jwt验证设置，不然在代码中取不到
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "token"
                            }
                        },
                        new List<string>()
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
        /// 健康检查
        /// </summary>
        /// <param name="services"></param>
        private void ConfigureServices_HealthChecks(IServiceCollection services)
        {
            var healthChecks = services.AddHealthChecks().AddRedis(Configuration["Redis:RedisConnectionString"]);
            var db = Configuration.GetSection("DB").Value;
            switch (db)
            {
                case "MsSql":
                    healthChecks.AddSqlServer(Configuration.GetConnectionString("MsSqlConnection"));
                    break;
                case "MySql":
                    healthChecks.AddMySql(Configuration.GetConnectionString("MySqlConnection"));
                    break;
                default:
                    healthChecks.AddSqlServer(Configuration.GetConnectionString("MsSqlConnection"));
                    break;
            }
            //配置监控存储地方
            var healthChecksUi = services.AddHealthChecksUI();
            var healthStorageType = Configuration["HealthChecks-UI:HealthStorageType"];
            switch (healthStorageType)
            {
                case "MsSql":
                    healthChecksUi.AddSqlServerStorage(Configuration["HealthChecks-UI:HealthStorageConnectionString"]);
                    break;
                case "MySql":
                    healthChecksUi.AddMySqlStorage(Configuration["HealthChecks-UI:HealthStorageConnectionString"]);
                    break;
                case "Memory":
                    healthChecksUi.AddInMemoryStorage();
                    break;
                default:
                    healthChecksUi.AddInMemoryStorage();
                    break;
            }
        }


        /// <summary>
        /// 配置Swagger
        /// </summary>
        /// <param name="app"></param>
        private void Config_Swagger(IApplicationBuilder app)
        {
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
        }

        /// <summary>
        /// 健康检查
        /// </summary>
        /// <param name="app"></param>
        private void Config_HealthChecks(IApplicationBuilder app)
        {
            app.UseHealthChecks("/health", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse

            }); //要进行该站点检测应添加此代码

            app.UseHealthChecksUI(config =>
            {
                config.UIPath = "/health_ui"; //切换ui地址
            }); //添加UI界面支持
        }

        /// <summary>
        /// 数据库操作语句显示
        /// </summary>
        private readonly ILoggerFactory DbConsoleLoggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddFilter((category, level) => category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Information).AddConsole();
        });


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
                LogHelper.Logger.Fatal(ex, $"【异常信息】：{ex.Message} 【请求路径】：{httpContext.Request.Method}:{httpContext.Request.Path}\n " + $"【UserHostAddress】:{LionWeb.GetClientIp()} " + $"【UserAgent】:{httpContext.Request.Headers["User-Agent"]}");

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
                    var content = ex.Message;
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