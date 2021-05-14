- [框架说明](#框架说明)
- [项目框架图](#项目框架图)
- [多租户权限设计表](#多租户权限设计表)
- [效果图](#效果图)
- [后端拉取运行](#后端拉取运行)
- [前端项目请参考 前端系列](#前端项目请参考-前端系列)
- [发布到docker中](#发布到docker中)
- [netcore3.1 发布到docker中所遇到的坑及解决](#netcore3.1-发布到docker中所遇到的坑及解决)

-----------------------

## 框架说明
该框架是本人学习过程中本着只有自己动手操作一遍才能真正理解，和遇到对应问题并解决问题的思路。和为了能在开发相应系统时能快速搭建出相关框架而做出的基于NetCore3.1+Vue的RBAC通用权限框架。

只有在敲的过程中才能遇见细节上的问题，成长无非就是发现问题、思考问题、解决问题、总结沉淀，后面才能去规避和提高代码质量。

如有发现什么错误，请联系我，将第一时间改正。互相学习，共同进步。

不要只光看，最终要的是要自己敲，不自己敲的话，当遇到了还是不会、一脸懵的！

> 前端测试地址 [lion.levy.net.cn](http://lion.levy.net.cn/login) , 可以自己注册，也可使用已有账号密码登录。**请不要用已有账号修改或删除系统已有数据，谢谢配合**
> 前端代码地址 [Gitee](https://gitee.com/levy_w_wang/lion-ui)       [GitHub](https://github.com/levy-w-wang/lion-ui)
> 后端接口地址 [lion.levy.net.cn](http://lion.api.levy.net.cn)
> 后端代码地址 [Gitee](https://gitee.com/levy_w_wang/LionFrame)     [GitHub](https://github.com/levy-w-wang/LionFrame)

** 如果您觉得对您有帮助的话，可以给个start，谢谢 **

## 项目框架图

![](https://img2020.cnblogs.com/blog/1750888/202105/1750888-20210514223845903-1683821238.png)

> EntityFrameworkCore CodeFirist开发  支持 MySql/SqlServer
> 数据迁移时自动生成种子数据，Quartz 表结构还请自行通过docs文件中对应数据库类型文件，执行添加表语句
> 异步 async/await 开发
> 采用类似仓储模式开发，封装底层常用数据库方法
> Nlog日志记录，可记录请求日志/异常日志等，调试阶段打印SQL语句
> 支持AOP切面编程，如日志、缓存、事务处理等
> 权限管理系统支持多租户多权限管理，页面动态管理页面&按钮权限
> 采用自定义JWT身份认证，可自带参数，验证过期时间，滑动无感重新颁发Token
> 支持Redis 和 Memory 两种缓存
> 使用三方DI AutoFac，实行批量注册，支持属性注入
> 采用Automapper做对象映射，并支持扩展方法直接映射。
> 使用 Quartz.net做任务调度，支持集群调度（未测试该情况,代码中有注释说明），支持反射调用本地程序集方式，支持调用API方式
> 使用Swagger提供API文档，并实现接口文档中填写授权信息方便调试。
> 支持CORS跨域，当前使用的是全跨域模式。
> 支持监控心跳检查HealthChecks，可实现对其它网页或服务的检查和通知。(代码没写，如需要，可联系我获取。通知支持钉钉和企业微信等)
> 支持RabbitMq消息队列，实现了死信队列和延迟队列。
> 支持docker部署，支持docker中部署同一网段
> 可配合Jenkins加 Sonar 做CI/CD & 代码质量检查
> Nginx可配置实现负载均衡
> 可配置 IpRateLimiting 做API限流处理（此前配置试过，没有起到效果，可能哪个地方冲突了），若需要，也可根据redis实现一个简易版的

## 多租户权限设计表
详细说明
Deleted、CreatedTime、CreatedBy、UpdatedTime、UpdatedBy 为常用表默认字段，可自行扩展增加Remark、Status、Sort等为表通用字段。特别说明，若数据量很大的表，可将非关键字段拆分出来，减小表大小，提高查询速度。
> * 数据都是按页存在，页存储空间有限，将非关键信息存另外的一个表，关键信息表所存数据页就少，查询速度就相应的提高了。 *个人理解*

**Sys_Tenant租户表**

> * 租户相当于是将系统租给某个公司使用，租户表管理公司企业信息，这里表结构只是给了一个示例。
> * **若有什么想要控制，就设计成相应的表，若数据资源固定则设计为字典表。**

字段名 | 说明 | 类型 | 主键
--|--|--|--
TenantId | 主键 | bigint | TRUE
TenantName | 租户名称  | nvarchar(50) | 
Remark | 备注 | nvarchar(32) | 
State | 状态:1-启用;-1-禁用 | int | 
CreatedTime | 创建时间 | datetime | 

**sys_user用户表**

字段名 | 说明 | 类型 | 主键
--|--|--|--
UserId | 主键 | bigint | TRUE
TenantId | 租户ID  | bigint | 
NickName | 用户名 | nvarchar(30) | 
PassWord | 密码 | nvarchar(512) | 
Email | Email | nvarchar(128) | 
Sex | 性别:0-女;1-男 | int | 
State | 状态:1-启用;-1-禁用 | int | 
CreatedTime | 创建时间 | datetime | 
CreatedBy | 创建人 | bigint	 | 
UpdatedTime | 修改时间 | datetime | 
UpdatedBy | 修改人 | bigint	 | 

**Sys_Role角色表**

字段名 | 说明 | 类型 | 主键
--|--|--|--
RoleId | 主键 | bigint | TRUE
TenantId | 租户ID  | bigint | 
RoleName | 角色名称 | nvarchar(25) | 
RoleDesc | 角色描述 | nvarchar(128) | 
Deleted | 逻辑删除标志 | tinyint | 
CreatedTime | 创建时间 | datetime | 
CreatedBy | 创建人 | bigint	 | 
UpdatedTime | 修改时间 | datetime | 
UpdatedBy | 修改人 | bigint	 | 

**Sys_User_Role_Relation用户角色关系表**

字段名 | 说明 | 类型 | 主键
--|--|--|--
UserId | 用户ID | bigint | TRUE
RoleId | 角色Id | bigint | TRUE
TenantId | 租户ID  | bigint | TRUE
State | 状态:1-启用;-1-禁用 | int | 
Deleted | 逻辑删除标志 | tinyint | 
CreatedTime | 创建时间 | datetime | 
CreatedBy | 创建人 | bigint	 | 
UpdatedTime | 修改时间 | datetime | 
UpdatedBy | 修改人 | bigint	 | 

**Sys_Menu权限资源表**

字段名 | 说明 | 类型 | 主键
--|--|--|--
MenuId | 主键，自定义 | nvarchar(40) | TRUE
MenuName | 资源名称 | nvarchar(64) | 
ParentMenuId | 父级Id--无限级菜单（长度加大）  | nvarchar(40) | 
Level | 菜单层级 | int | 
Url | 资源地址|nvarchar(256) | 
Type|资源类型：1:菜单 2:按钮|int | 
Icon| 图标|nvarchar(128) | 
OrderIndex|顺序|int | 
Deleted | 逻辑删除标志 | tinyint | 
CreatedTime | 创建时间 | datetime | 
CreatedBy | 创建人 | bigint	 | 
UpdatedTime | 修改时间 | datetime | 
UpdatedBy | 修改人 | bigint	 | 

**Sys_Role_Menu_Relation角色资源关系表**

字段名 | 说明 | 类型 | 主键
--|--|--|--
MenuId | 资源ID | nvarchar(40) | TRUE
RoleId | 角色Id | bigint | TRUE
TenantId | 租户ID  | bigint | TRUE
State | 状态:1-启用;-1-禁用 | int | 
Deleted | 逻辑删除标志 | tinyint | 
CreatedTime | 创建时间 | datetime | 
CreatedBy | 创建人 | bigint	 | 
UpdatedTime | 修改时间 | datetime | 
UpdatedBy | 修改人 | bigint	 | 

* 调度相关-额外创建的管理表

**Sys_Quartz调度任务综合表**

字段名 | 说明 | 类型 | 主键
--|--|--|--
JobGroup | 任务分组 | nvarchar(200) | TRUE
JobName | 任务名称 | nvarchar(200) | TRUE
JobType | 任务类型：0-无，1-api,2-程序集 | int |
BeginTime | 开始时间 | datetime | 
EndTime | 结束时间 | datetime | 
Cron|Cron表达式| nvarchar(40) | 
RunTimes| 执行次数 | int |
IntervalSecond| 循环次数 | int |
TriggerType| 任务类型：0-无，1-cron,2-简单类型 | int |
RequestPath|API地址或请求类程序集名称| nvarchar(56) | 
RequestMethod|API请求类型或请求类地址| nvarchar(40) | 
RequestParameters|请求Body参数| nvarchar(512) | 
Headers|请求头参数| nvarchar(256) | 
Priority|执行优先级，等级越高，相同时间先执行 | int | 
Description|任务描述| nvarchar(256) | 
NotifyEmail|通知邮箱| nvarchar(128) | 
MailMessage | 邮件通知类型：0-不通知,1-错误通知,2-全量通知 | int | 
TriggerState | 暂停 错误 阻塞 完成 等 | int | 
PreviousFireTime | 上次执行时间 | datetime | 
NextFireTime | 下次执行时间 | datetime | 
CreatedTime | 创建时间 | datetime | 

## 效果图

当用户注册使用，便是一个租户，租户也就代表是一个公司群体，可以以该账号创建子账号，给子账号分配页面及按钮权限。

> 接口文档

![](https://img2020.cnblogs.com/blog/1750888/202105/1750888-20210514223923353-1401695913.png)

> 用户界面

![](https://img2020.cnblogs.com/blog/1750888/202105/1750888-20210514223939024-1657002209.png)

> 角色管理界面

![](https://img2020.cnblogs.com/blog/1750888/202105/1750888-20210514223951997-1175939985.png)

> 系统管理菜单界面

![](https://img2020.cnblogs.com/blog/1750888/202105/1750888-20210514224005796-206030397.png)

>  调度任务界面

![](https://img2020.cnblogs.com/blog/1750888/202105/1750888-20210514224015594-615595197.png)

> 调度-接口及cron界面

![](https://img2020.cnblogs.com/blog/1750888/202105/1750888-20210514224027739-1900883043.png)

> 调度-程序集反射方式创建界面

![](https://img2020.cnblogs.com/blog/1750888/202105/1750888-20210514224036373-544727962.png)

> 调度日志界面

![](https://img2020.cnblogs.com/blog/1750888/202105/1750888-20210514224045652-1886625230.png)

## 后端拉取运行


*  **需配置`appsettings.json`中的相关信息** 配置邮箱发送人信息，本项目配置的是QQ邮箱，请根据需要配置，用以注册和找回密码的短信发送

*  **需配置`appsettings.json`中的相关信息** 配置数据库链接信息，执行数据库迁移命令（查看第7点），执行完成后，根据doc文件中选择`Quartz`对应数据库脚本语言创建相关表。

*  配置NLog.config文件数据库相关信息，用于记录日志，根据使用数据库选择相应链接，并配置数据库链接

*   **需配置`appsettings.json`中的相关信息** `Redis`链接信息

*   **需配置`appsettings.json`中的相关信息** `RabbitMQ`链接信息

*   **需配置`appsettings.json`中的相关信息** `Redis`链接信息

*  在`LionFrame.Data`项目中有个种子数据文件夹，在数据迁移时会添加相关信息

## 前端项目请参考 前端系列

在博客中之前已经写过前端系列，可参考下，有不清楚的地方可在个人简介中找到我的联系方式。若有什么错误的地方也欢迎指正~

## 发布到docker中

可参考项目中的docs文件夹下的 *同一网络部署到Docker中* 文件。因为我这边redis和mysql已经在之前就已经创建过了，所以此次只做了 MQ和项目发布到同一网段中，其它的可如法炮制。喜欢玩的也可以使用`docker-compose`来进行编写第一次的发布脚本，因为多环境的问题，建议使用`docker-compose`，可快速的进行部署，也可避免命令敲错等情况。

只有初次发布时配置可能比较麻烦，后面基本就只有项目需要多次发布，可引入jenkins 做 CI/CD，基本支持中小企业使用

若项目做的比较大可升级K8S，做弹性伸缩，nginx做负载均衡等等、

前端项目这里由于是腾讯云的学生服务器，比较卡顿，故申请了一个腾讯云的免费6个月的存储桶COS进行发布前端项目。

## netcore3.1 发布到docker中所遇到的坑及解决

由于docker中没有图片的依赖组件，在我们生成二维码的时候的时候需要使用到`System.Drawing.Common` 来使用 Image、Bitmap 等类型,通过docker logs 可查看到如下异常

```
System.TypeInitializationException: The type initializer for 'Gdip' threw an exception. ---> System.DllNotFoundException: Unable to load DLL 'libgdiplus': The specified module could not be found.
```

解决方法，在`dockerfile`中加上如下语句`RUN apt-get update && apt-get install -y libgdiplus`

由于国内网络原因，此处可能需要下载数十分钟，为了提高数据，可在`dockerfile`中加上`RUN cp sources.list /etc/apt/`一句话，使用镜像源，来提高下载速度，`sources.list`文件在docs文件夹中有提供。

这样就能解决图片的问题

> ps:小插曲，其实最开始我在这样处理后还是不能生成图片、日志中也看不到错误，只知道容器在调用二维码生成接口时就会退出，起先还以为是dockerfile引入有问题，
> 在各大网站查找相关讯息，博客园/csdn/stackoverflow/github等网站上查找。
> 在这个网址上 https://github.com/dotnet/dotnet-docker/issues/618 看到人家都是这样解决的，为啥我就不行
> 不禁陷入了沉思，突然想到可能是代码的问题，换了生成验证码的方式， 惊奇的发现可以了。有的时候得换一种思考的方式。

[原文地址](http://book.levy.net.cn/doc/backend/core3_1-vue.html)
