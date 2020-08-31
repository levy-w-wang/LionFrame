using System;
using System.Collections.Generic;
using System.Text;

namespace LionFrame.Basic.AutofacDependency
{
    /// <summary>
    /// 同一个生命周期生成的对象是同一个 请求开始-请求结束 在这次请求中获取的对象都是同一个
    /// 在同一个Scope内只初始化一个实例 ，可以理解为（ 每一个request级别只创建一个实例，同一个http request会在一个 scope内）
    /// </summary>
    public interface IScopedDependency
    {
    }
}
