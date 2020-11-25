using Autofac;
using AutoMapper;
using LionFrame.Basic;
using LionFrame.Basic.AutofacDependency;
using LionFrame.CoreCommon.AutoMapperCfg;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Quartz;
using Module = Autofac.Module;

namespace LionFrame.CoreCommon
{
    public class AutofacModule : Module
    {
        /// <summary>
        /// 容器初始化
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        protected override void Load(ContainerBuilder builder)
        {
            //所有程序集 和程序集下类型
            var deps = DependencyContext.Default;
            var libs = deps.CompileLibraries.Where(lib => !lib.Serviceable && lib.Type == "project"); //排除所有的系统程序集、Nuget下载包
            var listAllType = new List<Type>();
            foreach (var lib in libs)
            {
                try
                {
                    var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(lib.Name));
                    listAllType.AddRange(assembly.GetTypes());
                }
                catch
                {
                }
            }

            #region 注册ITransientDependency实现类   瞬时的  默认的

            var dependencyType = typeof(ITransientDependency);
            var arrDependencyType = listAllType.Where(t => dependencyType.IsAssignableFrom(t) && t != dependencyType
               ).ToArray();

            builder.RegisterTypes(arrDependencyType)
                .InstancePerDependency() //每次使用都创建一个新实例   瞬时的  默认的
                .PropertiesAutowired() // 开启属性注入
                ;

            #endregion

            #region 每次请求是唯一实例 IScopedDependency

            var scopeDependencyType = typeof(IScopedDependency);
            var arrScopeDependencyType = listAllType.Where(t => scopeDependencyType.IsAssignableFrom(t) && t != scopeDependencyType
                ).ToArray();

            builder.RegisterTypes(arrScopeDependencyType)
                .InstancePerLifetimeScope() //每次请求是唯一实例
                .PropertiesAutowired() // 开启属性注入
                ;

            #endregion

            #region 单一实例

            var singletonDependencyType = typeof(ISingletonDependency);
            var arrSingletonDependencyType = listAllType.Where(t => singletonDependencyType.IsAssignableFrom(t)
                && t != singletonDependencyType
                ).ToArray();

            builder.RegisterTypes(arrSingletonDependencyType)
                .SingleInstance()
                .PropertiesAutowired();

            #endregion

            #region 注册 mapper及配置 为属性注入

            var config = new MapperConfiguration(c => c.AddProfile<MappingProfile>());
            builder.RegisterInstance(config).SingleInstance().PropertiesAutowired();
            builder.RegisterType<Mapper>().SingleInstance().PropertiesAutowired();

            #endregion

            #region 注册controller实现类

            var controller = typeof(ControllerBase);
            var arrControllerType = listAllType.Where(t => controller.IsAssignableFrom(t) && !t.IsAbstract && t != controller).ToArray();
            builder.RegisterTypes(arrControllerType).PropertiesAutowired();

            #endregion

            var idWorker = LionWeb.Configuration.GetSection("IdWorker").Get<long>();
            builder.RegisterInstance(new IdWorker(idWorker)).SingleInstance().PropertiesAutowired();
        }
    }
}
