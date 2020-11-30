using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using LionFrame.Domain.BaseDomain;
using LionFrame.Model.QuartzModels;
using Quartz;

namespace LionFrame.Domain.SystemDomain
{
    [Table("Sys_Quartz")]
    public class SysQuartz : BaseModel
    {
        /// <summary>
        /// 任务分组
        /// </summary>
        [Required, MaxLength(200)]
        public string JobGroup { get; set; }
        /// <summary>
        /// 任务名称
        /// </summary>
        [Required, MaxLength(200)]
        public string JobName { get; set; }

        [Required]
        public JobTypeEnum JobType { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [Required]
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [Required]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Cron表达式
        /// </summary>
        [Required, MaxLength(40)]
        public string Cron { get; set; }

        /// <summary>
        /// 执行次数（默认无限循环）
        /// </summary>
        public int RunTimes { get; set; }

        /// <summary>
        /// 执行间隔时间，单位秒（如果有Cron，则IntervalSecond失效）
        /// </summary>
        public int IntervalSecond { get; set; }

        /// <summary>
        /// 触发器类型
        /// </summary>
        [Required]
        public TriggerTypeEnum TriggerType { get; set; }

        /// <summary>
        /// 请求url 或 仅项目完整名称(不包含文件夹路径)
        /// </summary>
        [Required, MaxLength(56)]
        public string RequestPath { get; set; }

        /// <summary>
        /// 请求类型 RequestTypeEnum   或  文件夹名.类名
        /// </summary>
        [Required, MaxLength(40)]
        public string RequestMethod { get; set; }

        /// <summary>
        /// 请求参数（Post，Put请求用） 调用方法参数
        /// </summary>
        [Required, MaxLength(512)]
        public string RequestParameters { get; set; }

        /// <summary>
        /// Headers(可以包含如：Authorization授权认证) http请求用
        /// 格式：{"Authorization":"userpassword.."}
        /// </summary>
        [Required, MaxLength(256)]
        public string Headers { get; set; }

        /// <summary>
        /// 执行优先级  默认等级5 等级越高 执行时间相同时就先执行谁
        /// </summary>
        [Required]
        public int Priority { get; set; }

        /// <summary>
        /// 任务描述
        /// </summary>
        [Required, MaxLength(256)]
        public string Description { get; set; }

        /// <summary>
        /// 通知邮箱
        /// </summary>
        [Required, MaxLength(128)]
        public string NotifyEmail { get; set; }

        /// <summary>
        /// 邮件通知类型
        /// </summary>
        [Required]
        public MailMessageEnum MailMessage { get; set; }

        /// <summary>
        /// 任务状态
        /// </summary>
        public MyTriggerState TriggerState { get; set; }

        /// <summary>
        /// 上次执行时间
        /// </summary>
        public DateTime? PreviousFireTime { get; set; }

        /// <summary>
        /// 下次执行时间
        /// </summary>
        public DateTime? NextFireTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedTime { get; set; }
    }
}
