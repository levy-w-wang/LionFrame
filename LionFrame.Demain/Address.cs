using LionFrame.Domain.BaseDomain;
using System.ComponentModel.DataAnnotations;

namespace LionFrame.Domain
{
    public class Address : BaseModel
    {
        [Key]
        public long Id { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Area { get; set; }
        public string Detail { get; set; }
    }
}
