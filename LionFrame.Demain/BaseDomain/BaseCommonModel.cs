using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LionFrame.Domain.BaseDomain
{
    public abstract class BaseCommonModel : BaseModel
    {
        protected BaseCommonModel()
        {
            Deleted = false;
            //CreatedTime = DateTime.Now; //避免更新的时候将该字段更新
            UpdatedTime = DateTime.Now;
        }

        public bool Deleted { get; set; }

        public DateTime CreatedTime { get; set; }

        public long CreatedBy { get; set; }

        public DateTime UpdatedTime { get; set; }

        public long UpdatedBy { get; set; }
    }
}
