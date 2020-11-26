using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using LionFrame.Domain.BaseDomain;

namespace LionFrame.Domain.SystemDomain
{
    [Table("Sys_Quartz_Log")]
    public class SysQuartzLog : BaseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long LogId { get; set; }

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
        /// 下次执行时间
        /// </summary>
        public DateTime NextFireTime { get; set; }

        /// <summary>
        /// 上次执行时间
        /// </summary>
        public DateTime PreviousFireTime { get; set; }
        /// <summary>
        /// 请求参数
        /// </summary>
        public string Parameters { get; set; }

        /// <summary>
        /// Headers(可以包含如：Authorization授权认证) http请求用
        /// 格式：{"Authorization":"userpassword.."}
        /// </summary>
        [Required, MaxLength(256)]
        public string Headers { get; set; }

        /// <summary>
        /// 任务描述
        /// </summary>
        [Required, MaxLength(256)]
        public string Description { get; set; }
        /// <summary>
        /// 请求结果
        /// </summary>
        public string Result { get; set; }
    }
}
