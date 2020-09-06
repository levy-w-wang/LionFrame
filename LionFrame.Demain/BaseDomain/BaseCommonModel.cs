using System;

namespace LionFrame.Domain.BaseDomain
{
    public abstract class BaseCommonModel : BaseModel
    {
        protected BaseCommonModel()
        {
            Deleted = false;
            CreatedTime = DateTime.Now;
            UpdatedTime = DateTime.Now;
        }
        public bool Deleted { get; set; }
        public DateTime CreatedTime { get; set; }
        public long CreatedBy { get; set; }
        public DateTime UpdatedTime { get; set; }
        public long UpdatedBy { get; set; }
    }
}
