using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using LionFrame.Domain.BaseDomain;

namespace LionFrame.Domain.SystemDomain
{
    [Table("Tenant_Info")]
    public class TenantInfo : BaseModel
    {
        //[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public long TenantId { get; set; }
        /// <summary>
        /// 租户名称
        /// </summary>
        [Required,MaxLength(50)]
        public string TenantName { get; set; }

        [MaxLength(32)]
        public string Remark { get; set; }

        public int State { get; set; }

        public DateTime CreatedTime { get; set; }
        
        public List<SysUser> SysUser { get; set; }
    }
}
