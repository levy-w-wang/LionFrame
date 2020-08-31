using System;
using System.Collections.Generic;
using System.Text;

namespace LionFrame.Basic.AutofacDependency
{
    /// <summary>
    /// 单例接口 SingleInstance
    /// 整个应用程序生命周期以内只创建一个实例
    /// 所有的都是程序启动注册进去根容器，每次请求都是从 根容器中取子容器
    /// </summary>
    public interface ISingletonDependency
    {
    }
}
