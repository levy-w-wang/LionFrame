using LionFrame.Domain.BaseDomain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LionFrame.Domain
{
    [Table("user")]
    public class User : BaseModel
    {
        [Key]
        public long Id { get; set; }

        [Required, MaxLength(20)]
        public string UserName { get; set; }
        public int Age { get; set; }

        public Address Address { get; set; }
    }
}
