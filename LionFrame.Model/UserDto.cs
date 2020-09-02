using System.ComponentModel.DataAnnotations;

namespace LionFrame.Model
{
    public class UserDto
    {
        [Required(ErrorMessage = "用户名必需")]
        public string UserName { get; set; }
        public string Age { get; set; }
        public string Address { get; set; }
    }
}
